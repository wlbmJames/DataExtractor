using System;
using System.Windows;
using ATAPY.Document.Data.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAnalyzer_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var rect = new Rect(13, 23, 100, 200);
        }

        private Document GetTestDocument()
        {
            var result = new Document();
            throw new NotImplementedException();
        }
    }
}
