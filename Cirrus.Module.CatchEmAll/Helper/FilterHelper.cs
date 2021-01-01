using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Helper
{
    internal static class FilterHelper
    {
        public static async Task<Filter> LoadFilterOrCreateDefaultAsync(IDataAccess<ICatchEmAllEntityContext> dataAccess, string workflowStepId, Func<Filter> defaultFilerFactory)
        {
            using (var context = dataAccess.GetContext())
            {
                var filter = await context.NoTracking<DAL.Entities.SavedFilter>()
                    .BelongingToCurrentUser()
                    .Where(e => e.WorkflowStepId == workflowStepId)
                    .SingleOrDefaultAsync();

                if (filter != null)
                {
                    return new Filter
                    {
                        OrderByProperties = filter.OrderByProperties,
                        OrderByAscending = filter.OrderByAscending
                    };
                }

                return defaultFilerFactory();
            }
        }

        public static async Task SaveFilterAsync(IDataAccess<ICatchEmAllEntityContext> dataAccess, string workflowStepId, Filter usedFilter)
        {
            if (usedFilter == null)
            {
                return;
            }

            using (var context = dataAccess.GetContext())
            {
                var filter = await context.Tracking<DAL.Entities.SavedFilter>()
                    .BelongingToCurrentUser()
                    .Where(e => e.WorkflowStepId == workflowStepId)
                    .SingleOrDefaultAsync();

                if (filter == null)
                {
                    filter = await context.AddAsync(new DAL.Entities.SavedFilter
                    {
                        User = await context.GetOrCreateUserReferenceAsync(),
                        WorkflowStepId = workflowStepId
                    });
                }

                filter.OrderByProperties = usedFilter.OrderByProperties;
                filter.OrderByAscending = usedFilter.OrderByAscending;

                await context.SaveChangesAsync();
            }
        }
    }
}
