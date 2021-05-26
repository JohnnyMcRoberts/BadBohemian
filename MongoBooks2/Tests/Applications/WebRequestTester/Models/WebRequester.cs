using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;

namespace WebRequestTester.Models
{
    public class WebRequester
    {
        /// <summary>
        /// The http client.
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// Get the initialized http client.
        /// </summary>
        public HttpClient HttpClient => _httpClient ?? (_httpClient = new HttpClient());

        /// <summary>
        /// Sends a GET request to an HTTP service.
        /// </summary>
        public async Task<string> SendGetToURL(string url)
        {
            try
            {
                string result = await HttpClient.GetStringAsync(url);
                string checkResult = result;
                HttpClient.Dispose();
                _httpClient = null;
                return checkResult;
            }
            catch (Exception ex)
            {
                string checkResult = "Error " + ex;
                HttpClient.Dispose();
                _httpClient = null;
                return checkResult;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequester"/> class.
        /// </summary>
        public WebRequester()
        {
            _httpClient = new HttpClient();
        }
    }
}
