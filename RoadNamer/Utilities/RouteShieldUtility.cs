using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer.Utilities
{
    [Serializable()]
    [XmlRoot(ElementName = "RouteShieldDocument")]
    class RouteShieldUtility
    {

        private static Dictionary<string, RouteShieldInfo> fallbackDict = new Dictionary<string, RouteShieldInfo>()
        {
            { "BC",new RouteShieldInfo(-0.25f, 0f, 0.5f, new Color32(50,46,136,255), "BC", "Icons/BC.png") },
            { "ON",new RouteShieldInfo(-0.65f, 0f, 0.5f, Color.black, "ON", "Icons/ON.png") },
            { "I",new RouteShieldInfo(-0.25f, 0f, 0.6f, Color.white, "I", "Icons/I.png") },
            { "US",new RouteShieldInfo(0f, 0f, 0.7f, Color.black, "US", "Icons/US.png") },
            { "AUS",new RouteShieldInfo(-0.25f, 0f, 0.5f, Color.white, "AUS", "Icons/AUS.png") },
            { "route",new RouteShieldInfo(0f, -0.5f, 0.3f, Color.white, "route", "Icons/route.png") },

        };

        [XmlIgnore]
        public Dictionary<string, RouteShieldInfo> routeShieldDictionary
        {
            get
            {
                return routeShieldInfoList == null ?
                        fallbackDict :
                        routeShieldInfoList.ToDictionary(item => item.Key, item => item.Value);
            }

            set
            {
                routeShieldInfoList = value == null ?
                                    fallbackDict.ToList() :
                                    value.ToList();
            }
        }


        [XmlArray("RouteShieldInfoList")]
        [XmlArrayItem("RouteShieldInfo", typeof(RouteShieldInfo))]
        public List<KeyValuePair<string, RouteShieldInfo>> routeShieldInfoList { get; set; }

        public RouteShieldInfo GetRouteShieldInfo(string key)
        {
            if (routeShieldDictionary.ContainsKey(key))
            {
                return routeShieldDictionary[key];
            }
            else
            {
                // Use a known value 
                return fallbackDict["route"];
            }
        }

        private static RouteShieldUtility instance = null;
        public static RouteShieldUtility Instance()
        {
            if (instance == null)
            {
                instance = new RouteShieldUtility();
            }

            return instance;
        }

        public static void SetInstance(RouteShieldUtility manager)
        {
            if (manager != null)
            {
                instance = manager;
            }
            else
            {
                LoggerUtilities.LogError("Tried to set RouteShieldInstance instance to a null variable!");
            }
        }

        /// <summary>
        /// Load all options from the disk.
        /// </summary>
        public static void LoadRouteShieldInfo()
        {
            if (File.Exists("RouteShieldOptions.xml"))
            {
                XmlSerializer xmlSerialiser = new XmlSerializer(typeof(RouteShieldUtility));
                StreamReader reader = new StreamReader("RouteShieldOptions.xml");

                RouteShieldUtility routeShieldUtility = xmlSerialiser.Deserialize(reader) as RouteShieldUtility;
                reader.Close();

                if (routeShieldUtility != null)
                {
                    SetInstance(routeShieldUtility);

                    LoggerUtilities.Log("Loaded route shield info file.");
                }
                else
                {
                    LoggerUtilities.LogError("Created route shield info is invalid!");
                }
            }
            else
            {
                LoggerUtilities.LogWarning("Could not load the route shield info file!");
            }
        }

       
    }

    [Serializable()]
    public class RouteShieldInfo
    {
        [XmlElement(IsNullable = false)]
        public float upOffset = 0f;

        [XmlElement(IsNullable = false)]
        public float leftOffset = 0f;

        [XmlElement(IsNullable = false)]
        public Color textColor = Color.black;

        [XmlElement(IsNullable = false)]
        public float textScale = 0.5f;

        [XmlElement(IsNullable = false)]
        public String textureName = "";

        [XmlElement(IsNullable = false)]
        public String texturePath = "";

        public RouteShieldInfo( float upOffset, float leftOffset ,float textScale, Color textColor,string textureName, string texturePath)
        {
            this.upOffset = upOffset;
            this.leftOffset = leftOffset;
            this.textColor = textColor;
            this.textScale = textScale;
            this.textureName = textureName;
            this.texturePath = texturePath;
        }
    }
}
