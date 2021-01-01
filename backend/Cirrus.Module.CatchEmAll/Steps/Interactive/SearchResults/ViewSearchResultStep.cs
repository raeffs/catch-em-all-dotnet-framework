using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.ViewModel.Extended;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.SearchResults
{
    [Step]
    public interface IViewSearchResultStep : IEditStep
    {
        EdgeDefinition UpdateFromSource { get; }
    }

    internal class ViewSearchResultStep : EditStep<Models.SearchResult, DAL.Entities.SearchResult, ICatchEmAllEntityContext>, IViewSearchResultStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition UpdateFromSource { get; } = new EdgeDefinition();

        internal ViewSearchResultStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.SearchResult> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task<Models.SearchResult> LoadAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.Tracking<DAL.Entities.SearchResult>()
                    .Where(e => e.Id == this.Id)
                    .SingleAsync();

                entity.New = false;

                await context.SaveChangesAsync();

                var dto = new Models.SearchResult
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    FinalPrice = entity.FinalPrice,
                    BidPrice = entity.BidPrice,
                    PurchasePrice = entity.PurchasePrice,
                    Ends = entity.Ends,
                    Sold = entity.Sold,
                    Closed = entity.Closed,
                    Favorite = entity.Favorite,
                    ExternalData = new Models.ExternalSearchResultData
                    {
                        Id = entity.Id,
                        ExternalId = entity.ExternalId
                    }
                };

                dto.ExternalData.ExternalUrl = SearchResultTransformations.ModelToUrl(dto);

                return dto;
            }
        }

        protected override async Task<List<ViewModelAction>> CreateActionsAsync(Models.SearchResult dto)
        {
            var actions = await base.CreateActionsAsync(dto);

            actions.RemoveAction(DisplayActions.Save);
            actions.RemoveAction(DisplayActions.AddToDashboard);

            actions.Add(new ViewModelAction(DisplayActions.AddToDashboard) { Type = ViewModelActionType.Toolbar, Icon = dto.Favorite ? "favorite" : "favorite_border" });

            if (!dto.Closed)
            {
                actions.Add(new ViewModelAction(DisplayActions.CatchEmAll_UpdateFromSource) { Color = ViewModelActionColor.Primary });
            }

            actions.Add(new ViewModelAction(DisplayActions.Delete) { Type = ViewModelActionType.None });

            return actions;
        }

        protected override async Task<EdgeDefinition> ProcessSuccessfulResponseAsync(ViewModelResult<EditViewModel> response, Models.SearchResult dto)
        {
            if (response.IsAction(DisplayActions.CatchEmAll_UpdateFromSource))
                return this.UpdateFromSource;

            if (response.IsAction(DisplayActions.AddToDashboard))
            {
                using (var context = this.dataAccess.GetContext())
                {
                    var entity = await context.Tracking<DAL.Entities.SearchResult>()
                        .Where(e => e.Id == this.Id)
                        .SingleAsync();

                    entity.Favorite = !entity.Favorite;

                    await context.SaveChangesAsync();
                }

                return this.Saved; // forces reload of the dto
            }

            if (response.IsAction(DisplayActions.Delete))
            {
                using (var context = this.dataAccess.GetContext())
                {
                    var entity = await context.Tracking<DAL.Entities.SearchResult>()
                        .Where(e => e.Id == this.Id)
                        .SingleAsync();

                    entity.Hidden = true;

                    await context.SaveChangesAsync();
                }

                return this.Back;
            }

            return await base.ProcessSuccessfulResponseAsync(response, dto);
        }

        protected override EditViewModel InterceptViewModel(EditViewModel viewModel, Models.SearchResult dto)
        {
            if (!dto.Sold)
            {
                viewModel.As<Models.SearchResult>().Hide(p => p.FinalPrice);
            }

            if (dto.Sold)
            {
                viewModel.As<Models.SearchResult>().Hide(p => p.PurchasePrice);
                viewModel.As<Models.SearchResult>().Hide(p => p.BidPrice);
                viewModel.As<Models.SearchResult>().Hide(p => p.Ends);
            }

            return base.InterceptViewModel(viewModel, dto);
        }

        protected override Task SaveAsync(Models.SearchResult dto)
        {
            throw new NotImplementedException();
        }
    }
}
