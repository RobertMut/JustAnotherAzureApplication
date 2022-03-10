using System.Collections.Generic;
using System.Linq;

namespace API.IntegrationTests.Common
{
    public class Pageable<T> : Azure.Pageable<T> where T : class
    {
        private readonly List<T> _list;
        private readonly Page<T> _page;

        public Pageable(IEnumerable<T> enumerable) : base()
        {
            _list = enumerable.ToList();
            _page = new Page<T>(_list);
        }

        public override IEnumerable<Azure.Page<T>> AsPages(string? continuationToken = null, int? pageSizeHint = null)
        { 
            return new List<Page<T>>() { _page };
        }
    }
}
