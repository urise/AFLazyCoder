using System.Xml;

namespace AFDuplicateFinder.Results
{
    public class DuplicateResultUnit
    {
        public string FileName { get; private set; }
        public int LineNumber { get; private set; }

        public DuplicateResultUnit(string fileName, int lineNumber)
        {
            FileName = fileName;
            LineNumber = lineNumber;
        }

        public DuplicateResultUnit(XmlNode node)
        {
            FileName = node.Attributes["filename"].Value;
            LineNumber = int.Parse(node.Attributes["linenumber"].Value);
        }

        #region Xml Methods

        public XmlNode GetXmlNode(XmlDocument xmlDocument)
        {
            var xmlNode = xmlDocument.CreateElement("unit");
            var attribute = xmlDocument.CreateAttribute("filename");
            attribute.Value = FileName;
            xmlNode.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("linenumber");
            attribute.Value = LineNumber.ToString();
            xmlNode.Attributes.Append(attribute);

            return xmlNode;
        }

        #endregion
    }
}
