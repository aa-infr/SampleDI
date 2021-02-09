using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Infrabel.ICT.Framework.Extension
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize an object
        /// </summary>
        public static string ToXml(this object adapterState)
        {
            // Represents an XML document,
            var xmlDoc = new XmlDocument();
            var xmlSerializer = new XmlSerializer(adapterState.GetType());

            // Creates a stream whose backing store is memory.
            using (var xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, adapterState);
                xmlStream.Position = 0;
                //Loads the XML document from the specified string.
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }

        /// <summary>
        /// Merge .xslt template & an object
        /// </summary>
        /// <param name="objectInst"></param>
        /// <param name="executingAssembly"></param>
        /// <param name="template">xslt template embedded in the executing assembly</param>
        /// <param name="embedded"></param>
        /// <returns></returns>
        public static string ToHtml(this object objectInst, Assembly executingAssembly, string template, bool embedded = true)
        {
            if (string.IsNullOrEmpty(template)) return null;
            // Represents an XML document,

            var xslt = new XslCompiledTransform();
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
            var xsltDoc = new XmlDocument();

            using (var reader = embedded ? new StreamReader(GetManifestResource(executingAssembly, template)) : File.OpenText(template))
            using (var xmlReader = XmlReader.Create(reader, settings))
            {
                xsltDoc.Load(xmlReader);
            }

            // Use XsltSettings to enable the use of the document() function.
            xslt.Load(xsltDoc, new XsltSettings(true, false), null);

            // Load the first XML file into a document
            var doc = new XmlDocument();
            doc.LoadXml(objectInst.ToXml());

            using (var writer = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(writer))
            {
                // Pass the resolver to the transform
                xslt.Transform(doc, null, xmlWriter);
                return writer.ToString();
            }
        }

        private static Stream GetManifestResource(Assembly executingAssembly, string templateName)
        {
            var assembly = executingAssembly;
            foreach (var resource in assembly.GetManifestResourceNames())
                if (resource.EndsWith(templateName, StringComparison.OrdinalIgnoreCase))
                    return assembly.GetManifestResourceStream(resource);
            return null;
        }
    }
}