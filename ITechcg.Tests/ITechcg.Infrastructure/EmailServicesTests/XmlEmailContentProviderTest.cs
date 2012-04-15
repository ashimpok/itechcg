using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITechcg.Infrastructure.EmailServices.ContentProviders;
using ITechcg.Infrastructure.EmailServices.Exceptions;
using System.IO;

namespace ITechcg.Tests.Itechcg.Infrastructure.EmailServicesTests
{
    [TestClass]
    public class XmlEmailContentProviderTest
    {
        [TestMethod]
        [ExpectedException(typeof(EmailServiceException))]
        public void ThrowErrorIfTemplateFolderIsNotRooted()
        {
            string templateFolder = string.Empty;
            IEmailContentProvider emailContentProvider = new XmlEmailContentProvider(templateFolder);            
        }

        [TestMethod]        
        public void ThrowErrorIfTemplateFolderIsNotAValidFolder()
        {
            try
            {
                //Create non-existent folder
                string templateFolder = string.Format(@"c:\{0}", Guid.NewGuid().ToString("N"));
                IEmailContentProvider emailContentProvider = new XmlEmailContentProvider(templateFolder);
            }
            catch (EmailServiceException ese)
            {
                Assert.IsTrue(ese.InnerException is FileNotFoundException,
                    "Inner exception: FileNotFoundException was expected");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected exception {0", ex.GetType());
            }
        }
    }
}
