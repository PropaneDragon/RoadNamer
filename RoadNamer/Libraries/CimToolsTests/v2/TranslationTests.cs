using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CimTools.v2.Utilities;
using System.Reflection;
using CimTools.v2;
using CimTools.v2.Attributes;
using System.IO;

namespace CimToolsTests.v2
{
    public static class ExportTranslations
    {
        [Translatable("translateTextA")]
        public static string TranslationA;

        [Translatable("translateTextB")]
        public static string TranslationB;
    }

    public class TranslationTester : Translation
    {
        public TranslationTester(CimToolBase toolBase) : base(toolBase, false)
        {
        }

        public Language LoadFromText(TextReader reader)
        {
            return DeserialiseLanguage(reader);
        }

        public bool ManuallyLoadLanguage(Language language)
        {
            if (language != null)
            {
                _languages.Add(language);

                return true;
            }

            return false;
        }
    }

    [TestClass]
    public class v2TranslationTests
    {
        [TestMethod]
        public void SaveLanguageFile()
        {
            CimToolBase toolBase = new CimToolBase(new CimToolSettings("", "SaveLanguageFileTest", modAssembly: Assembly.GetExecutingAssembly()));
            TranslationTester translation = new TranslationTester(toolBase);

            translation.GenerateLanguageTemplate();
        }

        [TestMethod]
        public void InterpretLanguageFile()
        {
            CimToolBase toolBase = new CimToolBase(new CimToolSettings("", "InterpretLanguageFileTest", modAssembly: Assembly.GetExecutingAssembly()));
            TranslationTester translation = new TranslationTester(toolBase);

            string xmlText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<Language xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" UniqueName=\"export\" ReadableName=\"Exported Language\">\n" +
                "<Translations>\n" +
                    "<Translation ID=\"translateTextA\" String=\"\" />\n" +
                    "<Translation ID=\"translateTextB\" String=\"\" />\n" +
                "</Translations>\n" +
            "</Language>\n";

            Language loadedLanguage = translation.LoadFromText(new StringReader(xmlText));

            Assert.IsNotNull(loadedLanguage);
        }

        [TestMethod]
        public void TranslateToLanguage()
        {
            CimToolBase toolBase = new CimToolBase(new CimToolSettings("", "TranslateToLanguageTest", modAssembly: Assembly.GetExecutingAssembly()));
            TranslationTester translation = new TranslationTester(toolBase);

            ExportTranslations.TranslationA = "";
            ExportTranslations.TranslationB = "";

            Assert.AreEqual("", ExportTranslations.TranslationA);
            Assert.AreEqual("", ExportTranslations.TranslationB);

            string xmlText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<Language xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" UniqueName=\"export\" ReadableName=\"Exported Language\">\n" +
                "<Translations>\n" +
                    "<Translation ID=\"translateTextA\" String=\"ChangedTextA\" />\n" +
                    "<Translation ID=\"translateTextB\" String=\"Changed Text B\"/>\n" +
                "</Translations>\n" +
            "</Language>\n";

            Language loadedLanguage = translation.LoadFromText(new StringReader(xmlText));

            Assert.IsNotNull(loadedLanguage);
            Assert.IsTrue(translation.ManuallyLoadLanguage(loadedLanguage));
            Assert.IsTrue(translation.TranslateTo("export"), "Translate to imported language");

            Assert.AreEqual("ChangedTextA", ExportTranslations.TranslationA);
            Assert.AreEqual("Changed Text B", ExportTranslations.TranslationB);
        }
    }
}
