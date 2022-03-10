using Azure;
using Moq;

namespace API.IntegrationTests.Common
{
    public class Response<T> : Azure.Response<T> where T : class
    {
        public int Status;
        private T _object;

        public Response(T obj, int status)
        {
            Status = status;
            _object = obj;
        }

        public override T Value => _object;

        public override Response GetRawResponse()
        {
            var mock = new Mock<Azure.Response>();
            mock.Setup(x => x.Status).Returns(Status);

            return mock.Object;
        }
    }
}
