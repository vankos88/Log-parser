using LogParser.Managers;
using LogParser.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace LogParserTests
{
    [TestClass]
    public class FIndFIlesTests
    {
        [TestMethod]
        public void TestEmptyPaths()
        {
            var manager = new LogParserManager();
            var model = new LogParserModel();
            manager.FindFiles(model).GetAwaiter().GetResult();
            Assert.AreEqual(1, model.ResultDisplay.Count);
            Assert.AreEqual("Empty paths", model.ResultDisplay[0]);
        }

        [TestMethod]
        public void AssertFilesCount()
        {
            var manager = new LogParserManager();
            var model = new LogParserModel();
            model.IncludeFileInfo = false;
            var dir = Directory.GetCurrentDirectory();
            model.Paths = $"{dir}\\Data";
            manager.FindFiles(model).GetAwaiter().GetResult();
            Assert.AreEqual(4, model.ResultDisplay.Count);
            Assert.AreEqual($"{model.Paths}\\example20190101.log", model.ResultDisplay[1]);
            Assert.AreEqual($"{model.Paths}\\example20200101.log", model.ResultDisplay[2]);
        }
    }
}
