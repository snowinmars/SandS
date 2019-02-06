using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Common.SandS
{
    public static class RequestReaderExtensions
    {
        public static async Task<T> ReadResponseAsync<T>(this HttpWebRequest request)
        {
            var response = await request.GetResponseAsync().ConfigureAwait(false);

            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                {
                    return default;
                }

                using (var reader = new StreamReader(stream))
                {
                    var jsonResult = await reader.ReadToEndAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<T>(jsonResult);
                }
            }
        }
    }
}
