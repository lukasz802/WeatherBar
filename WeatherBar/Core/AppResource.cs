using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace WeatherBar.Core
{
    public class AppResource
    {
        #region Fields

        private readonly Dictionary<string, string> resourceDictionary;

        #endregion

        #region Constructors

        public AppResource(XmlDocument xmlDocument)
        {
            resourceDictionary = PrepareReresourceDictionary(xmlDocument);
        }

        #endregion

        #region Public methods

        public void Use()
        {
            foreach (string key in resourceDictionary.Keys)
            {
                if (Application.Current.Resources.Contains(key))
                {
                    Application.Current.Resources[key] = resourceDictionary[key];
                }
            }
        }

        #endregion

        #region Private methods

        private Dictionary<string, string> PrepareReresourceDictionary(XmlDocument xmlDocument)
        {
            XmlNodeList xmlNodes;

            try
            {
                xmlNodes = xmlDocument.SelectSingleNode("Root")
                                      .SelectNodes("Row");
            }
            catch
            {
                throw new XmlException("Unable to parse the XML document.");
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (XmlNode node in xmlNodes)
            {
                result.Add(node.SelectSingleNode("Key").InnerText, node.SelectSingleNode("Value").InnerText);
            }

            return result;
        }

        #endregion
    }
}
