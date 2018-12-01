using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using WebRequestTester.Models;

namespace WebRequestTester.ViewModels
{
    public class ShellViewModel : Screen
    {
        private string _browserAddress;
        private string _httpGetResponse;
        private WebRequester _webRequester;

        public string BrowserAddress
        {
            get => _browserAddress;
            set
            {
                _browserAddress = value;
                NotifyOfPropertyChange(() => BrowserAddress);
            }
        }

        public string GetResponse => _httpGetResponse;

        /// <summary>
        /// Sends the http get.
        /// </summary>
        public async void SendGet()
        {
            _httpGetResponse = await _webRequester.SendGetToURL(_browserAddress);
            NotifyOfPropertyChange(() => GetResponse);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            _browserAddress = "http://localhost:3000/authors";
            _httpGetResponse = string.Empty;
            _webRequester = new WebRequester();
        }
    }
}
