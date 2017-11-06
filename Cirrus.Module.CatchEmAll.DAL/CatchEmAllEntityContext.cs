using System.Data.Entity;
using Cirrus.DAL;
using Cirrus.Logging;
using Cirrus.Module.CatchEmAll.DAL.Entities;

namespace Cirrus.Module.CatchEmAll.DAL
{
    internal class CatchEmAllEntityContext : EntityContext, ICatchEmAllEntityContext
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        static CatchEmAllEntityContext()
        {
            Database.SetInitializer<CatchEmAllEntityContext>(null);
        }

        public CatchEmAllEntityContext()
            : this("Cirrus.Modules")
        {
        }

        public CatchEmAllEntityContext(string nameOrConnectionString)
            : base(Logger, nameOrConnectionString, null)
        {
        }

        public DbSet<UserReference> Users { get; set; }

        public DbSet<SearchQuery> SearchQueries { get; set; }

        public DbSet<SearchResult> SearchResults { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<QueryExecution> QueryExecutions { get; set; }

        public DbSet<ResultExecution> ResultExecutions { get; set; }

        public DbSet<Settings> Settings { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<SavedFilter> SavedFilters { get; set; }

        internal static void ModelBuilding(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<SearchQuery>()
                .HasMany(e => e.Schedules)
                .WithMany(e => e.Queries)
                .Map(m => m.ToTable("SearchQuerySchedules", "catchemall").MapLeftKey("SearchQueryId").MapRightKey("ScheduleId"));

            modelBuilder
                .Entity<SearchQuery>()
                .HasMany(e => e.Tags)
                .WithMany(e => e.Queries)
                .Map(m => m.ToTable("SearchQueryTags", "catchemall").MapLeftKey("SearchQueryId").MapRightKey("TagId"));

            modelBuilder
                .Entity<Tag>()
                .HasRequired(e => e.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Category>()
                .HasRequired(e => e.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Feedback>()
                .HasRequired(e => e.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Settings>()
                .HasRequired(e => e.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<SavedFilter>()
                .HasRequired(e => e.User)
                .WithMany()
                .WillCascadeOnDelete(false);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ModelBuilding(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
