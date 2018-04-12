using System;
using System.Collections.Generic;
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
    public interface ICreateSearchQueryStep : ICreateStep
    {
        [Input(Optional = true)]
        long? CategoryId { get; set; }
    }

    internal class CreateSearchQueryStep : CreateStep<Models.SearchQuery, DAL.Entities.SearchQuery, ICatchEmAllEntityContext>, ICreateSearchQueryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public long? CategoryId { get; set; }

        public CreateSearchQueryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.SearchQuery> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task<Models.SearchQuery> LoadAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var settings = await context.NoTracking<DAL.Entities.Settings>()
                    .BelongingToCurrentUser()
                    .Select(e => new
                    {
                        e.EnableNotificationsDefault,
                        e.AutoFilterDeletedDuplicatesDefault
                    })
                    .SingleOrDefaultAsync();

                var categories = await context.NoTracking<DAL.Entities.Category>()
                    .BelongingToCurrentUser()
                    .Where(e => !this.CategoryId.HasValue || e.Id == this.CategoryId.Value)
                    .Select(e => new DropDownValue { Id = e.Id, Name = e.Name + " (" + e.Number + ")" })
                    .ToListAsync();

                var tags = await context.NoTracking<DAL.Entities.Tag>()
                    .BelongingToCurrentUser()
                    .Select(e => new PossibleValue { Id = e.Id.ToString(), Name = e.Name })
                    .OrderBy(e => e.Name)
                    .ToListAsync();

                var conditions = Enum.GetValues(typeof(DAL.Entities.Condition)).Cast<DAL.Entities.Condition>()
                    .Select(x => new PossibleValue { Id = ((int)x).ToString(), Name = Enum.GetName(typeof(DAL.Entities.Condition), x) })
                    .ToList();

                return new Models.SearchQuery
                {
                    CategorySelection = new DropDown
                    {
                        Id = this.CategoryId.GetValueOrDefault(),
                        PossibleValues = categories
                    },
                    EnableNotifications = settings?.EnableNotificationsDefault ?? true,
                    NotificationMode = DAL.Entities.NotificationMode.All,
                    AutoFilterDeletedDuplicates = settings?.AutoFilterDeletedDuplicatesDefault ?? false,
                    TagSelection = new Tag
                    {
                        CanAddValues = true,
                        Values = new List<PossibleValue>(),
                        PossibleValues = tags
                    },
                    Condition = new Tag
                    {
                        CanAddValues = false,
                        Values = conditions,
                        PossibleValues = conditions
                    }
                };
            }
        }

        protected override async Task SaveAsync(Models.SearchQuery dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
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

                var condition = dto.Condition.Values.Select(x => (DAL.Entities.Condition)int.Parse(x.Id)).Aggregate((a, b) => a | b);

                var entity = context.AddAsync(new DAL.Entities.SearchQuery
                {
                    Name = dto.Name,
                    UseDescription = dto.UseDescription,
                    WithAllTheseWords = dto.WithAllTheseWords,
                    WithOneOfTheseWords = dto.WithOneOfTheseWords,
                    WithExactlyTheseWords = dto.WithExactlyTheseWords,
                    WithNoneOfTheseWords = dto.WithNoneOfTheseWords,
                    CategoryId = dto.CategorySelection.Id,
                    EnableNotifications = dto.EnableNotifications,
                    NotificationMode = dto.NotificationMode,
                    DesiredPrice = dto.DesiredPrice,
                    AutoFilterDeletedDuplicates = dto.AutoFilterDeletedDuplicates,
                    Tags = tags,
                    TagValues = string.Join("|", tags.OrderBy(t => t.Name).Select(t => t.Name)),
                    Condition = condition
                });

                await context.SaveChangesAsync();

                this.Id = entity.Id;
            }
        }

        protected override EditViewModel InterceptViewModel(EditViewModel viewModel, Models.SearchQuery dto)
        {
            if (this.CategoryId.HasValue)
            {
                viewModel.As<Models.SearchQuery>().Attributes(m => m.CategorySelection).Add(new ReadonlyAttribute());
            }

            if (dto.NotificationMode != DAL.Entities.NotificationMode.OnlyBelowDesired)
            {
                viewModel.As<Models.SearchQuery>().Attributes(m => m.DesiredPrice).Add(new HiddenAttribute());
            }

            return base.InterceptViewModel(viewModel, dto);
        }
    }
}
