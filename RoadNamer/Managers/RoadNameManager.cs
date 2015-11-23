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

        /// <summary>
        /// Dictionary of the segmentId to the road name container
        /// </summary>
        public Dictionary<ushort, RoadContainer> m_roadDict = new Dictionary<ushort, RoadContainer>();
        /// <summary>
        /// Hashset of names already used( for random name generator)
        /// </summary> 
        public HashSet<string> m_usedNames = new HashSet<string>();

        public static RoadNameManager Instance()
        {
            if (instance == null)
            {
                instance = new RoadNameManager();
            }

            return instance;
        }

        public void SetRoadName(ushort segmentId, string newName, string oldName=null)
        {
			RoadContainer container = new RoadContainer( segmentId, newName );
            m_roadDict[segmentId] = container;
            if(oldName != null)
            {
                m_usedNames.Remove(StringUtilities.RemoveTags(oldName));
            }
            m_usedNames.Add(StringUtilities.RemoveTags(newName));
            EventBusManager.Instance().Publish("forceupdateroadnames", null);
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
                    m_roadDict[road.m_segmentId] = road;
                    m_usedNames.Add(StringUtilities.RemoveTags(road.m_roadName));
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
