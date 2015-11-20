using ColossalFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer.Managers
{
    public class RoadNameManager
    {
        private static RoadNameManager instance = null;

        public List<RoadContainer> m_roadList = new List<RoadContainer>();        
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

            //Save();
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

        public RoadContainer[] Save()
        {
            m_roads = m_roadList.ToArray();

            return m_roads;
        }

        public void Load(RoadContainer[] roadNames)
        {
            if (roadNames != null)
            {
                m_roads = roadNames;
                Initialise();
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

    [Serializable]
    public class RoadContainer
    {
        public string m_roadName = null;
        public ushort m_segmentId = 0;
    }
}
