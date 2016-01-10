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

        /// <summary>
        /// Dictionary of the segmentId to the route name container
        /// </summary>
        public Dictionary<ushort, RouteContainer> m_routeDict = new Dictionary<ushort, RouteContainer>();

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

        public void DelRoadRoute(ushort segmentId)
        {
            if (m_routeDict.ContainsKey(segmentId))
            {
                string routePrefix = m_routeDict[segmentId].m_routePrefix;
                string routeNum = m_routeDict[segmentId].m_route.ToString();
                string routeStr = routePrefix + '/' + routeNum;
                m_routeDict.Remove(segmentId);
                DecrementRoadRouteCounter(routeStr);
            }

        }

        public void SetRoadName(ushort segmentId, string newName, string oldName = null, string routePrefix = null, string route = null, string oldRouteStr=null)
        {
            RoadContainer roadContainer = null;
            RouteContainer routeContainer = null;
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

            if (routePrefix != null)
            {
                if (m_routeDict.ContainsKey(segmentId))
                {
                    routeContainer = m_routeDict[segmentId];
                    routeContainer.m_routePrefix = routePrefix;
                    routeContainer.m_route = route;
                }
                else
                {
                    routeContainer = new RouteContainer(segmentId, routePrefix, route);
                }
                m_routeDict[segmentId] = routeContainer;
                if (routeContainer.m_shieldObject == null && routeContainer.m_numTextObject == null)
                {
                    routeContainer.m_shieldObject = new GameObject();
                    routeContainer.m_shieldObject.AddComponent<MeshRenderer>();
                    routeContainer.m_shieldMesh = routeContainer.m_shieldObject.AddComponent<MeshFilter>();
                    routeContainer.m_numTextObject = new GameObject();
                    routeContainer.m_numTextObject.AddComponent<MeshRenderer>();
                    routeContainer.m_numMesh = routeContainer.m_numTextObject.AddComponent<TextMesh>();
                }
                string routeStr = routePrefix + '/' + route;
                if (!m_usedRoutes.ContainsKey(routeStr))
                {
                    m_usedRoutes[routeStr] = 0;
                }
                m_usedRoutes[routeStr] += 1;

                if (oldRouteStr != null)
                {
                    if (m_usedRoutes.ContainsKey(oldRouteStr))
                    {
                        m_usedRoutes[oldRouteStr] -= 1;
                        if (m_usedRoutes[oldRouteStr] <= 0)
                        {
                            m_usedRoutes.Remove(oldRouteStr);
                        }

                    }
                }
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

        public string GetRouteType(ushort segmentId)
        {
            if (RouteExists(segmentId))
            {
                RouteContainer container = m_routeDict[segmentId];
                return container.m_routePrefix;
            }
            else
            {
                return null;
            }
        }

        public string GetRouteStr(ushort segmentId)
        {
            if (RouteExists(segmentId))
            {
                RouteContainer container = m_routeDict[segmentId];
                return container.m_route;
            }
            else
            {
                return null;
            }
        }

        public bool RouteExists(ushort segmentId)
        {
            return m_routeDict.ContainsKey(segmentId);
        }

        public RoadContainer[] SaveRoads()
        {
            List<RoadContainer> returnList = new List<RoadContainer>(m_roadDict.Values);
            return returnList.ToArray();
        }

        public RouteContainer[] SaveRoutes()
        {
            return new List<RouteContainer>(m_routeDict.Values).ToArray();
        }

        public void Load(RoadContainer[] roadNames, RouteContainer[] routeNames)
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

            if (routeNames != null)
            {
                foreach (RouteContainer route in routeNames)
                {
                    m_routeDict[route.m_segmentId] = route;
                    if (route.m_shieldObject == null && route.m_numTextObject == null) {
                        route.m_shieldObject = new GameObject();
                        route.m_shieldObject.AddComponent<MeshRenderer>();
                        route.m_shieldMesh = route.m_shieldObject.AddComponent<MeshFilter>();
                        route.m_numTextObject = new GameObject();
                        route.m_numTextObject.AddComponent<MeshRenderer>();
                        route.m_numMesh = route.m_numTextObject.AddComponent<TextMesh>();
                    }

                    string routeStr = route.m_routePrefix + '/'+ route.m_route;
                    if (!m_usedRoutes.ContainsKey(routeStr))
                    {
                        m_usedRoutes[routeStr] = 0;
                    }
                    m_usedRoutes[routeStr] += 1;
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

    [Serializable]
    public class RouteContainer
    {

        public string m_routePrefix = null;
        public string m_route = "";

        public ushort m_segmentId = 0;

        [NonSerialized]
        public GameObject m_shieldObject;

        [NonSerialized]
        public MeshFilter m_shieldMesh;

        [NonSerialized]
        public GameObject m_numTextObject;

        [NonSerialized]
        public TextMesh m_numMesh;

        public RouteContainer(ushort segmentId, string routePrefix, string route)
        {
            this.m_segmentId = segmentId;
            this.m_routePrefix = routePrefix;
            this.m_route = route;
        }
    }
}
