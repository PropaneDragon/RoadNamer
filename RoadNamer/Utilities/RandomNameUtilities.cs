using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static readonly Dictionary<string,List<string>> routeTypeNames = new Dictionary<string,List<string>>
        {
            { "highway",new List<string>() { "Route","Highway","Interstate","Autobahn","M" } },
            { "road",new List<string>() { "Route","Highway" } },
            { "prepend",new List<string>() { "M","A","I-" } }

        };
        public static readonly Dictionary<string,List<string>> roadTypeNames = new Dictionary<string,List<string>>
        {
            { "Train Track",new List<string>() { "Railway","Railine" } },
            { "Train Cargo Track",new List<string>() { "Railway","Railine" } },
            { "Train Station Track",new List<string>() { "Railway","Railine" } },
            { "Station Track Eleva",new List<string>() { "Railway","Railine","Viaduct" } },
            { "Station Track Sunken",new List<string>() { "Railway","Railine" } },
            { "Metro Track",new List<string>() { "Metro","Line","Subway" } },
            { "Metro Station Track",new List<string>() { "Metro","Line","Subway" } },
            { "Oneway Train Track",new List<string>() { "Railway","Railine" } },
            { "Concrete Train Track",new List<string>() { "Railway","Railine" } },
            { "Illuminated Tracks",new List<string>() { "Railway","Railine" } },
            { "Illumin No Cable",new List<string>() { "Railway","Railine" } },
            { "No Cable Train Track",new List<string>() { "Railway","Railine" } },
            { "No Cable Train Elevat",new List<string>() { "Railway","Railine","Viaduct" } },
            { "No Cable Concrete Trac",new List<string>() { "Railway","Railine" } },
            { "Tram Tracks",new List<string>() { "Tramline","Tramway" } },
            { "Tram Tracks No Cable",new List<string>() { "Tramline","Tramway" } },
            { "Concrete Train Track",new List<string>() { "Railway","Railine" } },
            { "Concrete Train Track",new List<string>() { "Railway","Railine" } },
            { "Pedestrian",new List<string>() { "Walkway","Pathway" } },
            { "Pedestrian Pavement",new List<string>() { "Walkway","Pathway" } },
            { "Pedestrian Gravel",new List<string>() { "Trail","Pathway" } },
            { "Zonable Pedestrian",new List<string>() { "Walkway","Pathway" } },
            { "Zonable Pedestrian Pavement",new List<string>() { "Walkway","Pathway" } },
            { "Zonable Pedestrian Gravel",new List<string>() { "Trail","Pathway" } },
            { "Gravel Road",new List<string>() { "Trail","Pathway","Backroad","Lane","Street","Road","Byway" } },
            { "Highway Ramp",new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { "Rural Highway",new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { "Highway",new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { "Highway Barrier",new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { "Large Highway",new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { "Five-Lane Highway",new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { "Four-Lane Highway",new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { "Basic Road",new List<string>() { "Road","Street","Court","Lane","Drive","Court" } },
            { "Harbor Road",new List<string>() { "Accessway","Road","Street","Drive" } },
            { "Oneway Road",new List<string>() { "Road","Street","Court","Lane","Drive" } },
            { "Oneway 3L",new List<string>() { "Road","Street","Drive","Way" } },
            { "Oneway 4L",new List<string>() { "Road","Drive","Way","Parkway","Highway" } },
            { "Oneway 4L",new List<string>() { "Road","Drive","Way","Parkway","Highway" } },
            { "Small Avenue",new List<string>() { "Road","Drive","Avenue","Way","Parkway","Highway" } },
            { "Medium Avenue",new List<string>() { "Road","Drive","Avenue","Way","Parkway","Highway" } },
            { "Medium Avenue TL",new List<string>() { "Road","Drive","Avenue","Way","Parkway","Highway" } },
            { "Large Road",new List<string>() { "Road","Way","Parkway","Highway"} },
            { "Large Oneway",new List<string>() { "Road","Way","Parkway","Highway" } },
            { "Large Oneway Road",new List<string>() { "Expressway","Freeway","Highway","Motorway" } },
            { "Small Busway",new List<string>() { "Busway" } },
            { "Small Busway One Way",new List<string>() { "Busway" } },

        };

        public static string GenerateName(bool isRoute,string roadType)
        {
            Random random = new Random();
            StringBuilder returnString = new StringBuilder();
            if (isRoute)
            {
                int routeNum = random.Next(1, 1000);
                string routeType = "";
                List<string> routeTypeNameList;
                switch (roadType)
                {
                    case "prepend":
                        routeTypeNameList = routeTypeNames["prepend"];
                        routeType = routeTypeNameList[random.Next(routeTypeNames[roadType].Count)];
                        returnString.Append(routeNum);
                        returnString.Append(routeType);
                        break;
                    case "highway":
                        routeTypeNameList = routeTypeNames["highway"];
                        routeType = routeTypeNameList[random.Next(routeTypeNames[roadType].Count)];
                        returnString.Append(routeType);
                        returnString.Append(" ");
                        returnString.Append(routeNum);
                        break;
                    case "road":
                    default:
                        routeTypeNameList = routeTypeNames["highway"];
                        routeType = routeTypeNameList[random.Next(routeTypeNames[roadType].Count)];
                        returnString.Append(routeType);
                        returnString.Append(" ");
                        returnString.Append(routeNum);
                        break;
                };
            }
            else
            {
                List<string> roadTypeNameList;
                string roadTypeName;
                string roadName;
                if (roadTypeNames.ContainsKey(roadType))
                {
                    roadTypeNameList = roadTypeNames[roadType];
                }
                else
                {
                    //Pick a reasonable list of names for the random road name
                    roadTypeNameList = roadTypeNames["Oneway 4L"];
                }
                roadTypeName = roadTypeNameList[random.Next(roadTypeNameList.Count)];
                roadName = roadNames[random.Next(roadNames.Count)];
                returnString.Append(roadName);
                returnString.Append(" ");
                returnString.Append(roadTypeName);
            }
            return returnString.ToString();
        }
    }
}
