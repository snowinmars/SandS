using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web;

using Newtonsoft.Json;

namespace Common.SandS
{
    public class RequestBuilder
    {
        private readonly IDictionary<string, string> headers;
        private readonly IDictionary<string, string> parameters;

        private readonly Uri uri;
        private HttpVerbs verb;
        private ContentType contentType;
        private byte[] data;

        private HttpWebRequest request;

        public RequestBuilder(string pureUrl) : this(new Uri(pureUrl))
        {
        }

        public RequestBuilder(Uri uri) : this(uri, HttpVerbs.Post)
        {
        }

        public RequestBuilder(Uri uri, HttpVerbs verb)
        {
            this.uri = uri;
            this.verb = verb;
            contentType = Common.SandS.ContentType.Json;
            data = new byte[0];

            headers = new Dictionary<string, string>();
            parameters = new Dictionary<string, string>();
        }

        public RequestBuilder QueryParameters<T>(IDictionary<string, T> parameters)
        {
            foreach (var parameter in parameters)
            {
                QueryParameter(parameter.Key, parameter.Value);
            }

            return this;
        }

        public RequestBuilder QueryParameter<T>(string name, T value)
        {
            parameters.Add(name, value.ToString());

            return this;
        }

        public RequestBuilder Method(HttpVerbs method)
        {
            this.verb = method;

            return this;
        }

        public RequestBuilder ContentType(ContentType contentType)
        {
            this.contentType = contentType;

            return this;
        }

        public RequestBuilder Data<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data);

            return Data(Encoding.UTF8.GetBytes(json));
        }

        public RequestBuilder Data(string data)
        {
            return Data(Encoding.UTF8.GetBytes(data));
        }

        public RequestBuilder Data(byte[] data)
        {
            if (data != null)
            {
                if (this.verb == HttpVerbs.Get)
                {
                    throw new InvalidOperationException();
                }

                this.data = data;
            }

            return this;
        }

        public RequestBuilder Headers(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                Header(header.Key, header.Value);
            }

            return this;
        }

        public RequestBuilder Header(string key, string value)
        {
            this.headers.Add(key, value);

            return this;
        }

        public HttpWebRequest Build()
        {
            var url = BuildUrl();

            request = (HttpWebRequest)WebRequest.Create(url);

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            request.Method = verb.ToString();
            request.ContentType = Stringify(contentType);

            request.ContentLength = data.Length;

            if (data.Length > 0)
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            return request;
        }

        private string BuildUrl()
        {
            var urlBuilder = new UriBuilder(uri);

            var query = HttpUtility.ParseQueryString(urlBuilder.Query);

            foreach (var parameter in parameters)
            {
                query[parameter.Key] = parameter.Value;
            }

            urlBuilder.Query = query.ToString();

            var url = urlBuilder.ToString();
            return url;
        }

        private string Stringify(ContentType contentType)
        {
            switch (contentType)
            {
                case Common.SandS.ContentType.Json:
                    return "application/json; charset=utf-8";

                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
            }
        }
    }
}
