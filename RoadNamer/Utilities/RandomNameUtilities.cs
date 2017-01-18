using ColossalFramework.IO;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RoadNamer.Utilities
{

    internal class RandomNameConfiguration
    {

        private static readonly string ROAD_NAME_KEY = "roadnames";
        private static readonly string ROUTE_TYPE_KEY = "routetypes";
        private static readonly string ROAD_TYPE_KEY = "roadTypes";
        private static readonly string SIMPLIFY_ROAD_TYPE_MAPPING_KEY = "roadTypeMappings";

        //Taken from https://github.com/mabako/reddit-for-city-skylines
        private static string ConfigPath
        {
            get
            {
                string path = string.Format("{0}/{1}/", DataLocation.localApplicationData, "ModConfig");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path += "road-namer.json";
                return path;
            }
        }

        /// <summary>
        /// Mapping to reduce all the various RoadTypes to a smaller list for use with roadTypeNames
        /// </summary>
        public static Dictionary<RoadTypes, RoadTypes> m_simplifiedRoadTypeMapping = new Dictionary<RoadTypes, RoadTypes>()
        {
            {RoadTypes.None,RoadTypes.None},
            {RoadTypes.ZonablePedestrianGravel,RoadTypes.PedestrianGravel},
            {RoadTypes.ZonablePedestrianPavement,RoadTypes.PedestrianPavement},
            {RoadTypes.ZonablePedestrianElevated,RoadTypes.PedestrianPavement},
            {RoadTypes.PedestrianConnection,RoadTypes.PedestrianPavement},
            {RoadTypes.PedestrianConnectionInside,RoadTypes.PedestrianPavement},
            {RoadTypes.PedestrianConnectionSurface,RoadTypes.PedestrianPavement},
            {RoadTypes.PedestrianGravel,RoadTypes.PedestrianGravel},
            {RoadTypes.PedestrianPavement,RoadTypes.PedestrianPavement},
            {RoadTypes.PedestrianPavementBicycle,RoadTypes.PedestrianPavementBicycle},
            {RoadTypes.PedestrianPavementBicycleElevated,RoadTypes.PedestrianPavementBicycle},
            {RoadTypes.PedestrianSlope,RoadTypes.PedestrianPavement},
            {RoadTypes.PedestrianSlopeBicycle,RoadTypes.PedestrianPavementBicycle},
            {RoadTypes.PedestrianTunnel,RoadTypes.PedestrianPavement},
            {RoadTypes.PedestrianElevatedBicycle,RoadTypes.PedestrianPavementBicycle},
            {RoadTypes.PedestrianElevated,RoadTypes.PedestrianPavement},
            {RoadTypes.BasicRoad,RoadTypes.BasicRoad},
            {RoadTypes.BasicRoadBicycle,RoadTypes.BasicRoad},
            {RoadTypes.BasicRoadDecorationTrees,RoadTypes.BasicRoad},
            {RoadTypes.BasicRoadDecorationGrass,RoadTypes.BasicRoad},
            {RoadTypes.BasicRoadBridge,RoadTypes.BasicRoad},
            {RoadTypes.BasicRoadElevated,RoadTypes.BasicRoad},
            {RoadTypes.BasicRoadElevatedBike,RoadTypes.BasicRoad},
            {RoadTypes.BasicRoadTunnel,RoadTypes.BasicRoad},
            {RoadTypes.BasicRoadSlope,RoadTypes.BasicRoad},
            {RoadTypes.HarborRoad,RoadTypes.BasicRoad},
            {RoadTypes.SmallBusway,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayDecorationGrass,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayDecorationTrees,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayElevated,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayOneWay,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayOneWayDecorationGrass,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayOneWayDecorationTrees,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayOneWayElevated,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayOneWaySlope,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayOneWayTunnel,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayOneWayBridge,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswaySlope,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayTunnel,RoadTypes.SmallBusway},
            {RoadTypes.SmallBuswayBridge,RoadTypes.SmallBusway},
            {RoadTypes.SmallAvenue,RoadTypes.MediumRoad},
            {RoadTypes.SmallRuralHighway,RoadTypes.LargeHighway},
            {RoadTypes.OnewayRoad,RoadTypes.BasicRoad},
            {RoadTypes.OnewayRoadDecorationTrees,RoadTypes.BasicRoad},
            {RoadTypes.OnewayRoadDecorationGrass,RoadTypes.BasicRoad},
            {RoadTypes.OnewayRoadElevated,RoadTypes.BasicRoad},
            {RoadTypes.OnewayRoadBridge,RoadTypes.BasicRoad},
            {RoadTypes.OnewayRoadSlope,RoadTypes.BasicRoad},
            {RoadTypes.OnewayRoadTunnel,RoadTypes.BasicRoad},
            {RoadTypes.Oneway3L,RoadTypes.MediumRoad},
            {RoadTypes.Oneway4L,RoadTypes.MediumRoad},
            {RoadTypes.LargeOneway,RoadTypes.LargeRoad},
            {RoadTypes.LargeOnewayDecorationGrass,RoadTypes.LargeRoad},
            {RoadTypes.LargeOnewayDecorationTrees,RoadTypes.LargeRoad},
            {RoadTypes.LargeOnewayBridge,RoadTypes.LargeRoad},
            {RoadTypes.LargeOnewayElevated,RoadTypes.LargeRoad},
            {RoadTypes.LargeOnewayRoadSlope,RoadTypes.LargeRoad},
            {RoadTypes.MediumRoad,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadBicycle,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadBus,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadElevated,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadElevatedBike,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadElevatedBus,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadSlope,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadSlopeBike,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadSlopeBus,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadDecorationGrass,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadDecorationTrees,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadBridge,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadTunnel,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadTunnelBike,RoadTypes.MediumRoad},
            {RoadTypes.MediumRoadTunnelBus,RoadTypes.MediumRoad},
            {RoadTypes.MediumAvenue,RoadTypes.MediumRoad},
            {RoadTypes.MediumAvenueTL,RoadTypes.MediumRoad},
            {RoadTypes.LargeRoad,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadBicycle,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadBridge,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadBus,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadDecorationGrass,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadDecorationGrassWithBusLanes,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadDecorationTrees,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadDecorationTreesWithBusLanes,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadElevated,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadElevatedBike,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadElevatedBus,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadElevatedWithBusLanes,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadSlope,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadSlopeBus,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadSlopeWithBusLanes,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadTunnel,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadTunnelBus,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadTunnelWithBusLanes,RoadTypes.LargeRoad},
            {RoadTypes.LargeRoadWithBusLanes,RoadTypes.LargeRoad},
            {RoadTypes.LargeHighway,RoadTypes.Highway},
            {RoadTypes.LargeHighwayElevated,RoadTypes.Highway},
            {RoadTypes.LargeHighwaySlope,RoadTypes.Highway},
            {RoadTypes.LargeHighwayTunnel,RoadTypes.Highway},
            {RoadTypes.GravelRoad,RoadTypes.GravelRoad},
            {RoadTypes.TrainTrack,RoadTypes.TrainTrack},
            {RoadTypes.MetroTrack,RoadTypes.MetroTrack},
            {RoadTypes.MetroStationTrack,RoadTypes.MetroTrack},
            {RoadTypes.TrainStationTrack,RoadTypes.TrainTrack},
            {RoadTypes.TrainTrackBridge,RoadTypes.TrainTrack},
            {RoadTypes.TrainTrackElevated,RoadTypes.TrainTrack},
            {RoadTypes.TrainTrackSlope,RoadTypes.TrainTrack},
            {RoadTypes.TrainTrackTunnel,RoadTypes.TrainTrack},
            {RoadTypes.OnewayTrainTrack,RoadTypes.TrainTrack},
            {RoadTypes.OnewayTrainTrackElevated,RoadTypes.TrainTrack},
            {RoadTypes.OnewayTrainTrackSlope,RoadTypes.TrainTrack},
            {RoadTypes.OnewayTrainTrackTunnel,RoadTypes.TrainTrack},
            {RoadTypes.OnewayTrainTrackBridge,RoadTypes.TrainTrack},
            {RoadTypes.StationTrackEleva,RoadTypes.TrainTrack},
            {RoadTypes.StationTrackSunken,RoadTypes.TrainTrack},
            {RoadTypes.TrainConnectionTrack,RoadTypes.TrainTrack},
            {RoadTypes.RuralHighway,RoadTypes.RuralHighway},
            {RoadTypes.RuralHighwayElevated,RoadTypes.RuralHighway},
            {RoadTypes.RuralHighwaySlope,RoadTypes.RuralHighway},
            {RoadTypes.RuralHighwayTunnel,RoadTypes.RuralHighway},
            {RoadTypes.Highway,RoadTypes.Highway},
            {RoadTypes.HighwayBridge,RoadTypes.Highway},
            {RoadTypes.HighwayElevated,RoadTypes.Highway},
            {RoadTypes.HighwayRamp,RoadTypes.Highway},
            {RoadTypes.HighwayRampSlope,RoadTypes.Highway},
            {RoadTypes.HighwayRampElevated,RoadTypes.Highway},
            {RoadTypes.HighwayRampTunnel,RoadTypes.Highway},
            {RoadTypes.HighwaySlope,RoadTypes.Highway},
            {RoadTypes.HighwayTunnel,RoadTypes.Highway},
            {RoadTypes.HighwayBarrier,RoadTypes.Highway},
            {RoadTypes.AirplaneTaxiway,RoadTypes.AirplaneRunway},
            {RoadTypes.AirplaneRunway,RoadTypes.AirplaneRunway},
            {RoadTypes.Dam,RoadTypes.BasicRoad},
        };

        private static List<string> defaultRoadNames
        {
            get
            {
                var roadNameList = new List<string>();
                roadNameList.Add("Main");
                roadNameList.Add("High");
                roadNameList.Add("Park");
                return roadNameList;
            }
        }

       
        /// <summary>
        ///List of name used for road naming (main, first, etc)
        /// </summary>
        public static List<string> m_roadNames = defaultRoadNames;
        /// <summary>
        ///Hashset of names already used( for random name generator)
        /// </summary> 
        public static HashSet<string> m_usedNames = new HashSet<string>();
        /// <summary>
        ///Dict of route types to designations typically asscociated with the route type (Interstate, Autobahn, Route, Highway, etc)
        /// </summary>
        public static Dictionary<RoadTypes, List<string>> m_routeTypeNames = new Dictionary<RoadTypes, List<string>>
        {
            { RoadTypes.RuralHighway,new List<string>() { "Route","Highway","Interstate","Autobahn" } },
            { RoadTypes.Highway,new List<string>() { "Route","Highway","Interstate","Autobahn" } },
            { RoadTypes.BasicRoad,new List<string>() { "Route","Highway" } },
            { RoadTypes.MediumRoad,new List<string>() { "Route","Highway" } },
            { RoadTypes.LargeRoad,new List<string>() { "Route","Highway" } },
            { RoadTypes.None,new List<string>() { "M","A","I-" } }
        };
        
        /// <summary>
        /// Mapping of road types to typical road type names associated with the road type
        /// </summary>
        public static Dictionary<RoadTypes, List<string>> m_roadTypeNames = new Dictionary<RoadTypes, List<string>>
        {
            { RoadTypes.None, new List<string>() { "huh?", "what?" } },
            { RoadTypes.TrainTrack,new List<string>() { "Railway","Railine" } },
            { RoadTypes.MetroTrack,new List<string>() { "Metro","Line","Subway" } },
            { RoadTypes.PedestrianPavement,new List<string>() { "Walkway","Pathway" } },
            { RoadTypes.PedestrianGravel,new List<string>() { "Trail","Pathway" } },
            { RoadTypes.PedestrianPavementBicycle,new List<string>() { "Trail","Pathway","Bikeway" } },
            { RoadTypes.GravelRoad,new List<string>() { "Trail","Pathway","Backroad","Lane","Street","Road","Byway" } },
            { RoadTypes.RuralHighway,new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { RoadTypes.Highway,new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { RoadTypes.BasicRoad,new List<string>() { "Road","Street","Court","Lane","Drive","Court" } },
            { RoadTypes.MediumRoad,new List<string>() { "Road","Drive","Avenue","Way","Parkway","Highway" } },
            { RoadTypes.LargeRoad,new List<string>() { "Road","Way","Parkway","Highway"} },
            { RoadTypes.SmallBusway,new List<string>() { "Busway" } },
            { RoadTypes.AirplaneRunway,new List<string>() { "Runway" } }

        };

        internal static void Load( bool clearDefaultNames=true )
        {

            try
            {
                string buffer = File.ReadAllText(ConfigPath);
                var json = JSON.Parse(buffer).AsObject;
                if (json[ROAD_NAME_KEY] != null )
                {
                    // Populate road name list if available in json file
                    var roadNameList = json[ROAD_NAME_KEY].AsArray.List();
                    m_roadNames = clearDefaultNames ? new List<string>() : m_roadNames;
                    foreach (JSONNode node in roadNameList)
                    {
                        m_roadNames.Add(node.Value);
                    }
                }
                if (json[ROUTE_TYPE_KEY] != null)
                {
                    //Populate route type list if available in json file
                    var routeTypeMapping = json[ROUTE_TYPE_KEY].AsObject;
                    foreach(string roadTypeStr in routeTypeMapping.Keys)
                    {
                        try
                        {
                            RoadTypes roadType;
                            roadType = (RoadTypes)Enum.Parse(typeof(RoadTypes), roadTypeStr);
                            List<JSONNode> routeTypeList = routeTypeMapping[roadTypeStr].AsArray.List();
                            if (clearDefaultNames)
                            {
                                m_routeTypeNames[roadType] = new List<string>();
                            }
                            else
                            {
                                m_routeTypeNames[roadType] = m_routeTypeNames.ContainsKey(roadType) ? m_routeTypeNames[roadType] : new List<string>();
                            }
                            foreach ( JSONNode node in routeTypeList)
                            {
                                //TODO:Maybe check for duplicates with a set, and then convert back to list?
                                m_routeTypeNames[roadType].Add(node.Value);
                            }
                        }
                        catch
                        {
                            LoggerUtilities.Log("roadTypeStr does not map to any RoadTypes value: " + roadTypeStr);
                        }
                    }
                }
                if (json[ROAD_TYPE_KEY] != null)
                {
                    //Populate road type list if available in json file
                    var roadTypeMapping = json[ROAD_TYPE_KEY].AsObject;
                    foreach (string roadTypeStr in roadTypeMapping.Keys)
                    {
                        try
                        {
                            RoadTypes roadType;
                            roadType = (RoadTypes)Enum.Parse(typeof(RoadTypes), roadTypeStr);
                            List<JSONNode> roadTypeList = roadTypeMapping[roadTypeStr].AsArray.List();
                            if (clearDefaultNames)
                            {
                                m_roadTypeNames[roadType] = new List<string>();
                            }
                            else
                            {
                                m_roadTypeNames[roadType] = m_roadTypeNames.ContainsKey(roadType) ? m_roadTypeNames[roadType] : new List<string>();
                            }
                            foreach (JSONNode node in roadTypeList)
                            {
                                //TODO:Maybe check for duplicates with a set, and then convert back to list?
                                m_roadTypeNames[roadType].Add(node.Value);
                            }
                        }
                        catch
                        {
                            LoggerUtilities.LogWarning("roadTypeStr does not map to any RoadTypes value: " + roadTypeStr);
                        }
                    }
                }
                if(json[SIMPLIFY_ROAD_TYPE_MAPPING_KEY] != null)
                {
                    var roadTypeSimplifiedMappings = json[SIMPLIFY_ROAD_TYPE_MAPPING_KEY].AsObject;
                    
                    foreach(string roadTypeStr in roadTypeSimplifiedMappings.Keys)
                    {
                        try
                        {
                            RoadTypes keyRoadType;
                            RoadTypes valueRoadType;
                            string roadTypeMappingValue = roadTypeSimplifiedMappings[roadTypeStr].Value;
                            keyRoadType = (RoadTypes)Enum.Parse(typeof(RoadTypes), roadTypeStr);
                            valueRoadType = (RoadTypes)Enum.Parse(typeof(RoadTypes), roadTypeMappingValue);
                            //Don't clear out default values
                            m_simplifiedRoadTypeMapping[keyRoadType] = valueRoadType;
                        }
                        catch (ArgumentException)
                        {
                            LoggerUtilities.Log("RoadType from JSON is not valid!");
                        }
                    }
                }

                LoggerUtilities.Log("Random road names have been loaded!");
            }
            catch (Exception e)
            {
                LoggerUtilities.LogException(e);
            }
        }

    }

    /// <summary>
    /// List of names for the random name generator
    /// </summary>
    public static class RandomNameUtilities
    {
       
        /// <summary>
        /// Generates a route name(i.e: M1, I-234, Highway 101, etc)
        /// </summary>
        public static string GenerateRouteName( string roadTypeStr)
        {
            System.Random random = new System.Random();
            StringBuilder returnString = new StringBuilder();
            string routeTypeStr = "";
            int routeNum = random.Next(1, 1000);
            RoadTypes routeType = RoadTypes.MediumRoad;
            List<string> routeTypeNameList;

            try
            {
                routeType = (RoadTypes)Enum.Parse(typeof(RoadTypes), roadTypeStr);
            }
            catch
            {
                LoggerUtilities.LogWarning("roadTypeStr does not map to any RoadTypes value: " + roadTypeStr);
                //Don't bother, the default type is suitable for a fallback type
            }

            if ( RandomNameConfiguration.m_simplifiedRoadTypeMapping.ContainsKey(routeType))
            {
                routeType = RandomNameConfiguration.m_simplifiedRoadTypeMapping[routeType];
            }
            else
            {
                //Type is not applicable, return a road name instead
                return GenerateRoadName(roadTypeStr);
            }

            if (RandomNameConfiguration.m_routeTypeNames.ContainsKey(routeType))
            {
                routeTypeNameList = RandomNameConfiguration.m_routeTypeNames[routeType];
            }
            else
            {
                routeTypeNameList = RandomNameConfiguration.m_routeTypeNames[RoadTypes.MediumRoad];
            }

            if (routeType == RoadTypes.None)
            {
                roadTypeStr = routeTypeNameList[random.Next(RandomNameConfiguration.m_routeTypeNames[routeType].Count)];
                returnString.Append(routeTypeStr);
                returnString.Append(routeNum);
            }
            else
            {
                routeTypeStr = routeTypeNameList[random.Next(RandomNameConfiguration.m_routeTypeNames[routeType].Count)];
                returnString.Append(routeTypeStr);
                returnString.Append(" ");
                returnString.Append(routeNum);
            }

            return returnString.ToString();
        }

        public static string GenerateRoadName( string roadTypeStr )
        {
            System.Random random = new System.Random();
            StringBuilder returnString = new StringBuilder();

            List<string> roadTypeNameList;
            string roadTypeName;
            string roadName;
            RoadTypes roadType = RoadTypes.MediumRoad;
            try
            {
                roadType = (RoadTypes)Enum.Parse(typeof(RoadTypes), roadTypeStr);
            }
            catch
            {
                LoggerUtilities.LogWarning("roadTypeStr does not map to any RoadTypes value: " + roadTypeStr);
                //Don't bother, the default type is suitable for a fallback type
            }

            //If a road type exists in the simplified mapping, then use it, otherwise,
            //fall back to Medium Roads
            if (RandomNameConfiguration.m_simplifiedRoadTypeMapping.ContainsKey(roadType))
            {
                roadType = RandomNameConfiguration.m_simplifiedRoadTypeMapping[roadType];
            }

            roadTypeNameList = RandomNameConfiguration.m_roadTypeNames[roadType];
            roadTypeName = roadTypeNameList[random.Next(roadTypeNameList.Count)];
            roadName = RandomNameConfiguration.m_roadNames[random.Next(RandomNameConfiguration.m_roadNames.Count)];
            returnString.Append(roadName);
            returnString.Append(" ");
            returnString.Append(roadTypeName);
            return returnString.ToString();
        }
    }
}
