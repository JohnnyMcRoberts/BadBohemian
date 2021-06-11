namespace BooksMigration.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.DirectoryServices.ActiveDirectory;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;

    public class SettingsViewModel : PropertyChangedBase
    {
        #region Constants

        public readonly string ConnectionDetailsFile = "ConnectionDetails.resx";

        public enum PropertyName
        {
            LocalConnectionString,
            RemoteHost,
            RemotePassword,
            RemotePort,
            RemoteRetryWrites,
            RemoteUserName,
            RemoteUseSsl
        }

        public readonly Dictionary<PropertyName, string> PropertiesLookup = new Dictionary<PropertyName, string>
        {
            {PropertyName.LocalConnectionString, "LocalConnectionString"},
            {PropertyName.RemoteHost, "RemoteHost"},
            {PropertyName.RemotePassword, "RemotePassword"},
            {PropertyName.RemotePort, "RemotePort"},
            {PropertyName.RemoteRetryWrites, "RemoteRetryWrites"},
            {PropertyName.RemoteUserName, "RemoteUserName"},
            {PropertyName.RemoteUseSsl, "RemoteUseSsl"}
        };

        #endregion

        #region Local Data

        string _localConnectionString = string.Empty;

        string _remoteHost = string.Empty;

        int _remotePort = 10255;

        string _remoteUserName = string.Empty;

        string _remotePassword = string.Empty;

        bool _remoteUseSsl = true;

        bool _remoteRetryWrites = false;

        #endregion

        #region Public Properties

        public string LocalConnectionString
        {
            get { return _localConnectionString; }
            set
            {
                _localConnectionString = value;
                NotifyOfPropertyChange(() => LocalConnectionString);
                NotifyOfPropertyChange(() => CanApplySettings);
            }
        }

        public string RemoteHost
        {
            get { return _remoteHost; }
            set
            {
                _remoteHost = value;
                NotifyOfPropertyChange(() => RemoteHost);
                NotifyOfPropertyChange(() => CanApplySettings);
            }
        }

        public int RemotePort
        {
            get { return _remotePort; }
            set
            {
                _remotePort = value;
                NotifyOfPropertyChange(() => RemotePort);
                NotifyOfPropertyChange(() => CanApplySettings);
            }
        }

        public string RemoteUserName
        {
            get { return _remoteUserName; }
            set
            {
                _remoteUserName = value;
                NotifyOfPropertyChange(() => RemoteUserName);
                NotifyOfPropertyChange(() => CanApplySettings);
            }
        }

        public string RemotePassword
        {
            get { return _remotePassword; }
            set
            {
                _remotePassword = value;
                NotifyOfPropertyChange(() => RemotePassword);
                NotifyOfPropertyChange(() => CanApplySettings);
            }
        }

        public bool RemoteUseSsl
        {
            get { return _remoteUseSsl; }
            set
            {
                _remoteUseSsl = value;
                NotifyOfPropertyChange(() => RemoteUseSsl);
                NotifyOfPropertyChange(() => CanApplySettings);
            }
        }

        public bool RemoteRetryWrites
        {
            get { return _remoteRetryWrites; }
            set
            {
                _remoteRetryWrites = value;
                NotifyOfPropertyChange(() => RemoteRetryWrites);
                NotifyOfPropertyChange(() => CanApplySettings);
            }
        }

        public bool CanApplySettings =>
            !string.IsNullOrWhiteSpace(LocalConnectionString) &&
            !string.IsNullOrWhiteSpace(RemoteHost) &&
            !string.IsNullOrWhiteSpace(RemoteUserName) &&
            !string.IsNullOrWhiteSpace(RemotePassword) &&
            RemotePort > 0;

        #endregion

        #region Private Methods Methods

        private Dictionary<string, string> GetValues()
        {
            Dictionary<string, string> valuesDictionary = new Dictionary<string, string>();

            foreach (PropertyName prop in PropertiesLookup.Keys)
            {
                switch (prop)
                {
                    case PropertyName.LocalConnectionString:
                        valuesDictionary.Add(PropertiesLookup[prop], LocalConnectionString);
                        break;
                    case PropertyName.RemoteHost:
                        valuesDictionary.Add(PropertiesLookup[prop], RemoteHost);
                        break;
                    case PropertyName.RemotePassword:
                        valuesDictionary.Add(PropertiesLookup[prop], RemotePassword);
                        break;
                    case PropertyName.RemotePort:
                        valuesDictionary.Add(PropertiesLookup[prop], RemotePort.ToString());
                        break;
                    case PropertyName.RemoteRetryWrites:
                        valuesDictionary.Add(PropertiesLookup[prop], RemoteRetryWrites.ToString());
                        break;
                    case PropertyName.RemoteUserName:
                        valuesDictionary.Add(PropertiesLookup[prop], RemoteUserName);
                        break;
                    case PropertyName.RemoteUseSsl:
                        valuesDictionary.Add(PropertiesLookup[prop], RemoteUseSsl.ToString());
                        break;
                }
            }

            return valuesDictionary;
        }

        private void SetFromValues(Dictionary<string, string> valuesSet)
        {
            foreach (string propertyName in valuesSet.Keys)
            {
                if (!PropertiesLookup.ContainsValue(propertyName))
                    continue;

                KeyValuePair<PropertyName, string> property =
                    PropertiesLookup.FirstOrDefault(x => x.Value == propertyName);

                switch (property.Key)
                {
                    case PropertyName.LocalConnectionString:
                        LocalConnectionString = valuesSet[propertyName];
                        break;
                    case PropertyName.RemoteHost:
                        RemoteHost = valuesSet[propertyName];
                        break;
                    case PropertyName.RemotePassword:
                        RemotePassword = valuesSet[propertyName];
                        break;
                    case PropertyName.RemotePort:
                        RemotePort =
                            int.TryParse(valuesSet[propertyName], out _remotePort) ? int.Parse(valuesSet[propertyName]) : 0;
                        break;
                    case PropertyName.RemoteRetryWrites:
                        RemoteRetryWrites =
                            !bool.TryParse(valuesSet[propertyName], out _remoteRetryWrites) || bool.Parse(valuesSet[propertyName]);
                        break;
                    case PropertyName.RemoteUserName:
                        RemoteUserName = valuesSet[propertyName];
                        break;
                    case PropertyName.RemoteUseSsl:
                        RemoteUseSsl =
                            !bool.TryParse(valuesSet[propertyName], out _remoteUseSsl) || bool.Parse(valuesSet[propertyName]);
                        break;
                }
            }


        }

        private void WriteUpdatesToResource()
        {
            Dictionary<string, string> values = GetValues();

            using (ResXResourceWriter resx = new ResXResourceWriter(GetPathToConfigFile(ConnectionDetailsFile)))
            {
                foreach (KeyValuePair<string, string> pairValue in values)
                {

                    resx.AddResource(pairValue.Key, pairValue.Value);
                }
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Gets the full path to a file in the bin directory.
        /// </summary>
        /// <param name="filename">The name of the config file.</param>
        /// <returns>The full path for the local file.</returns>
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

        #endregion

        #region Public Methods

        public async Task SaveSettings()
        {
            WriteUpdatesToResource();
        }

        #endregion

        public SettingsViewModel()
        {
            // Create a ResXResourceReader for the file items.resx.
            ResXResourceReader rsxr =
                new ResXResourceReader(GetPathToConfigFile(ConnectionDetailsFile));

            Dictionary<string, string> valuesSet = new Dictionary<string, string>();

            // Iterate through the resources and display the contents to the console.
            foreach (DictionaryEntry d in rsxr)
            {
                Console.WriteLine(d.Key.ToString() + ":\t" + d.Value.ToString());
                valuesSet.Add(d.Key.ToString(), d.Value.ToString());
            }

            //Close the reader.
            rsxr.Close();

            SetFromValues(valuesSet);
        }
    }
}
