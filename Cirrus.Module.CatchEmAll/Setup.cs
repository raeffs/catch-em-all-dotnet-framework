using System;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Setup;

namespace Cirrus.Module.CatchEmAll
{
    public class Setup : SetupModule
    {
        private static IContainer container;

        public override void PostBootstrap(IContainer buildContainer)
        {
            base.PostBootstrap(buildContainer);

            if (container != null)
                throw new InvalidOperationException($"{nameof(Setup)} was already instantiated.");

            container = buildContainer;
        }

        internal static T Resolve<T>(ref IOwned<T> factory) => (factory = factory ?? container.Resolve<Func<IOwned<T>>>()()).Value;

        protected override void BoostrapComponents()
        {
            CatchEmAllExtendingContext.SetAddHandlers();
            this.Builder.Register(c => new CatchEmAllEntityContext(Context.CurrentConfiguration?.Configurations["Cirrus.Modules"])).As<ICatchEmAllEntityContext>().As<IEntityContext>().InstancePerDependency();
            this.Builder.RegisterInstance(new CatchEmAllExtendingContext()).As<ExtendingContext>();

            this.Builder.RegisterType<NavigationSeed>().As<INavigationSeed>();
            this.Builder.RegisterType<UiModuleSeed>().As<IUiModuleSeed>();
        }

        protected override void BootstrapServices()
        {
        }

        protected override void BootstrapWorkflowSteps()
        {
        }
    }
}
