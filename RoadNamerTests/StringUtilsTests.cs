using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoadNamer.Utilities;
using UnityEngine;

namespace RoadNamerTests
{
    [TestClass]
    public class StringUtilsTests
    {
        [TestMethod]
        public void TestColourExtraction()
        {
            Color extractedColour = StringUtilities.ExtractColourFromTags("<color#ff0000>Red!</color>", new Color(1, 1, 1));
            
            Assert.IsTrue(extractedColour.r - 1.0f < 0.1); //Comparing floats is not fantastic
            Assert.IsTrue(extractedColour.g - 0.0f < 0.1);
            Assert.IsTrue(extractedColour.b - 0.0f < 0.1);

            extractedColour = StringUtilities.ExtractColourFromTags("<color#00ff00>Green!</color>", new Color(1, 1, 1));

            Assert.IsTrue(extractedColour.r - 0.0f < 0.1);
            Assert.IsTrue(extractedColour.g - 1.0f < 0.1);
            Assert.IsTrue(extractedColour.b - 0.0f < 0.1);

            extractedColour = StringUtilities.ExtractColourFromTags("<color#0000ff>Blue!</color>", new Color(1, 1, 1));

            Assert.IsTrue(extractedColour.r - 0.0f < 0.1);
            Assert.IsTrue(extractedColour.g - 0.0f < 0.1);
            Assert.IsTrue(extractedColour.b - 1.0f < 0.1);
        }

        [TestMethod]
        public void TestTagRemoval()
        {
            string resultString = StringUtilities.RemoveTags("<color#ff0000>Red!</color>");
            Assert.AreEqual<string>("Red!", resultString);

            resultString = StringUtilities.RemoveTags("<color#ff0000><<<<--- ARROWS! --->>>></color>");
            Assert.AreEqual<string>("<<<<--- ARROWS! --->>>>", resultString);
        }
    }
}
