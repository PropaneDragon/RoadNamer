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

        public Dictionary<ushort, RouteContainer> m_routeMap = new Dictionary<ushort, RouteContainer>();
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

        public void SetRoadName(ushort segmentId, string name, string routePrefix = null, int routeNum = -1)
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

            if(routePrefix != null)
            {
                RouteContainer newRoute = new RouteContainer(segmentId, routePrefix, routeNum);
                m_routeMap[segmentId] = newRoute;
            }
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
            m_roads = m_roadList.ToArray();

            return m_roads;
        }

        public RouteContainer[] SaveRoutes()
        {
            return new List<RouteContainer>(m_routeMap.Values).ToArray();
        }

        public void Load(RoadContainer[] roadNames, RouteContainer[] routeNames)
        {
            if (roadNames != null)
            {
                m_roads = roadNames;
                Initialise();
            }

            if(routeNames != null)
            {
                foreach (RouteContainer route in routeNames)
                {
                    m_routeMap[route.m_segmentId] = route;
                }
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
