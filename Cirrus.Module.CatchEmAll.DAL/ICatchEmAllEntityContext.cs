using System.Data.Entity;
using Cirrus.DAL;
using Cirrus.Module.CatchEmAll.DAL.Entities;

namespace Cirrus.Module.CatchEmAll.DAL
{
    public interface ICatchEmAllEntityContext : IEntityContext
    {
        DbSet<UserReference> Users { get; }

        DbSet<SearchQuery> SearchQueries { get; }

        DbSet<SearchResult> SearchResults { get; }

        DbSet<Category> Categories { get; }

        DbSet<Schedule> Schedules { get; }

        DbSet<QueryExecution> QueryExecutions { get; }

        DbSet<ResultExecution> ResultExecutions { get; }

        DbSet<Settings> Settings { get; }

        DbSet<Feedback> Feedbacks { get; }

        DbSet<Tag> Tags { get; }

        DbSet<SavedFilter> SavedFilters { get; }
    }
}
