using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.Notifications
{
    [Step]
    public interface ISaveResultsStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        [Input]
        ICollection<long> ResultIds { get; set; }
    }

    internal class SaveResultsStep : ISaveResultsStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public ICollection<long> ResultIds { get; set; } = new List<long>();

        public SaveResultsStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            try
            {
                using (var context = this.dataAccess.GetContext())
                {
                    var results = await context.Tracking<DAL.Entities.SearchResult>()
                        .Where(e => this.ResultIds.Contains(e.Id))
                        .ToListAsync();

                    foreach (var result in results)
                    {
                        result.Notified = true;
                    }

                    await context.SaveChangesAsync();
                }

                return this.Next;
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Error(CLMTIDs.CouldNotSaveResults, e, "Could not save results with ids {ResultIds}!", this.ResultIds);
                return this.Next;
            }
        }
    }
}
