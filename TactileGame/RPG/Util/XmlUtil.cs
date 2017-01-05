using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TactileGame.RPG.Util
{
    class XmlUtil
    {
        public static int Get(XmlNode node, string attr, int defaultValue)
        {
            XmlAttribute attribute = node.Attributes[attr];

            if (attribute == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Int32.Parse(attribute.Value);
                }
                catch(Exception)
                {
                    return defaultValue;
                }
            }
        }

        public static string Get(XmlNode node, string attr, string defaultValue)
        {
            XmlAttribute attribute = node.Attributes[attr];

            if (attribute == null)
            {
                return defaultValue;
            }
            else
            {
                if (attribute.Value == null)
                {
                    return defaultValue;
                }
                else
                {
                    return attribute.Value;
                }
            }
        }

        public static string[] Get(XmlNode node, string attr, char[] separator)
        {
            XmlAttribute attribute = node.Attributes[attr];

            if (attribute == null)
            {
                return new string[0];
            }
            else
            {
                if (attribute.Value == null)
                {
                    return new string[0];
                }
                else
                {
                    return attribute.Value.Split(separator);
                }
            }
        }

        public static bool Get(XmlNode node, string attr, bool defaultValue)
        {
            XmlAttribute attribute = node.Attributes[attr];

            if (attribute == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Boolean.Parse(attribute.Value);
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }
        }

        public static Direction Get(XmlNode node, string attr, Direction defaultValue)
        {
            XmlAttribute attribute = node.Attributes[attr];

            if (attribute == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return (Direction)Int32.Parse(attribute.Value);
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }
        }
    }
}
