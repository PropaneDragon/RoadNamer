using CimTools.v2;
using CimTools.v2.Attributes;
using CimTools.v2.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;

namespace CimToolsTests.v2
{
    [XmlOptions(key = "OverriddenStaticName")]
    public static class TestOverriddenNameClass
    {
        public static Dictionary<string, double> testDictionaryDouble = new Dictionary<string, double>() { { "doublekey1", 0.00005 }, { "doublekey2", 0.5555555 } };
        public static List<int> testListInt = new List<int>() { 1, 2, 3, 4, 5, 6 };
        public static int testInt = 15;
        public static string testString = "test string";
    }

    [XmlOptions]
    public static class TestNonOverriddenNameClass
    {
        public static Dictionary<string, string> testDictionaryString = new Dictionary<string, string>() { { "stringkey1", "test dictionary string 1" } };
        public static List<double> testListDouble = new List<double>() { 1.1234, 2.2345, 3.3456, 4.4567, 5.5678, 6.6789 };
        public static int testInt = 1000;
        public static string testString = "another test string";
    }

    [XmlOptions(key = "OverriddenNonStaticName")]
    public class TestNonStaticOverriddenClass
    {
        public Dictionary<string, double> testDictionaryDouble = new Dictionary<string, double>() { { "doublekey2", 0.00005 } };
        public List<int> testListInt = new List<int>() { 10, 20, 30, 40, 50, 60 };
        public int testInt = 150;
        public string testString = "test non static string";
    }

    [XmlOptions]
    public class TestNonStaticNonOverriddenClass
    {
        public Dictionary<string, string> testDictionaryString = new Dictionary<string, string>() { { "stringkey2", "test dictionary string 2" } };
        public List<double> testListDouble = new List<double>() { 10.1234, 20.2345, 30.3456, 40.4567, 50.5678, 60.6789 };
        public int testInt = 50000;
        public string testString = "another another non static test string";
    }

    [TestClass]
    public class v2SavedOptionTests
    {
        [TestMethod]
        public void SaveLoadUsingAttributes()
        {
            XmlFileManager testManager = new XmlFileManager(new CimToolBase(new CimToolSettings("", "SaveLoadUsingAttributesTest", modAssembly: Assembly.GetExecutingAssembly())));

            TestNonStaticOverriddenClass testClassOverridden = new TestNonStaticOverriddenClass();
            TestNonStaticNonOverriddenClass testClassNonOverridden = new TestNonStaticNonOverriddenClass();

            testManager.AddObjectToSave(testClassOverridden);
            testManager.AddObjectToSave(testClassNonOverridden);
            testManager.Save();

            TestOverriddenNameClass.testInt = -100;
            TestOverriddenNameClass.testString = "differentValue";
            TestOverriddenNameClass.testDictionaryDouble = new Dictionary<string, double>() { { "new double", -23.4567 }, { "another double", 98877.52322 } };
            TestOverriddenNameClass.testListInt = new List<int>() { 9, 8, 7, 6 };

            testClassOverridden.testInt = -100;
            testClassOverridden.testString = "differentValue";
            testClassOverridden.testDictionaryDouble = new Dictionary<string, double>() { { "new double", -23.4567 }, { "another double", 98877.52322 } };
            testClassOverridden.testListInt = new List<int>() { 9, 8, 7, 6 };

            Assert.AreEqual(-100, TestOverriddenNameClass.testInt);
            Assert.AreEqual("differentValue", TestOverriddenNameClass.testString);
            Assert.AreEqual(-23.4567, TestOverriddenNameClass.testDictionaryDouble["new double"]);
            Assert.AreEqual(98877.52322, TestOverriddenNameClass.testDictionaryDouble["another double"]);
            Assert.AreEqual(4, TestOverriddenNameClass.testListInt.Count);

            Assert.AreEqual(-100, testClassOverridden.testInt);
            Assert.AreEqual("differentValue", testClassOverridden.testString);
            Assert.AreEqual(-23.4567, testClassOverridden.testDictionaryDouble["new double"]);
            Assert.AreEqual(98877.52322, testClassOverridden.testDictionaryDouble["another double"]);
            Assert.AreEqual(4, testClassOverridden.testListInt.Count);

            testManager.Load();

            Assert.AreEqual(15, TestOverriddenNameClass.testInt);
            Assert.AreEqual("test string", TestOverriddenNameClass.testString);
            Assert.AreEqual(0.00005, TestOverriddenNameClass.testDictionaryDouble["doublekey1"]);
            Assert.AreEqual(2, TestOverriddenNameClass.testDictionaryDouble.Count);
            Assert.AreEqual(6, TestOverriddenNameClass.testListInt.Count);
            Assert.AreEqual(1, TestOverriddenNameClass.testListInt[0]);
            Assert.AreEqual(2, TestOverriddenNameClass.testListInt[1]);
            Assert.AreEqual(3, TestOverriddenNameClass.testListInt[2]);
            Assert.AreEqual(4, TestOverriddenNameClass.testListInt[3]);
            Assert.AreEqual(5, TestOverriddenNameClass.testListInt[4]);
            Assert.AreEqual(6, TestOverriddenNameClass.testListInt[5]);

            Assert.AreEqual(150, testClassOverridden.testInt);
            Assert.AreEqual("test non static string", testClassOverridden.testString);
            Assert.AreEqual(0.00005, testClassOverridden.testDictionaryDouble["doublekey2"]);
            Assert.AreEqual(1, testClassOverridden.testDictionaryDouble.Count);
            Assert.AreEqual(6, testClassOverridden.testListInt.Count);
            Assert.AreEqual(10, testClassOverridden.testListInt[0]);
            Assert.AreEqual(20, testClassOverridden.testListInt[1]);
            Assert.AreEqual(30, testClassOverridden.testListInt[2]);
            Assert.AreEqual(40, testClassOverridden.testListInt[3]);
            Assert.AreEqual(50, testClassOverridden.testListInt[4]);
            Assert.AreEqual(60, testClassOverridden.testListInt[5]);
        }
    }
}
