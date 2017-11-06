using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.DAL.Extensions;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.Helper;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.Notifications
{
    [Step]
    public interface ITriggerIftttWebhookStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        [Input]
        ICollection<long> ResultIds { get; set; }
    }

    internal class TriggerIftttWebhookStep : ITriggerIftttWebhookStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public ICollection<long> ResultIds { get; set; } = new List<long>();

        public TriggerIftttWebhookStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
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

                    var iftttSettings = await context.NoTracking<DAL.Entities.Settings>()
                        .Where(s => s.User.UserId == query.Category.User.UserId && s.EnableIftttNotification)
                        .Select(s => new
                        {
                            s.IftttMakerKey,
                            s.IftttMakerEventName
                        })
                        .SingleOrDefaultAsync();

                    if (query.EnableNotifications && !string.IsNullOrWhiteSpace(iftttSettings?.IftttMakerKey) && !string.IsNullOrWhiteSpace(iftttSettings?.IftttMakerEventName))
                    {
                        var url = $"https://maker.ifttt.com/trigger/{iftttSettings.IftttMakerEventName}/with/key/{iftttSettings.IftttMakerKey}";
                        var content = new
                        {
                            value1 = new
                            {
                                Name = query.Name,
                                Url = SearchQueryTransformations.EntityToUrl(query)
                            },
                            value2 = results.Select(result => new
                            {
                                Name = result.Name,
                                Description = result.Description,
                                BidPrice = result.BidPrice,
                                PurchasePrice = result.PurchasePrice,
                                Url = SearchResultTransformations.EntityToUrl(result)
                            })
                            .ToArray()
                        };

                        var client = new HttpClient();
                        var response = await client.PostAsJsonAsync(url, content);
                    }

                    return this.Next;
                }
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Error(CLMTIDs.CouldNotTriggerIftttWebhook, e, "Could not trigger IFTTT webhook notification for result ids {ResultIds}!", this.ResultIds);
                return this.Next;
            }
        }
    }
}
