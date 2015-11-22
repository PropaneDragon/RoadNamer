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
        
        //Dictionary of the segmentId to the road name container
        public Dictionary<ushort, RoadContainer> m_roadDict = new Dictionary<ushort, RoadContainer>();


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
			RoadContainer container = new RoadContainer( segmentId, name );
            m_roadDict[segmentId] = container;
        }

        public string GetRoadName(ushort segmentId)
        {
            return m_roadDict.ContainsKey(segmentId) ? m_roadDict[segmentId].m_roadName : null;
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
            if (roadNames != null)
            {
                foreach (RoadContainer road in roadNames)
                {
                    m_roadDict[road.m_segmentId] = road;
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

        public RoadContainer( ushort segmentId, string roadName )
        {
            m_roadName = roadName;
            m_segmentId = segmentId;
        }
    }
}
