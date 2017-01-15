using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoadNamer.Utilities;
using System.IO;
using System.Xml.Serialization;

namespace RoadNamerTests
{
    [TestClass]
    public class RandomRoadNameTests
    {
        [TestMethod]
        public void TestRoadNames()
        {
            RandomNameUtility.Instance().m_roadNames = new RandomRoadName[]
            {
                new RandomRoadName
                {
                    Name = "School",
                    NameHasToContain = "",
                    MaximumLanes = 2
                },
                new RandomRoadName
                {
                    Name = "Cherry",
                    NameHasToContain = "",
                    MinimumLanes = 1,
                    MaximumLanes = 2,
                    ForcePostfix = true
                }
            };

            RandomNameUtility.Instance().m_roadPrefixes = new RandomRoadPrefix[]
            {
                new RandomRoadPrefix
                {
                    Name = "Old",
                    NameHasToContain = "",
                    MaximumLanes = 2
                },
                new RandomRoadPrefix
                {
                    Name = "New",
                    NameHasToContain = "",
                    MinimumLanes = 0,
                    MaximumLanes = 2
                }
            };

            RandomNameUtility.Instance().m_roadPostfixes = new RandomRoadPostfix[]
            {
                new RandomRoadPostfix
                {
                    Name = "Street",
                    NameHasToContain = "",
                    MinimumLanes = 0,
                    MaximumLanes = 2
                },
                new RandomRoadPostfix
                {
                    Name = "Lane",
                    NameHasToContain = "",
                    MinimumLanes = 0,
                    MaximumLanes = 2
                }
            };

            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(RandomNameUtility));
            StreamWriter writer = new StreamWriter("test.xml");
            
            xmlSerialiser.Serialize(writer, RandomNameUtility.Instance());
            writer.Close();
        }

        [TestMethod]
        public void TestRoadDeserialise()
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(RandomNameUtility));
            StreamReader reader = new StreamReader("test.xml");

            RandomNameUtility utility = xmlSerialiser.Deserialize(reader) as RandomNameUtility;
            reader.Close();

            Assert.AreEqual<string>(utility.m_roadNames[0].Name, "School");
            Assert.IsTrue(utility.m_roadNames[0].ForcePostfix);
            Assert.IsFalse(utility.m_roadNames[0].ForcePrefix);
            Assert.AreEqual<int>(utility.m_roadNames[0].MinimumLanes, -1);
            Assert.AreEqual<int>(utility.m_roadNames[0].MaximumLanes, 5);
        }
    }
}
