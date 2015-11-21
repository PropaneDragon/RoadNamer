using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadNamer.Utilities
{
    /// <summary>
    /// List of names for the random name generator
    /// </summary>
    class RandomNameUtilities
    {

        public static readonly List<string> roadNames = new List<string>()
        {
            "Main",
            "Station",
            "High",
            "Park",
            "Broadway",
            "Church",
            "Lake",
            "Hill",
            "Bridge",
            "Ridge",
            "View",
            "Water",
            "Front",
            "Market",
            "Fairway",
            "Canterbury",
            "Highbury",
            "Veteran's Memorial",
            "Oak",
            "Elm",
            "Cedar",
            "Pine",
            "Hickory",
            "Maple",

            "First",
            "Second",
            "Third",
            "Fourth",
            "Fifth",

            "Alaska",
            "Connecticut",
            "Illinois",
            "Michigan",
            "New York",
            "Pennsylvania",
            "Washington",
            "Yonge",
            "Bloor",
            "Charles",
            "Windsor",

            "TotallyMoo",
            "BloodyPenguin",
            "Boformer"
        };

        public static readonly Dictionary<string, List<string>> routeTypeNames = new Dictionary<string, List<string>>
        {
            //TODO: Either redefine the keys as RoadTypes, or create const values for the keys
            { "highway",new List<string>() { "Route","Highway","Interstate","Autobahn" } },
            { "road",new List<string>() { "Route","Highway" } },
            { "singlechar",new List<string>() { "M","A","I-" } }
        };

        /// <summary>
        /// Mapping to reduce all the various RoadTypes to a smaller list for use with roadTypeNames
        /// </summary>
        public static readonly Dictionary<RoadTypes, RoadTypes> simplifiedMapping = new Dictionary<RoadTypes, RoadTypes>()
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

        /// <summary>
        /// Mapping of road types to typical road type names associated with the road type
        /// </summary>
        public static readonly Dictionary<RoadTypes, List<string>> roadTypeNames = new Dictionary<RoadTypes, List<string>>
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
                Debug.Log("roadTypeStr does not map to any RoadTypes value: " + roadTypeStr);
                //Don't bother, the default type is suitable for a fallback type
            }

            if (simplifiedMapping.ContainsKey(routeType))
            {
                routeType = simplifiedMapping[routeType];
            }
            else
            {
                //Type is not applicable, return a road name instead
                return GenerateRoadName(roadTypeStr);
            }

            switch (routeType)
            {
                //Single character route types (i.e:M1,I-111)
                //TODO:Maybe replace single letter route type with a seperate enum value?
                case RoadTypes.None:
                    routeTypeNameList = routeTypeNames["prepend"];
                    roadTypeStr = routeTypeNameList[random.Next(routeTypeNames["prepend"].Count)];
                    returnString.Append(routeTypeStr);
                    returnString.Append(routeNum);
                    break;
                //Highway route types (i.e:Highway 101, Interstate 128, etc)
                case RoadTypes.RuralHighway:
                case RoadTypes.Highway:
                    routeTypeNameList = routeTypeNames["highway"];
                    routeTypeStr = routeTypeNameList[random.Next(routeTypeNames["highway"].Count)];
                    returnString.Append(routeTypeStr);
                    returnString.Append(" ");
                    returnString.Append(routeNum);
                    break;
                //basic road route types (i.e:Route 1, highway 128, etc)
                case RoadTypes.BasicRoad:
                case RoadTypes.MediumRoad:
                case RoadTypes.LargeRoad:
                    routeTypeNameList = routeTypeNames["road"];
                    routeTypeStr = routeTypeNameList[random.Next(routeTypeNames["road"].Count)];
                    returnString.Append(routeTypeStr);
                    returnString.Append(" ");
                    returnString.Append(routeNum);
                    break;
                default:
                    //Type is not applicable, return a road name instead
                    return GenerateRoadName(roadTypeStr);
            };

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
                Debug.Log("roadTypeStr does not map to any RoadTypes value: " + roadTypeStr);
                //Don't bother, the default type is suitable for a fallback type
            }

            //If a road type exists in the simplified mapping, then use it, otherwise,
            //fall back to Medium Roads
            if (simplifiedMapping.ContainsKey(roadType))
            {
                roadType = simplifiedMapping[roadType];
            }

            roadTypeNameList = roadTypeNames[roadType];
            roadTypeName = roadTypeNameList[random.Next(roadTypeNameList.Count)];
            roadName = roadNames[random.Next(roadNames.Count)];
            returnString.Append(roadName);
            returnString.Append(" ");
            returnString.Append(roadTypeName);

            return returnString.ToString();
        }
    }
}
