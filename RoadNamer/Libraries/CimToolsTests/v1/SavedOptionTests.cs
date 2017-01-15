using CimTools.Legacy.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CimToolsTests.Legacy
{
    [TestClass]
    public class v1SavedOptionTests
    {
        [TestMethod]
        public void SetGetLooseValues()
        {
            XmlFileManager fileManager = new XmlFileManager(new CimTools.Legacy.CimToolSettings("TestSaveLoad"));
            fileManager.Data.SetValue("testFloat", 10.4f);
            fileManager.Data.SetValue("testString", "hello");
            fileManager.Data.SetValue("testDouble", 19.2);
            fileManager.Data.SetValue("testInt", 19);

            float floatValue = 0;
            string stringValue = "";
            double doubleValue = 0;
            int intValue = 0;

            bool valid = fileManager.Data.GetValue("testFloat", ref floatValue) == ExportOptionBase.OptionError.NoError;
            Assert.IsTrue(valid, "Retrieving \"testFloat\" option");
            if (valid)
            {
                Assert.AreEqual(10.4f, floatValue);
            }

            valid = fileManager.Data.GetValue("testString", ref stringValue) == ExportOptionBase.OptionError.NoError;
            Assert.IsTrue(valid, "Retrieving \"testString\" option");
            if (valid)
            {
                Assert.AreEqual("hello", stringValue);
            }

            valid = fileManager.Data.GetValue("testDouble", ref doubleValue) == ExportOptionBase.OptionError.NoError;
            Assert.IsTrue(valid, "Retrieving \"testDouble\" option");
            if (valid)
            {
                Assert.AreEqual(19.2, doubleValue);
            }

            valid = fileManager.Data.GetValue("testInt", ref intValue) == ExportOptionBase.OptionError.NoError;
            Assert.IsTrue(valid, "Retrieving \"testInt\" option");
            if (valid)
            {
                Assert.AreEqual(19, intValue);
            }

            valid = fileManager.Data.GetValue("testNotInList", ref floatValue) == ExportOptionBase.OptionError.NoError;
            Assert.IsFalse(valid, "Retrieving \"testNotInList\" option");

            valid = fileManager.Data.GetValue("testInt", ref intValue) == ExportOptionBase.OptionError.NoError;
            Assert.IsTrue(valid, "Retrieving \"testInt\" option");
            if (valid)
            {
                Assert.AreNotEqual("string", intValue);
                Assert.AreNotEqual("19", intValue);
            }
        }

        [TestMethod]
        public void SetGetGroupedValues()
        {
            XmlFileManager fileManager = new XmlFileManager(new CimTools.Legacy.CimToolSettings("TestSaveLoad"));
            fileManager.Data.SetValue("float", 12.3f, "group 1");
            fileManager.Data.SetValue("double", 34.5, "group 1");
            fileManager.Data.SetValue("double 2", 67.8, "group 1");
            fileManager.Data.SetValue("string", "a string", "group 1");
            fileManager.Data.SetValue("string 2", "56.7", "group 1");
            fileManager.Data.SetValue("int", 1, "group 1");

            fileManager.Data.SetValue("float", 98.7f, "group 2");
            fileManager.Data.SetValue("double", 65.4, "group 2");

            var foundFloatValues = fileManager.Data.GetValues<float>("group 1");
            Assert.AreEqual(1, foundFloatValues.Count);
            Assert.IsTrue(foundFloatValues.ContainsKey("float"));
            Assert.AreEqual(12.3f, foundFloatValues["float"]);

            foundFloatValues = fileManager.Data.GetValues<float>("group 1", false);
            Assert.AreEqual(5, foundFloatValues.Count);

            var foundDoubleValues = fileManager.Data.GetValues<double>("group 1");
            Assert.AreEqual(2, foundDoubleValues.Count);
            Assert.IsTrue(foundDoubleValues.ContainsKey("double"));
            Assert.IsTrue(foundDoubleValues.ContainsKey("double 2"));
            Assert.AreEqual(34.5, foundDoubleValues["double"]);
            Assert.AreEqual(67.8, foundDoubleValues["double 2"]);

            var foundStringValues = fileManager.Data.GetValues<string>("group 1");
            Assert.AreEqual(2, foundStringValues.Count);
            Assert.IsTrue(foundStringValues.ContainsKey("string"));
            Assert.IsTrue(foundStringValues.ContainsKey("string 2"));
            Assert.AreEqual("a string", foundStringValues["string"]);
            Assert.AreEqual("56.7", foundStringValues["string 2"]);

            var foundIntValues = fileManager.Data.GetValues<int>("group 1");
            Assert.AreEqual(1, foundIntValues.Count);
            Assert.IsTrue(foundIntValues.ContainsKey("int"));
            Assert.AreEqual(1, foundIntValues["int"]);

            foundFloatValues = fileManager.Data.GetValues<float>("group 2");
            Assert.AreEqual(1, foundFloatValues.Count);
            Assert.IsTrue(foundFloatValues.ContainsKey("float"));
            Assert.AreEqual(98.7f, foundFloatValues["float"]);

            foundDoubleValues = fileManager.Data.GetValues<double>("group 2");
            Assert.AreEqual(1, foundDoubleValues.Count);
            Assert.IsTrue(foundDoubleValues.ContainsKey("double"));
            Assert.AreEqual(65.4, foundDoubleValues["double"]);
        }

        [TestMethod]
        public void SetExistingValues()
        {
            XmlFileManager fileManager = new XmlFileManager(new CimTools.Legacy.CimToolSettings("TestSaveLoad"));
            fileManager.Data.SetValue("int a", 1, "group 1");

            var foundIntValues = fileManager.Data.GetValues<int>("group 1");
            Assert.AreEqual(1, foundIntValues.Count);
            Assert.AreEqual(1, foundIntValues["int a"]);

            fileManager.Data.SetValue("int a", 2, "group 1");

            foundIntValues = fileManager.Data.GetValues<int>("group 1");
            Assert.AreEqual(1, foundIntValues.Count);
            Assert.AreEqual(2, foundIntValues["int a"]);

            fileManager.Data.SetValue("int b", 3, "group 1");

            foundIntValues = fileManager.Data.GetValues<int>("group 1");
            Assert.AreEqual(2, foundIntValues.Count);
            Assert.AreEqual(2, foundIntValues["int a"]);
            Assert.AreEqual(3, foundIntValues["int b"]);

            fileManager.Data.SetValue("int b", 4, "group 1");

            foundIntValues = fileManager.Data.GetValues<int>("group 1");
            Assert.AreEqual(2, foundIntValues.Count);
            Assert.AreEqual(2, foundIntValues["int a"]);
            Assert.AreEqual(4, foundIntValues["int b"]);
        }

        [TestMethod]
        public void SaveLoadValues()
        {
            XmlFileManager fileManager = new XmlFileManager(new CimTools.Legacy.CimToolSettings("TestSaveLoad"));
            
            fileManager.Data.SetValue("testUshort", (ushort)2);
            fileManager.Data.SetValue("testFloat", 10.4f);
            fileManager.Data.SetValue("testString", "hello");
            fileManager.Data.SetValue("testDouble", 19.2);
            fileManager.Data.SetValue("testInt", 19);
            fileManager.Data.SetValue("testInt", 2082, "awesome group");
            fileManager.Data.SetValue("testDouble", 106.8, "awesome group");
            fileManager.Data.SetValue("testString", "hello again", "awesome group");
            fileManager.Data.SetValue("testDifferentString", "hello again again", "awesome group");
            fileManager.Data.SetValue("testDifferentGroup", "hi", "not as awesome group");

            bool succeeded = fileManager.Save() == ExportOptionBase.OptionError.NoError;
            Assert.IsTrue(succeeded, "Options failed to save to disk");

            if (succeeded)
            {
                float floatValue = 0;
                int intValue = 0;
                string stringValue = "";

                fileManager = new XmlFileManager(new CimTools.Legacy.CimToolSettings("TestSaveLoad"));

                bool valid = fileManager.Data.GetValue("testFloat", ref floatValue) == ExportOptionBase.OptionError.NoError;
                Assert.IsFalse(valid);

                fileManager.Load();

                valid = fileManager.Data.GetValue("testFloat", ref floatValue) == ExportOptionBase.OptionError.NoError;
                Assert.IsTrue(valid, "Obtaining \"testFloat\" from the options");
                if (valid)
                {
                    Assert.AreEqual(10.4f, floatValue, "Checking whether the value is a float");
                }

                valid = fileManager.Data.GetValue("testInt", ref intValue, "awesome group") == ExportOptionBase.OptionError.NoError;
                Assert.IsTrue(valid, "Obtaining \"testInt\" from the options under \"awesome group\"");
                if (valid)
                {
                    Assert.AreEqual(2082, intValue, "Checking whether the value is an int");
                }

                valid = fileManager.Data.GetValue("testString", ref stringValue, "awesome group") == ExportOptionBase.OptionError.NoError;
                Assert.IsTrue(valid, "Obtaining \"testString\" from the options under \"awesome group\"");
                if (valid)
                {
                    Assert.AreEqual("hello again", stringValue, "Checking whether the value is a string");
                }
            }
        }

        [TestMethod]
        public void ErrorHandling()
        {
            XmlFileManager fileManager = new XmlFileManager(new CimTools.Legacy.CimToolSettings("DoesntExist"));

            Assert.AreEqual(fileManager.Load(), ExportOptionBase.OptionError.FileNotFound);

            int tempIntStorage = 0;

            Assert.AreEqual(fileManager.Data.GetValue<int>("NotAValidName", ref tempIntStorage, "NotAValidGroup"), ExportOptionBase.OptionError.GroupNotFound);
            Assert.AreEqual(fileManager.Data.GetValue<int>("NotAValidName", ref tempIntStorage), ExportOptionBase.OptionError.OptionNotFound);
        }
    }
}
