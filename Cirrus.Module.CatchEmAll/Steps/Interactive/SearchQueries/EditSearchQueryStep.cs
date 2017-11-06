using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Attributes;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.ViewModel.Extended;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.SearchQueries
{
    [Step]
    public interface IEditSearchQueryStep : IEditStep
    {
    }

    internal class EditSearchQueryStep : EditStep<Models.SearchQuery, DAL.Entities.SearchQuery, ICatchEmAllEntityContext>, IEditSearchQueryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        internal EditSearchQueryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.SearchQuery> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task<Models.SearchQuery> LoadAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var categories = await context.NoTracking<DAL.Entities.Category>()
                    .BelongingToCurrentUser()
                    .Select(e => new DropDownValue { Id = e.Id, Name = e.Name + " (" + e.Number + ")" })
                    .ToListAsync();

                var tags = await context.NoTracking<DAL.Entities.Tag>()
                    .BelongingToCurrentUser()
                    .Select(e => new PossibleValue { Id = e.Id.ToString(), Name = e.Name })
                    .OrderBy(e => e.Name)
                    .ToListAsync();

                var dto = await context.NoTracking<DAL.Entities.SearchQuery>()
                    .Select(e => new Models.SearchQuery
                    {
                        Id = e.Id,
                        Name = e.Name,
                        UseDescription = e.UseDescription,
                        WithAllTheseWords = e.WithAllTheseWords,
                        WithExactlyTheseWords = e.WithExactlyTheseWords,
                        WithNoneOfTheseWords = e.WithNoneOfTheseWords,
                        WithOneOfTheseWords = e.WithOneOfTheseWords,
                        EnableNotifications = e.EnableNotifications,
                        NotificationMode = e.NotificationMode,
                        DesiredPrice = e.DesiredPrice,
                        AutoFilterDeletedDuplicates = e.AutoFilterDeletedDuplicates,
                        TagSelection = new Tag
                        {
                            CanAddValues = true,
                            Values = e.Tags.Select(x => new PossibleValue
                            {
                                Id = x.Id.ToString(),
                                Name = x.Name
                            })
                            .OrderBy(x => x.Name)
                            .ToList()
                        },
                        CategorySelection = new DropDown
                        {
                            Id = e.CategoryId
                        },
                        ExecutionCount = e.Executions.Count(),
                        Executions = e.Executions
                            .OrderByDescending(x => x.Start)
                            .Select(x => new Models.ExecutionSummary
                            {
                                Id = x.Id,
                                Start = x.Start,
                                End = x.End,
                                Successful = x.Successful,
                                IsUserInitiated = x.IsUserInitiated
                            })
                            .Take(5)
                            .ToList()
                    })
                    .FirstOrDefaultAsync(e => e.Id == this.Id);

                dto.CategorySelection.PossibleValues = categories;
                dto.TagSelection.PossibleValues = tags;

                return dto;
            }
        }

        protected override async Task SaveAsync(Models.SearchQuery dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.Tracking<DAL.Entities.SearchQuery>().FirstOrDefaultAsync(e => e.Id == this.Id);

                var tagNames = dto.TagSelection.Values.Select(e => e.Name);

                var tags = await context.Tracking<DAL.Entities.Tag>()
                    .BelongingToCurrentUser()
                    .Where(e => tagNames.Contains(e.Name))
                    .ToListAsync();

                foreach (var tagName in tagNames.Where(t1 => !tags.Any(t2 => t2.Name == t1)))
                {
                    var newTag = new DAL.Entities.Tag
                    {
                        Name = tagName,
                        User = await context.GetOrCreateUserReferenceAsync()
                    };
                    await context.AddAsync(newTag);
                    tags.Add(newTag);
                }

                entity.Name = dto.Name;
                entity.UseDescription = dto.UseDescription;
                entity.WithAllTheseWords = dto.WithAllTheseWords;
                entity.WithExactlyTheseWords = dto.WithExactlyTheseWords;
                entity.WithNoneOfTheseWords = dto.WithNoneOfTheseWords;
                entity.WithOneOfTheseWords = dto.WithOneOfTheseWords;
                entity.EnableNotifications = dto.EnableNotifications;
                entity.NotificationMode = dto.NotificationMode;
                entity.DesiredPrice = dto.DesiredPrice;
                entity.AutoFilterDeletedDuplicates = dto.AutoFilterDeletedDuplicates;
                entity.Tags.Replace(tags);
                entity.TagValues = string.Join("|", tags.OrderBy(t => t.Name).Select(t => t.Name));

                await context.SaveChangesAsync();
            }
        }

        protected override EditViewModel InterceptViewModel(EditViewModel viewModel, Models.SearchQuery dto)
        {
            viewModel.As<Models.SearchQuery>().Attributes(x => x.CategorySelection).Add(new ReadonlyAttribute());

            if (dto.NotificationMode != DAL.Entities.NotificationMode.OnlyBelowDesired)
            {
                viewModel.As<Models.SearchQuery>().Attributes(m => m.DesiredPrice).Add(new HiddenAttribute());
            }

            return base.InterceptViewModel(viewModel, dto);
        }
    }
}
