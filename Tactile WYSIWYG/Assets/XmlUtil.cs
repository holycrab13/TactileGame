using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XmlUtil
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
            catch (Exception)
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


    internal static string[] GetArray(XmlNode node, string attr, char separator)
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
}
