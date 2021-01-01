using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Readonly]
    [Template("@cirrus/module.catch-em-all.ui#ExternalResultDataComponent")]
    [Name(nameof(DataObjects.CatchEmAll_SearchResult), nameof(DataObjects.CatchEmAll_SearchResults))]
    internal class ExternalSearchResultData : IHasIndex
    {
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        [Hidden]
        public long Id { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_ExternalId))]
        public long ExternalId { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_ExternalUrl))]
        public string ExternalUrl { get; set; }
    }
}
