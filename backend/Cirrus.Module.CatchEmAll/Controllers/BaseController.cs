using System;
using System.Net;
using Cirrus.DAL;
using Cirrus.Engine.Interface;
using Cirrus.Engine.Scheduler;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Controllers
{
    [ResponseType(typeof(DefaultResponse), StatusCode = HttpStatusCode.Unauthorized)]
    public abstract class BaseController : Engine.Interface.BaseController
    {
        private IOwned<IDataAccess<ICatchEmAllEntityContext>> dataAccess;
        private IOwned<IBatchScheduler> scheduler;

        internal IDataAccess<ICatchEmAllEntityContext> DataAccess => Setup.Resolve(ref this.dataAccess);

        internal Lazy<IBatchScheduler> Scheduler => new Lazy<IBatchScheduler>(() =>
        {
            Setup.Resolve(ref this.scheduler);
            return this.scheduler.Value;
        });

        internal BaseController()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataAccess?.Dispose();
                this.scheduler?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
