using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Workflows.Interactive;
using HandlebarsDotNet;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.Notifications
{
    [Step]
    public interface ISendEmailStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        [Input]
        ICollection<long> ResultIds { get; set; }
    }

    internal class SendEmailStep : ISendEmailStep
    {
        private static readonly Func<object, string> BodyFormatter;

        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public ICollection<long> ResultIds { get; set; } = new List<long>();

        static SendEmailStep()
        {
            Handlebars.RegisterHelper("price", (writer, context, parameters) =>
            {
                if (parameters[0] == null || parameters[0].GetType().Name == "UndefinedBindingResult")
                    writer.Write("-.-- CHF");
                else
                    writer.Write(string.Format("{0:F2} CHF", parameters[0]));
            });

            Handlebars.RegisterHelper("workflowUrl", (writer, context, parameters) =>
            {
                // todo var baseUrl = Configuration.CirrusConfiguration.ServerAddress("Website");
                var baseUrl = "https://example.com";
                var workflowId = string.Join("/", (parameters[0] as string).Split('/').Select(p => HttpUtility.UrlEncode(p)).ToArray());
                writer.Write(string.Format("{0}/core/workflow/{1}", baseUrl, workflowId));
            });

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cirrus.Module.CatchEmAll.Templates.NewResultsNotification.html"))
            using (var reader = new StreamReader(stream))
            {
                var template = reader.ReadToEnd();
                BodyFormatter = Handlebars.Compile(template);
            }
        }

        public SendEmailStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            try
            {
                using (var context = this.dataAccess.GetContext())
                {
                    var results = await context.NoTracking<DAL.Entities.SearchResult>()
                        .Include(r => r.Query)
                        .Include(r => r.Query.Category)
                        .Include(r => r.Query.Category.User)
                        .Where(r => this.ResultIds.Contains(r.Id))
                        .ToListAsync();

                    var query = results.First().Query;

                    var tos = (await context.NoTracking<DAL.Entities.Settings>()
                        .Where(s => s.User.UserId == query.Category.User.UserId && s.EnableEmailNotification)
                        .ToListAsync())
                        .SelectMany(s => new[] { s.Email, s.AlternativeEmail })
                        .Where(m => !string.IsNullOrWhiteSpace(m))
                        .Select(m => new EmailAddress(m))
                        .ToList();

                    if (query.EnableNotifications && tos.Count > 0)
                    {
                        var from = new EmailAddress("me@example.com");
                        var subject = $"Catch 'Em All Notification: {query.Name}";
                        var body = BodyFormatter(new
                        {
                            Name = query.Name,
                            Url = Cirrus.Helpers.WorkflowNameBuilder.ToString(WorkflowIds.SearchQueryByIdWorkflow, new Dictionary<string, object>
                            {
                                { SearchQueryByIdWorkflow.InputQueryId, query.Id },
                                { SearchQueryByIdWorkflow.InputCanGoBack, false }
                            }),
                            Results = results.Select(r => new
                            {
                                r.Name,
                                r.Description,
                                r.BidPrice,
                                r.PurchasePrice,
                                ExternalUrl = SearchResultTransformations.EntityToUrl(r)
                            })
                        });
                        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, body, body);
                        var client = new SendGridClient("sendgrid-api-token");
                        var response = await client.SendEmailAsync(msg);
                    }

                    return this.Next;
                }
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Error(CLMTIDs.CouldNotSendEmail, e, "Could not send notification email for result ids {ResultIds}!", this.ResultIds);
                return this.Next;
            }
        }
    }
}