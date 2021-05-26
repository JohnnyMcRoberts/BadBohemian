namespace WebRequestTester.ViewModels
{
    using System;
    using System.IO;
    using Caliburn.Micro;
    
    using WebRequestTester.Models;

    using BooksMailbox;
    using Newtonsoft.Json;

    public class ShellViewModel : Screen
    {
        public readonly string TestEmailParametersFile = "testEmailParameters.json";
        private string _browserAddress;
        private string _httpGetResponse;
        private WebRequester _webRequester;

        private TestEmailParameters _emailParameters;

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
        /// Gets the full path to a file in the bin directory.
        /// </summary>
        /// <param name="filename">The name of the config file.</param>
        /// <returns>The full path for the local JSON file.</returns>
        public static string GetPathToConfigFile(string filename)
        {
            try
            {
                string projectPath = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(projectPath, filename);
            }
            catch (Exception ex)
            {
                return ("Directory for file not found: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the text from the pull path to a file.
        /// </summary>
        /// <param name="filePath">The full file path of the file.</param>
        /// <param name="text">The text within the file if exists, null otherwise.</param>
        /// <returns>The true if the file exists, false otherwise.</returns>
        public static bool GetFileTextFromPath(out string text, string filePath)
        {
            text = null;
            if (!File.Exists(filePath))
            {
                return false;
            }

            text = File.ReadAllText(filePath);
            return true;
        }

        /// <summary>
        /// Sends the http get.
        /// </summary>
        public async void SendGet()
        {
            _httpGetResponse = await _webRequester.SendGetToURL(_browserAddress);
            NotifyOfPropertyChange(() => GetResponse);
        }


        /// <summary>
        /// Sends the http get.
        /// </summary>
        public async void SendMail()
        {
            //SmtpEmailer.SendNicerEmail();
            //.SendTestEmail();
            //SendBasicEmail.SendTestEmail();

            StmpConnection connection = 
                new StmpConnection(_emailParameters.FromEmail, _emailParameters.Password);

            SmtpEmailer.SendTestEmail(connection, _emailParameters.ToEmail);

            HtmlEmailDefinition emailDefinition = new HtmlEmailDefinition()
            {
                FromEmailDisplayName = _emailParameters.FromEmailDisplayName,
                ToEmail = _emailParameters.ToEmail, 
                ToEmailDisplayName = _emailParameters.ToEmailDisplayName,
                Subject = "Your ace from outer space",
                BodyHtml = "Sometimes it fine <em>It's great to use HTML in mail!!</em> and <b>SUPER!!!!</b> is how it is."
            };

            SmtpEmailer.SendHtmlEmail(connection,emailDefinition);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            _browserAddress = "http://localhost:3000/authors";
            _httpGetResponse = string.Empty;
            _webRequester = new WebRequester();

            // set dummy values
            _emailParameters = 
                new TestEmailParameters
                {
                    FromEmail = "abs@abc.com",
                    FromEmailDisplayName = "Big jim",
                    ToEmail = "joe@the.house.gov",
                    ToEmailDisplayName = "terry the thief",
                    Password = "fillMeIn!"
                };

            string fullPath = GetPathToConfigFile(TestEmailParametersFile);

            if (GetFileTextFromPath(out string text, fullPath))
            {
                _emailParameters =
                    JsonConvert.DeserializeObject<TestEmailParameters>(
                        text,
                        new JsonSerializerSettings
                        {
                            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                            TypeNameHandling = TypeNameHandling.All
                        });
            }

        }
    }
}
