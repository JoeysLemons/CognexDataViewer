using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CognexDataViewer.Helpers
{
    class SvgReader
    {
        private XmlDocument svgDocument;

        public SvgReader()
        {
            svgDocument = new XmlDocument();
        }

        public void SaveSVG(string filePath)
        {
            try
            {
                svgDocument.Save(filePath);
            }
            catch(Exception ex)
            {
                
            }
        }

        public void LoadSvg(string filePath)
        {
            try
            {
                svgDocument.Load(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading SVG file: " + ex.Message);
            }
        }

        public List<string> GetAllUniqueTags()
        {
            List<string> uniqueTags = new List<string>();

            if (svgDocument != null)
            {
                var allElements = svgDocument.GetElementsByTagName("*");
                foreach (XmlNode element in allElements)
                {
                    if (!uniqueTags.Contains(element.Name))
                    {
                        uniqueTags.Add(element.Name);
                    }
                }
            }

            return uniqueTags;
        }

        public string GetAttributeValueOfTag(string tagName, string attributeName)
        {
            if (svgDocument != null)
            {
                XmlNodeList elements = svgDocument.GetElementsByTagName(tagName);
                foreach (XmlNode element in elements)
                {
                    if (element.Attributes != null)
                    {
                        XmlAttribute attr = element.Attributes[attributeName];
                        if (attr != null)
                        {
                            return attr.Value;
                        }
                    }
                }
            }

            return null;
        }

        public void AddAttributeToTag(string tagName, string attributeName, string attributeValue)
        {
            if (svgDocument != null)
            {
                XmlNodeList elements = svgDocument.GetElementsByTagName(tagName);
                foreach (XmlNode element in elements)
                {
                    // If the attribute already exists, update its value
                    if (element.Attributes[attributeName] != null)
                    {
                        element.Attributes[attributeName].Value = attributeValue;
                    }
                    else // If the attribute does not exist, create and add it
                    {
                        XmlAttribute newAttr = svgDocument.CreateAttribute(attributeName);
                        newAttr.Value = attributeValue;
                        element.Attributes.Append(newAttr);
                    }
                }
            }
        }


    }
}
