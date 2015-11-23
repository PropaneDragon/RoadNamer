using ColossalFramework;
using RoadNamer.Utilities;
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
            return RoadExists(segmentId) ? m_roadDict[segmentId].m_roadName : null;
        }

        public bool RoadExists(ushort segmentId)
        {
            return m_roadDict.ContainsKey(segmentId);
        }

        public RoadContainer[] Save()
        {
            List<RoadContainer> returnList = new List<RoadContainer>(m_roadDict.Values);
            return returnList.ToArray();
        }

        public void Load(RoadContainer[] roadNames)
        {
            if (m_roadDict != null)
            {
                foreach (RoadContainer road in m_roadDict.Values)
                {
                    m_roadDict[road.m_segmentId] = road;
                    m_usedNames.Add(StringUtilities.RemoveTags(road.m_roadName));
                }
            }
            else
            {
                LoggerUtilities.LogError("Something went wrong loading the road names!");
            }
        }
    }

    [Serializable]
    public class RoadContainer
    {
        public string m_roadName = null;
        public ushort m_segmentId = 0;

        public RoadContainer(ushort segmentId, string roadName)
        {
            this.m_segmentId = segmentId;
            this.m_roadName = roadName;
        }
    }
}
