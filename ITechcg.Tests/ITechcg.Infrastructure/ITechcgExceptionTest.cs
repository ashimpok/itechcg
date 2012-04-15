using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ITechcg.Infrastructure.Exceptions;

namespace ITechcg.Tests.Itechcg.Infrastructure
{
    [TestClass]
    public class ITechcgExceptionTest
    {
        [TestMethod]
        public void ITechcgExceptionIsAlwaysApplicationExceptionAddMessagesAreSetProperly()
        {
            ITechcgException ex = new ITechcgException("My message");
            Assert.IsTrue(ex is ApplicationException, "ITechcgException must be derieved from ApplicationException");
            Assert.AreEqual("My message", ex.Message, "My message was expected");
            Assert.IsNull(ex.InnerException, "Inner exception is not expected");

            ex = new ITechcgException("My message is {0} {1}", "pretty simple", 1);
            Assert.AreEqual("My message is pretty simple 1", ex.Message, "ITechcgException  is not setting the message properly with formatstring");
            Assert.IsNull(ex.InnerException, "Inner exception is not expected");

            Exception bex = new Exception();
            ex = new ITechcgException(bex, "My message");
            Assert.AreEqual("My message", ex.Message, "My message was expected");
            Assert.IsNotNull(ex.InnerException, "Inner exception is expected");

            ex = new ITechcgException(bex, "My message is {0} {1}", "pretty simple", 1);
            Assert.AreEqual("My message is pretty simple 1", ex.Message, "ITechcgException  is not setting the message properly with formatstring");
            Assert.IsNotNull(ex.InnerException, "Inner exception is expected");
        }
    }
}
