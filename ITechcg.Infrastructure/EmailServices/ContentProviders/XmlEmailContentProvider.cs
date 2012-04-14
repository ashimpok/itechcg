using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Xml.XPath;
using ITechcg.Infrastructure.EmailServices.Exceptions;
using ITechcg.Infrastructure.EmailServices.ServiceModel;

namespace ITechcg.Infrastructure.EmailServices.ContentProviders
{
    /// <summary>
    /// Class implements an email information provider for XML based storage. This class reads an XML file in a specific format
    /// and provides email content.
    /// </summary>
    /// <example>
    /// A sample XML fragment:
    /// <code>
    ///     <?xml version="1.0" encoding="windows-1250"?>
    ///     <email>
    ///         <subject>Your Materials</subject>
    ///         <sender-email>apokharel@aifs.com</sender-email>
    ///         <html-view>
    ///	            <linked-resources>
    ///		            <resource cid="logo" path="LinkedResources/logo.bmp"/>
    ///		            <resource cid="footer" path="LinkedResources/footer.jpg"/>
    ///	            </linked-resources>
    ///	            <email-body>
    ///	                Actual html code here with inline styling to be compatible with most email clients
    ///                 __FIRST_NAME__ - Placeholders
    ///	            </email-body>
    ///         </html-view>
    ///         <text-view>
    ///             Text if email client does not support html emails. __FIRST_NAME__ - placeholders can be placed here as well.
    ///         </text-view>
    ///     </email>
    /// </code>
    /// </example>
    public sealed class XmlEmailContentProvider : IEmailContentProvider
    {
        string templateFolder;

        /// <summary>
        /// Initializes XmlEmailContentProvider
        /// </summary>
        /// <param name="TemplateFolder">Full path to the Template Folder</param>
        public XmlEmailContentProvider(string TemplateFolder)
        {
            if (!Path.IsPathRooted(TemplateFolder))
                throw new EmailServiceException("Full path to the Template Folder must be provided.");

            if (!File.Exists(TemplateFolder))
            {
                FileNotFoundException fnfe = new FileNotFoundException("Template folder not found", TemplateFolder);
                throw new EmailServiceException(fnfe, "Template folder specified does not exist.");
            }

            templateFolder = TemplateFolder;
        }

        EmailContent content;
        public EmailContent Content
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Initializes this content provider
        /// </summary>
        /// <param name="data">Xml file. If full path is not provided, file is expected in templateFolder.</param>
        public void Initialize(string data)
        {
            content = new EmailContent();

            string xmlFullPath = data;

            if (!Path.IsPathRooted(xmlFullPath))
            {
                xmlFullPath = Path.Combine(templateFolder, data);
            }

            if (!File.Exists(xmlFullPath))
            {
                throw new EmailServiceException("Specified file {0} does not exist", xmlFullPath);
            }

            //Read the sbr using XmlReader
            StringReader strReader = new StringReader(File.ReadAllText(xmlFullPath));
            XPathDocument xpathDoc = new XPathDocument(strReader);

            XPathNavigator navigator = xpathDoc.CreateNavigator();

            XPathNavigator subjectNav = navigator.SelectSingleNode("/email/subject");
            content.Subject = subjectNav.InnerXml;

            XPathNavigator senderNav = navigator.SelectSingleNode("/email/sender-email");
            if (senderNav != null)
                content.SenderEmail = senderNav.InnerXml;

            XPathNavigator textViewNavigator = navigator.SelectSingleNode("/email/text-view");
            if (textViewNavigator != null)
            {
                content.PlainTextTemplateBody = textViewNavigator.InnerXml;
            }

            //Read htmlView            
            XPathNavigator htmlViewNavigator = navigator.SelectSingleNode("/email/html-view");
            if (htmlViewNavigator != null)
            {
                XPathNavigator emailBodyNavigator = htmlViewNavigator.SelectSingleNode("email-body");
                if (emailBodyNavigator != null)
                {
                    content.HTMLTemplateBody = emailBodyNavigator.InnerXml;

                    //get resources
                    XPathNodeIterator resourcesIterator = htmlViewNavigator.Select("linked-resources/resource");
                    while (resourcesIterator.MoveNext())
                    {
                        XPathNavigator resourceNavigator = resourcesIterator.Current;
                        string cid = resourceNavigator.GetAttribute("cid", string.Empty);
                        string cidPath = resourceNavigator.GetAttribute("path", string.Empty);

                        if (!string.IsNullOrWhiteSpace(cidPath) && !string.IsNullOrWhiteSpace(cid))
                        {
                            if (!Path.IsPathRooted(cidPath))
                            {
                                cidPath = Path.Combine(templateFolder, cidPath);
                            }

                            if (File.Exists(cidPath))
                            {
                                LinkedResource lnkResource = new LinkedResource(cidPath, "image/*");

                                lnkResource.ContentId = cid;
                                content.AddLinkedResource(lnkResource);
                            }
                        }
                    }
                }
            }
        }
    }
}
