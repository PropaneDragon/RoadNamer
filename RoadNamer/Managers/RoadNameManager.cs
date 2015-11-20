using ColossalFramework;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer.Managers
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "RoadNames", Namespace = "", IsNullable = false)]
    public class RoadNameManager
    {
        private static RoadNameManager instance = null;
        public List<RoadContainer> m_roadList = new List<RoadContainer>();

        [XmlArray("Roads")]
        [XmlArrayItem("Road", typeof(RoadContainer))]
        private RoadContainer[] m_roads = null;        

        public static RoadNameManager Instance()
        {
            if (instance == null)
            {
                instance = new RoadNameManager();
            }

            return instance;
        }

        public void SetRoadName(ushort segmentId, string name)
        {
            bool foundRoad = false;

            foreach (RoadContainer road in m_roadList)
            {
                if (road.m_segmentId == segmentId)
                {
                    road.m_roadName = name;
                    foundRoad = true;
                }
            }

            if(!foundRoad)
            {
                RoadContainer newRoad = new RoadContainer();
                newRoad.m_roadName = name;
                newRoad.m_segmentId = segmentId;

                m_roadList.Add(newRoad);
            }

            Save();
        }

        public string GetRoadName(ushort segmentId)
        {
            string returnString = null;

            foreach (RoadContainer road in m_roadList)
            {
                if (road.m_segmentId == segmentId)
                {
                    returnString = road.m_roadName;
                }
            }

            return returnString;
        }

        public bool RoadExists(ushort segmentId)
        {
            foreach(RoadContainer road in m_roadList)
            {
                if(road.m_segmentId == segmentId)
                {
                    return true;
                }
            }

            return false;
        }

        public void Save()
        {
            m_roads = m_roadList.ToArray();

            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(RoadNameManager));
            StreamWriter writer = new StreamWriter("RoadNameTest.xml");

            xmlSerialiser.Serialize(writer, this);
            writer.Close();
        }

        public static void Load()
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(RoadNameManager));
            StreamReader reader = new StreamReader("RoadNameTest.xml");
            
            RoadNameManager newManager = xmlSerialiser.Deserialize(reader) as RoadNameManager;
            reader.Close();

            if (newManager != null)
            {
                newManager.Initialise();
                instance = newManager;
            }
            else
            {
                Debug.LogError("Failed to load the roads from XML");
            }
        }

        public void Initialise()
        {
            if (m_roads != null)
            {
                foreach (RoadContainer road in m_roads)
                {
                    m_roadList.Add(road);
                }
            }
            else
            {
                Debug.LogError("Something went wrong loading the road names!");
            }
        }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class RoadContainer
    {
        public string m_roadName = null;
        public ushort m_segmentId = 0;
    }
}
