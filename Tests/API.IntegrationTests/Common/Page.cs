using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class Page<T> : Azure.Page<T> where T : class
    {
        private readonly IReadOnlyList<T> _list;

        public Page(IEnumerable<T> enumerable)
        {
            _list = enumerable.ToList();
        }
        public override IReadOnlyList<T> Values => _list;

        public override string? ContinuationToken => "";

        public override Response GetRawResponse()
        {
            throw new NotImplementedException();
        }
    }
}
