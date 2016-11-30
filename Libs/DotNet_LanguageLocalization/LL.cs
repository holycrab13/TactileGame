using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Xml.Schema;

namespace tud.mci.LanguageLocalization
{
    /// <summary>
    /// A class for interpreting localization definitions.
    /// A localization file has to be of the following xml structure:
    /// 
    /// &lt;?xml version="1.0" encoding="utf-16" standalone="yes" ?&gt;
    /// &lt;localization&gt;
    /// 
    /// 	&lt;locale language="default"&gt;
    /// 		&lt;trans id="no_value"&gt;no translation value for key {0}.&lt;/trans&gt;
    /// 	&lt;/locale>
    /// 
    /// 	&lt;locale language="de"&gt;
    /// 		&lt;trans id="no_value"&gt;kein Übersetzungswert für den Schlüssel {0} vorhanden.&lt;/trans&gt;
    /// 	&lt;/locale&gt;
    /// 	
    /// &lt;/localization&gt;
    /// </summary>
    public class LL
    {
        #region Members
        /// <summary>
        /// Dictionary for the language definitions
        /// 
        /// Dictionary[ location key,
        ///     Dictionary[ key, localization string]
        /// ]
        /// 
        /// </summary>
        Dictionary<String, Dictionary<String, String>> definitions = new Dictionary<String, Dictionary<String, String>>();

        const string LANGUAGE_DEFAULT = "default";

        static System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LL"/> class.
        /// You have to set a localization definition file <see cref="LL.LoadFromFile"/>.
        /// </summary>
        public LL()
        {
            try
            {
                setDefaultCulture();
                definitions = BuildLanguageDefinitionsFromXML(tud.mci.LanguageLocalization.Properties.Resources.Language);
            }
            catch { }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LL"/> class.
        /// </summary>
        /// <param name="laguageDefinitionXML">The localization definition XML string e.g. from the Proprties.Recources.XXXX</param>
        public LL(string laguageDefinitionXML)
            : this()
        {
            try
            {
                definitions = CombineDictionaries(definitions, BuildLanguageDefinitionsFromXML(laguageDefinitionXML));
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Culture Setting

        /// <summary>
        /// Sets the default culture.
        /// Checks if in the [App.exe].config is the specific 'appSettings' key 'DefaultCulture' was set.
        /// 
        /// in the [App.exe].config file something like this has to be entered if you want to override 
        /// to use the default system language setting.
        /// 
        /// &lt;?xml version ="1.0"?&gt;
        /// &lt;configuration&gt;
        ///  	[...]
        ///  	&lt;appSettings&gt;
        /// 		&lt;add key="DefaultCulture" value="en-US" /&gt;
        /// 	&lt;/appSettings&gt;
        /// 	[...]
        /// &lt;/configuration&gt;
        /// </summary>
        void setDefaultCulture()
        {
            try
            {
                string cultureName = String.Empty;
                String appConfigCulture = ConfigurationManager.AppSettings["DefaultCulture"];
                if (!String.IsNullOrWhiteSpace(appConfigCulture))
                {
                    cultureName = appConfigCulture.ToString();
                }

                if (!String.IsNullOrWhiteSpace(cultureName))
                {
                    CultureInfo _culture = new CultureInfo(cultureName);
                    if (_culture != null)
                    {
                        culture = _culture;
                    }
                }
            }
            catch { }
            finally
            {
                if (culture == null)
                {
                    culture = System.Globalization.CultureInfo.CurrentCulture;
                }
            }
        }

        #endregion

        #region Dictionary Functions

        /// <summary>
        /// Loads the localization definitions form a file
        /// </summary>
        /// <param name="path">the file path of the localization definition file</param>
        /// <returns><c>true</c>if the file could been loaded, otherwise <c>false</c>.</returns>
        public bool LoadFromFile(String path)
        {
            if (File.Exists(path))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);
                    definitions = BuildLanguageDefinitionsFromXML(doc);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears the whole internal localization definitions. 
        /// </summary>
        public void ResetDefinition()
        {
            definitions = new Dictionary<String, Dictionary<String, String>>();
        }

        /// <summary>
        /// Set the default culture to use for localization if it is different from the current system culture. 
        /// </summary>
        /// <param name="_cunture">The culture to use for localization instead of the system culture.</param>
        public void SetStandardCulture(System.Globalization.CultureInfo _cunture)
        {
            culture = _cunture;
        }

        #endregion

        #region Localization File Interpretation

        /// <summary>
        /// Builds the language definitions from an XML document.
        /// </summary>
        /// <param name="doc">The XML document node.</param>
        /// <returns>
        /// Dictionary for the language definitions
        /// 
        /// Dictionary[ location key,
        ///     Dictionary[ key, localization string]
        /// ]
        /// </returns>
        private static Dictionary<String, Dictionary<String, String>> BuildLanguageDefinitionsFromXML(string xmlString)
        {
            if (!String.IsNullOrWhiteSpace(xmlString))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);

                // validation
                try
                {
                    doc.Schemas.Add(getValidtaionSchema());
                    doc.Schemas.Compile();
                    doc.Validate(DTDValidation);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error in localization definition XML file validation file load:\r\n" + ex);
                }

                return BuildLanguageDefinitionsFromXML(doc);
            }

            return new Dictionary<String, Dictionary<String, String>>();
        }

        static XmlSchema _mySchema;
        static XmlSchema getValidtaionSchema()
        {

            if (_mySchema == null)
            {
                System.Text.Encoding encode = System.Text.Encoding.UTF8;
                MemoryStream ms = new MemoryStream(encode.GetBytes(Properties.Resources.localization));
                _mySchema = XmlSchema.Read(ms, DTDValidation);
            }
            return _mySchema;
        }


        static System.Xml.Schema.ValidationEventHandler DTDValidation = new System.Xml.Schema.ValidationEventHandler(booksSettingsValidationEventHandler);
        static void booksSettingsValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            if (e.Severity == System.Xml.Schema.XmlSeverityType.Warning)
            {
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
            }
            else if (e.Severity == System.Xml.Schema.XmlSeverityType.Error)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
            }
        }


        /// <summary>
        /// Builds the language definitions from an XML document.
        /// </summary>
        /// <param name="doc">The XML document node.</param>
        /// <returns>
        /// Dictionary for the language definitions
        /// 
        /// Dictionary[ location key,
        ///     Dictionary[ key, localization string]
        /// ]
        /// </returns>
        private static Dictionary<String, Dictionary<String, String>> BuildLanguageDefinitionsFromXML(XmlDocument doc)
        {
            Dictionary<String, Dictionary<String, String>> definitions = new Dictionary<String, Dictionary<String, String>>();

            if (doc != null && doc.HasChildNodes && doc.DocumentElement != null)
            {
                foreach (XmlNode node in doc.DocumentElement)
                {
                    if (node != null && node.Name.Equals("locale"))
                    {
                        var languageNode = node.Attributes["language"];
                        string lang = (languageNode == null || String.IsNullOrWhiteSpace(languageNode.Value)) ? LANGUAGE_DEFAULT : languageNode.Value.ToLower();
                        var localeDef = buildLacalizationMapping(node);

                        if (definitions.ContainsKey(lang))
                        {
                            definitions[lang] = CombineDictionaries(definitions[lang], localeDef);
                        }
                        else
                        {
                            definitions.Add(lang, localeDef);
                        }
                    }
                }
            }

            return definitions;
        }

        /// <summary>
        /// Interpret a XML node that defines on locale (language) definition set.
        /// </summary>
        /// <param name="node">the 'locale' node for one translation set</param>
        /// <returns>an interpreted set of key value pairs for a translation key and the 
        /// corresponding translated text value.</returns>
        private static Dictionary<String, String> buildLacalizationMapping(XmlNode node)
        {
            Dictionary<String, String> definitions = new Dictionary<String, String>();

            if (node != null && node.HasChildNodes)
            {
                foreach (XmlNode trans in node)
                {
                    if (trans != null && trans.Name.Equals("trans"))
                    {
                        var idNode = trans.Attributes["id"];
                        if (idNode != null && !String.IsNullOrWhiteSpace(idNode.Value))
                        {
                            string id = trans.Attributes["id"].Value;
                            string value = trans.InnerText;

                            if (definitions.ContainsKey(id))
                            {
                                definitions[id] = value;
                            }
                            else
                            {
                                definitions.Add(id, value);
                            }
                        }
                    }
                }
            }

            return definitions;
        }

        /// <summary>
        /// Combines two dictionaries
        /// </summary>
        /// <param name="first">the first (and basic) dictionary. Is the one that will be updated (overwritten) by the second.</param>
        /// <param name="second">the second dictionary, extend or update the first dictionary</param>
        /// <returns>The first dictionary extended or updated by the second dictionary</returns>
        public static Dictionary<String, String> CombineDictionaries(Dictionary<String, String> first, Dictionary<String, String> second)
        {
            Dictionary<String, String> dict = first != null ? first :
                second != null ? second : new Dictionary<String, String>();

            if (first != null && second != null)
            {
                foreach (var item in second)
                {
                    if (dict.ContainsKey(item.Key))
                    {
                        dict[item.Key] = item.Value;
                    }
                    else
                    {
                        dict.Add(item.Key, item.Value);
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Combines two dictionaries
        /// </summary>
        /// <param name="first">the first (and basic) dictionary. Is the one that will be updated (overwritten) by the second.</param>
        /// <param name="second">the second dictionary, extend or update the first dictionary</param>
        /// <returns>The first dictionary extended or updated by the second dictionary</returns>
        public static Dictionary<String, Dictionary<String, String>> CombineDictionaries(Dictionary<String, Dictionary<String, String>> first, Dictionary<String, Dictionary<String, String>> second)
        {
            Dictionary<String, Dictionary<String, String>> dict = first != null ? first :
                second != null ? second : new Dictionary<String, Dictionary<String, String>>();

            if (first != null && second != null)
            {
                foreach (var locale in second)
                {
                    if (dict.ContainsKey(locale.Key))
                    {
                        dict[locale.Key] = CombineDictionaries(dict[locale.Key], locale.Value);
                    }
                    else
                    {
                        dict.Add(locale.Key, locale.Value);
                    }
                }
            }

            return dict;
        }

        #endregion

        #region Translation

        #region DEFAULT

        /// <summary>
        /// get the default language definition for the requested 
        /// key if defined. Without any interpretation.
        /// </summary>
        /// <param name="key">the translation entry id</param>
        /// <returns>the 'default' locale translation definition for the requested id or the empty string.</returns>
        public String GetDefaultTrans(String key)
        {
            if (definitions != null && definitions.ContainsKey(LANGUAGE_DEFAULT))
            {
                var langDev = definitions[LANGUAGE_DEFAULT];
                if (langDev.ContainsKey(key))
                {
                    return langDev[key];
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// get the default language definition for the requested
        /// key if defined with filled in placeholders if available.
        /// </summary>
        /// <param name="key">the translation entry id</param>
        /// <param name="strs">The strings to fill the placeholders (e.g. '{0}').</param>
        /// <returns>
        /// the 'default' locale translation definition for the requested id or the empty string.
        /// </returns>
        public String GetDefaultTrans(String key, params string[] strs)
        {
            String val = GetDefaultTrans(key);
            val = String.Format(val, strs);
            return val;
        }

        #endregion

        /// <summary>
        /// get the language definition for the requested 
        /// key if defined. Without any interpretation.
        /// </summary>
        /// <param name="key">the translation entry id</param>
        /// <returns>the localized (system based) translation definition for the requested id or 
        /// the default translation if no localized version exist or the empty string.</returns>
        public String GetTrans(String key)
        {
            return GetTrans(key, culture);
        }

        /// <summary>
        /// get the language definition for the requested 
        /// key if defined. Without any interpretation.
        /// </summary>
        /// <param name="key">the translation entry id</param>
        /// <returns>the localized (system based) translation definition for the requested id or 
        /// the default translation if no localized version exist or the empty string.</returns>
        public String GetTrans(String key, System.Globalization.CultureInfo _culture)
        {
            if (_culture == null) _culture = culture;
            string locale = LANGUAGE_DEFAULT;

            if (_culture != null) { locale = _culture.TwoLetterISOLanguageName.ToLower(); }

            locale = locale.ToLower();
            if (definitions != null && definitions.ContainsKey(locale))
            {
                var langDev = definitions[locale];
                if (langDev.ContainsKey(key))
                {
                    return langDev[key];
                }
            }

            return GetDefaultTrans(key);
        }

        /// <summary>
        /// get the language definition for the requested
        /// key if defined with filled in placeholders if available.
        /// </summary>
        /// <param name="key">the translation entry id</param>
        /// <param name="_culture">The locale.</param>
        /// <param name="strs">The strings to fill the placeholders (e.g. '{0}').</param>
        /// <returns>
        /// the localized translation definition for the requested id or
        /// the default translation if no localized version exist or the empty string.
        /// </returns>
        /// <exception cref="System.ArgumentException">Some arguments to fill are missing - strs</exception>
        public String GetTrans(String key, System.Globalization.CultureInfo _culture, params string[] strs)
        {
            String val = GetTrans(key, _culture);
            try
            {
                val = String.Format(val, strs);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Some arguments to fill are missing", "strs", ex);
            }
            return val;
        }

        /// <summary>
        /// get the language definition for the requested
        /// key if defined with filled in placeholders if available.
        /// </summary>
        /// <param name="key">the translation entry id</param>
        /// <param name="_culture">The locale.</param>
        /// <param name="strs">The strings to fill the placeholders (e.g. '{0}').</param>
        /// <returns>
        /// the localized translation definition for the requested id or
        /// the default translation if no localized version exist or the empty string.
        /// </returns>
        public String GetTrans(String key, System.Globalization.CultureInfo _culture, params object[] strs)
        {
            string[] str_ = new string[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] != null) str_[i] = strs[i].ToString();
            }

            return GetTrans(key, culture, str_);
        }

        /// <summary>
        /// get the language definition for the requested
        /// key if defined with filled in placeholders if available.
        /// </summary>
        /// <param name="key">the translation entry id</param>
        /// <param name="strs">The strings to fill the placeholders (e.g. '{0}').</param>
        /// <returns>
        /// the localized translation definition for the requested id or
        /// the default translation if no localized version exist or the empty string.
        /// </returns>
        public String GetTrans(String key, params string[] strs)
        {
            return GetTrans(key, culture, strs);
        }

        /// <summary>
        /// get the language definition for the requested
        /// key if defined with filled in placeholders if available.
        /// </summary>
        /// <param name="key">the translation entry id</param>
        /// <param name="strs">The strings to fill the placeholders (e.g. '{0}').</param>
        /// <returns>
        /// the localized translation definition for the requested id or
        /// the default translation if no localized version exist or the empty string.
        /// </returns>
        public String GetTrans(String key, params object[] strs)
        {
            return GetTrans(key, culture, strs);
        }

        #endregion
    }
}
