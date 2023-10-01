using System.IO;
using Uhost.Core.Models.Graylog;
using Uhost.Core.Services.RestClient;

namespace Uhost.Core.Services.Graylog
{
    public sealed class GraylogService : IGraylogService
    {
        private readonly IRestClientService _client;

        public GraylogService(IRestClientService client)
        {
            _client = client;
        }

        public MemoryStream GetGraylogCsvStream(GraylogRestQueryModel query)
        {
            var response = _client.GetResponse(query);

            if (response.IsSuccessStatusCode)
            {
                var stream = new MemoryStream();
                response.Content
                    .ReadAsStream()
                    .CopyTo(stream);
                stream.Position = 0;

                return stream;
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
