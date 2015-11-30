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
        public Dictionary<string,int > m_usedNames = new Dictionary<string, int>();
        public Dictionary<ushort, RouteContainer> m_routeMap = new Dictionary<ushort, RouteContainer>();

        public static RoadNameManager Instance()
        {
            if (instance == null)
            {
                instance = new RoadNameManager();
            }

            return instance;
        }

        public void SetRoadName(ushort segmentId, string newName, string oldName = null, string routePrefix = null,)
        {
            RoadContainer container = null;
            if (m_roadDict.ContainsKey(segmentId))
            {
                container = m_roadDict[segmentId];
                container.m_roadName = newName;
            }
            else
                {
                container = new RoadContainer(segmentId, newName);
                if (container.m_textObject == null)
                {
                    container.m_textObject = new GameObject();
                    container.m_textObject.AddComponent<MeshRenderer>();
                    container.m_textMesh = container.m_textObject.AddComponent<TextMesh>();
                    container.m_shieldObject = new GameObject();
                    container.m_shieldObject.AddComponent<MeshRenderer>();
                    container.m_shieldMesh = container.m_shieldObject.AddComponent<MeshFilter>();
                    container.m_numTextObject = new GameObject();
                    container.m_numTextObject.AddComponent<MeshRenderer>();
                    container.m_numMesh = container.m_numTextObject.AddComponent<TextMesh>();
                }
            }
            m_roadDict[segmentId] = container;

            if (oldName != null)
            {
                string strippedOldName = StringUtilities.RemoveTags(oldName);
                if (m_usedNames.ContainsKey(strippedOldName))
                {
                    m_usedNames[strippedOldName] -= 1;
                    if (m_usedNames[strippedOldName] <= 0)
                    {
                        m_usedNames.Remove(strippedOldName);
            }

            if(routePrefix != null)
            {
                RouteContainer newRoute = new RouteContainer(segmentId, routePrefix, routeNum);
                m_routeMap[segmentId] = newRoute;
            }
        }
            }
            string strippedNewName = StringUtilities.RemoveTags(newName);
            if (!m_usedNames.ContainsKey(strippedNewName))
        {
                m_usedNames[strippedNewName] = 0;
                }
            m_usedNames[strippedNewName] += 1;
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

        public string getRoute(ushort segmentId)
        {
            if( RouteExists(segmentId))
            {
                RouteContainer container = m_routeMap[segmentId];
                return container.m_routePrefix + container.m_routeNum.ToString();
            }
            else
            {
                return null;
            }
        }

        public bool RouteExists(ushort segmentId)
        {
            return m_routeMap.ContainsKey(segmentId);
        }

        public RoadContainer[] SaveRoads()
        {
            List<RoadContainer> returnList = new List<RoadContainer>(m_roadDict.Values);
            return returnList.ToArray();
        }

        public RouteContainer[] SaveRoutes()
        {
            return new List<RouteContainer>(m_routeMap.Values).ToArray();
        }

        public void Load(RoadContainer[] roadNames, RouteContainer[] routeNames)
        {
            if (m_roadDict != null)
            {
                foreach (RoadContainer road in roadNames)
        {
                    if (road.m_textObject == null)
            {
                        road.m_textObject = new GameObject();
                        road.m_textObject.AddComponent<MeshRenderer>();

                        road.m_textMesh = road.m_textObject.AddComponent<TextMesh>();
                        road.m_shieldObject = new GameObject();
                        road.m_shieldObject.AddComponent<MeshRenderer>();
                        road.m_shieldMesh = road.m_shieldObject.AddComponent<MeshFilter>();
                        road.m_numTextObject = new GameObject();
                        road.m_numTextObject.AddComponent<MeshRenderer>();
                        road.m_numMesh = road.m_numTextObject.AddComponent<TextMesh>();
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
            if (routeNames != null)
            {
                foreach (RouteContainer route in routeNames)
                {
                    m_routeMap[route.m_segmentId] = route;
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
        public GameObject m_shieldObject;

        [NonSerialized]
        public TextMesh m_textMesh;

        [NonSerialized]
        public MeshFilter m_shieldMesh;

        [NonSerialized]
        public GameObject m_numTextObject;

        [NonSerialized]
        public TextMesh m_numMesh;

        public RoadContainer(ushort segmentId, string roadName)
        {
            this.m_segmentId = segmentId;
            this.m_roadName = roadName;
        }
    }
    [Serializable]
    public class RouteContainer
    {

        public string m_routePrefix = null;
        public int m_routeNum = 0;

        public ushort m_segmentId = 0;

        public RouteContainer(ushort segmentId, string routePrefix, int routeNum)
        {
            this.m_segmentId = segmentId;
            this.m_routePrefix = routePrefix;
            this.m_routeNum = routeNum;
        }
    }
}
