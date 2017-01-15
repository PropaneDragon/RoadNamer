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
        /// Dictioanry of names/routes already used, to the number of segments that use that name/route
        /// </summary> 
        public Dictionary<string, int> m_usedNames = new Dictionary<string, int>();
        public Dictionary<string, int> m_usedRoutes = new Dictionary<string, int>();


        public static RoadNameManager Instance()
        {
            if (instance == null)
            {
                instance = new RoadNameManager();
            }

            return instance;
        }

        private void DecrementRoadNameCounter(string name)
        {
            string strippedName = StringUtilities.RemoveTags(name);
            if (m_usedNames.ContainsKey(strippedName))
            {
                m_usedNames[strippedName] -= 1;
                if (m_usedNames[strippedName] <= 0)
                {
                    m_usedNames.Remove(strippedName);
                }

            }
        }

        private void DecrementRoadRouteCounter(string routeStr)
        {
            if (m_usedRoutes.ContainsKey(routeStr))
            {
                m_usedRoutes[routeStr] -= 1;
                if (m_usedRoutes[routeStr] <= 0)
                {
                    m_usedRoutes.Remove(routeStr);
                }

            }
        }

        public void DelRoadName(ushort segmentId)
        {
            if(m_roadDict.ContainsKey(segmentId))
            {
                string roadName = m_roadDict[segmentId].m_roadName;
                m_roadDict.Remove(segmentId);
                DecrementRoadNameCounter(roadName);
            }
        }

        public void SetRoadName(ushort segmentId, string newName, string oldName = null)
        {
            RoadContainer roadContainer = null;
            if( newName.Length > 0)
            {
                if (m_roadDict.ContainsKey(segmentId))
                {
                    roadContainer = m_roadDict[segmentId];
                    roadContainer.m_roadName = newName;
                }
                else
                {
                    roadContainer = new RoadContainer(segmentId, newName);
                    if (roadContainer.m_textObject == null)
                    {
                        roadContainer.m_textObject = new GameObject();
                        roadContainer.m_textObject.AddComponent<MeshRenderer>();
                        roadContainer.m_textMesh = roadContainer.m_textObject.AddComponent<TextMesh>();

                    }
                }
                string strippedNewName = StringUtilities.RemoveTags(newName);
                if (!m_usedNames.ContainsKey(strippedNewName))
                {
                    m_usedNames[strippedNewName] = 0;
                }
                m_usedNames[strippedNewName] += 1;
            }
            else
            {
                string strippedName = StringUtilities.RemoveTags(newName);
                if (m_usedNames.ContainsKey(strippedName))
                {
                    m_usedNames[strippedName] -= 1;
                    if (m_usedNames[strippedName] <= 0)
                    {
                        m_usedNames.Remove(strippedName);
                    }

                }
            }
         
            m_roadDict[segmentId] = roadContainer;

            if (oldName != null)
            {
                DecrementRoadNameCounter(oldName);
            }

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

        public RoadContainer[] SaveRoads()
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
                    if (road.m_textObject == null)
                    {
                        road.m_textObject = new GameObject();
                        road.m_textObject.AddComponent<MeshRenderer>();

                        road.m_textMesh = road.m_textObject.AddComponent<TextMesh>();

                    }
                    m_roadDict[road.m_segmentId] = road;
                    string strippedNewName = StringUtilities.RemoveTags(road.m_roadName);
                    if (!m_usedNames.ContainsKey(strippedNewName))
                    {
                        m_usedNames[strippedNewName] = 0;
                    }
                    m_usedNames[strippedNewName] += 1;
          
                }
            }

        }

    }

    [Serializable]
    public class RoadContainer
    {
        public string m_roadName = null;
        public ushort m_segmentId = 0;

        [NonSerialized]
        public GameObject m_textObject;

        [NonSerialized]
        public TextMesh m_textMesh;

        public RoadContainer(ushort segmentId, string roadName)
        {
            this.m_segmentId = segmentId;
            this.m_roadName = roadName;
        }
    }
}
