using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    public static class XamlNodeExtenstion
    {
        public static string GetName(this string xml)
        {
            int index;
            int lastIndex;
            if (xml.Contains("<"))
            {
                index = xml.IndexOf('<');
                lastIndex = xml.IndexOf(' ', index);
                if (lastIndex < 0)
                    lastIndex = xml.IndexOf("/>", index);
                if (lastIndex < 0)
                    lastIndex = xml.IndexOf(">", index);
                return xml.Substring(index + 1, lastIndex - index - 1);
            }

            return string.Empty;

        }

        public static string GetDisplayName(this string xml)
        {
            string ret = string.Empty;
            if (xml.Contains(" Name="))
            {
                ret = GetDisplayName(xml, " Name=");
            }
            else if (xml.Contains(" DisplayName="))
            {
                ret = GetDisplayName(xml, " DisplayName=");
            }
            else if (xml.Contains(" x:Class="))
            {
                ret = GetDisplayName(xml, " x:Class=");
            }

            return UnescapeXML(ret);
        }

        public static string GetTypeName(this string word)
        {
            var array = word.Split(':');
            var name = array.Last();
            if (name == "Activity")
                name = "ActivityBuilder";
            return name;
        }

        private static string GetDisplayName(string xml, string pattern)
        {
            int index;
            int lastIndex;

            index = xml.IndexOf(pattern) + pattern.Length;
            lastIndex = xml.IndexOf("\"", index + 1);
            return xml.Substring(index + 1, lastIndex - index - 1);
        }

        public static void SetName(this XamlTreeNode node, string xml)
        {
            int index;
            int lastIndex;
            string name;
            if (xml.Contains("<"))
            {
                index = xml.IndexOf('<');
                lastIndex = xml.IndexOf(' ', index);
                if (lastIndex < 0)
                    lastIndex = xml.IndexOf("/>", index);
                if (lastIndex < 0)
                    lastIndex = xml.IndexOf(">", index);
                name = xml.Substring(index + 1, lastIndex - index - 1);
                node.Name = name;
            }

            if (xml.Contains("Name="))
            {
                index = xml.IndexOf("Name=") + "Name=".Length;
                lastIndex = xml.IndexOf("\"", index + 1);
                name = xml.Substring(index + 1, lastIndex - index - 1);
                node.DisplayName = name;
            }
            if (xml.Contains("DisplayName="))
            {
                index = xml.IndexOf("DisplayName=") + "DisplayName=".Length;
                lastIndex = xml.IndexOf("\"", index + 1);
                name = xml.Substring(index + 1, lastIndex - index - 1);
                node.DisplayName = UnescapeXML(name);
            }
            if (xml.Contains("x:Class="))
            {
                index = xml.IndexOf("x:Class=") + "x:Class=".Length;
                lastIndex = xml.IndexOf("\"", index + 1);
                name = xml.Substring(index + 1, lastIndex - index - 1);
                node.DisplayName = name;
            }

            node.Type = GetTypeName(node.Name);
        }

        private static string UnescapeXML(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            string returnString = s;
            returnString = returnString.Replace("&apos;", "'");
            returnString = returnString.Replace("&quot;", "\"");
            returnString = returnString.Replace("&gt;", ">");
            returnString = returnString.Replace("&lt;", "<");
            returnString = returnString.Replace("&amp;", "&");
            return returnString;
        }

        public static string RemoveGenericsName(this string name)
        {
            string result = name;
            int index = name.IndexOf('`');
            if (index > 0)
            {
                result = name.Substring(0, index);
            }
            return result;
        }
    }
}
