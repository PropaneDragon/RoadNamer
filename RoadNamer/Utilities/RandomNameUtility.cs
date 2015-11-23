using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer.Utilities
{
    [Serializable()]
    [XmlRoot(ElementName = "RoadNameDocument")]
    public class RandomNameUtility
    {
        [XmlElement("AlwaysAssignPrefixes", IsNullable = false)]
        public bool m_alwaysAssignPrefixes = false;

        [XmlElement("AlwaysAssignPostfixes", IsNullable = false)]
        public bool m_alwaysAssignPostfixes = false;

        [XmlElement("DebugPrintRoadName", IsNullable = false)]
        public bool m_debugPrintRoadNames = false;

        [XmlArray("RoadPrefixes")]
        [XmlArrayItem("Prefix", typeof(RandomRoadPrefix))]
        public RandomRoadPrefix[] m_roadPrefixes;

        [XmlArray("RoadPostfixes")]
        [XmlArrayItem("Postfix", typeof(RandomRoadPostfix))]
        public RandomRoadPostfix[] m_roadPostfixes;

        [XmlArray("RoadNames")]
        [XmlArrayItem("RoadName", typeof(RandomRoadName))]
        public RandomRoadName[] m_roadNames;

        private static RandomNameUtility instance = null;
        public static RandomNameUtility Instance()
        {
            if (instance == null)
            {
                instance = new RandomNameUtility();
            }

            return instance;
        }

        public static void SetInstance(RandomNameUtility manager)
        {
            if (manager != null)
            {
                instance = manager;
            }
            else
            {
                LoggerUtilities.LogError("Tried to set RandomName instance to a null variable!");
            }
        }

        public static RandomRoadPrefix GetRoadPrefix(RandomRoadPrefix checkingPrefix, bool alwaysPick = false, int forcePick = -1)
        {
            RandomRoadPrefix returnPrefix = null;

            if (Instance().m_roadPrefixes != null && Instance().m_roadPrefixes.Length > 0)
            {
                List<RandomRoadPrefix> validPrefixes = new List<RandomRoadPrefix>();

                foreach (RandomRoadPrefix prefix in Instance().m_roadPrefixes)
                {
                    if (ValidRoadPrefix(prefix, checkingPrefix))
                    {
                        validPrefixes.Add(prefix);
                    }
                }

                if (validPrefixes.Count > 0)
                {
                    bool assignAPrefix = true;

                    if (!Instance().m_alwaysAssignPrefixes)
                    {
                        assignAPrefix = UnityEngine.Random.Range(0f, 1f) < 0.5f;
                    }

                    if (assignAPrefix || alwaysPick)
                    {
                        int prefixToPick = (int)Math.Round(UnityEngine.Random.Range(0f, validPrefixes.Count - 1));
                        prefixToPick = forcePick != -1 ? forcePick : prefixToPick;

                        returnPrefix = validPrefixes[prefixToPick];
                    }
                }
            }

            return returnPrefix;
        }

        private static bool ValidRoadPrefix(RandomRoadPrefix prefixToCheck, RandomRoadPrefix checkingPrefix)
        {
            bool valid = true;

            if (prefixToCheck.MinimumLanes > 0 && checkingPrefix.MinimumLanes <= prefixToCheck.MinimumLanes)
            {
                valid = false;
                LoggerUtilities.LogWarning("Min lane fail " + prefixToCheck.MinimumLanes + ", " + prefixToCheck.MinimumLanes);
            }

            if (prefixToCheck.MaximumLanes > 0 && checkingPrefix.MaximumLanes > prefixToCheck.MaximumLanes)
            {
                valid = false;
                LoggerUtilities.LogWarning("Max lane fail " + prefixToCheck.MaximumLanes + ", " + prefixToCheck.MaximumLanes);
            }

            if (prefixToCheck.NameHasToContain != null && prefixToCheck.NameHasToContain != "" && !checkingPrefix.NameHasToContain.Contains(prefixToCheck.NameHasToContain.ToLower()))
            {
                valid = false;
                LoggerUtilities.LogWarning("Road name fail " + prefixToCheck.NameHasToContain + ", " + prefixToCheck.NameHasToContain);
            }

            return valid;
        }

        public static RandomRoadName GetRoadName(RandomRoadName checkingName, int forcePick = -1)
        {
            RandomRoadName returnName = null;

            if (Instance().m_roadNames != null && Instance().m_roadNames.Length > 0)
            {
                List<RandomRoadName> validNames = new List<RandomRoadName>();

                foreach (RandomRoadName name in Instance().m_roadNames)
                {
                    if (ValidRoadName(name, checkingName))
                    {
                        validNames.Add(name);
                    }
                }

                if (validNames.Count > 0)
                {
                    int nameToPick = (int)Math.Round(UnityEngine.Random.Range(0f, validNames.Count - 1));
                    nameToPick = forcePick != -1 ? forcePick : nameToPick;

                    returnName = validNames[nameToPick];
                }
            }
            else
            {
                LoggerUtilities.LogWarning("There's no road names in the XML file! Can't use the randomiser!");
            }

            return returnName;
        }

        private static bool ValidRoadName(RandomRoadName nameToCheck, RandomRoadName checkingName)
        {
            bool valid = true;

            if (nameToCheck.MinimumLanes > 0 && checkingName.MinimumLanes <= nameToCheck.MinimumLanes)
            {
                valid = false;
                LoggerUtilities.LogWarning("Min lanes fail " + checkingName.MinimumLanes + ", " + nameToCheck.MinimumLanes);
            }

            if (nameToCheck.MaximumLanes > 0 && checkingName.MaximumLanes > nameToCheck.MaximumLanes)
            {
                valid = false;
                LoggerUtilities.LogWarning("Max lanes fail " + checkingName.MaximumLanes + ", " + nameToCheck.MaximumLanes);
            }

            if (nameToCheck.NameHasToContain != null && nameToCheck.NameHasToContain != "" && !checkingName.NameHasToContain.Contains(nameToCheck.NameHasToContain.ToLower()))
            {
                valid = false;
            }

            return valid;
        }

        public static RandomRoadPostfix GetRoadPostfix(RandomRoadPostfix checkingPostfix, bool alwaysPick = false, int forcePick = -1)
        {
            RandomRoadPostfix returnPostfix = null;

            if (Instance().m_roadPostfixes != null && Instance().m_roadPostfixes.Length > 0)
            {
                List<RandomRoadPostfix> validPostfixes = new List<RandomRoadPostfix>();

                foreach (RandomRoadPostfix postfix in Instance().m_roadPostfixes)
                {
                    if (ValidRoadPostfix(postfix, checkingPostfix))
                    {
                        validPostfixes.Add(postfix);
                    }
                }

                if (validPostfixes.Count > 0)
                {
                    bool assignAPostfix = true;

                    if (!Instance().m_alwaysAssignPostfixes)
                    {
                        assignAPostfix = UnityEngine.Random.Range(0f, 1f) < 0.5f;
                    }

                    if (assignAPostfix || alwaysPick)
                    {
                        int postfixToPick = (int)Math.Round(UnityEngine.Random.Range(0f, validPostfixes.Count - 1));
                        postfixToPick = forcePick != -1 ? forcePick : postfixToPick;

                        returnPostfix = validPostfixes[postfixToPick];
                    }
                }
            }

            return returnPostfix;
        }

        private static bool ValidRoadPostfix(RandomRoadPostfix postfixToCheck, RandomRoadPostfix checkingPostfix)
        {
            bool valid = true;

            if (postfixToCheck.MinimumLanes > 0 && checkingPostfix.MinimumLanes <= postfixToCheck.MinimumLanes)
            {
                valid = false;
                LoggerUtilities.LogWarning("Min lane fail " + postfixToCheck.MinimumLanes + ", " + postfixToCheck.MinimumLanes);
            }

            if (postfixToCheck.MaximumLanes > 0 && checkingPostfix.MaximumLanes > postfixToCheck.MaximumLanes)
            {
                valid = false;
                LoggerUtilities.LogWarning("Max lane fail " + postfixToCheck.MaximumLanes + ", " + postfixToCheck.MaximumLanes);
            }

            if (postfixToCheck.NameHasToContain != null && postfixToCheck.NameHasToContain != "" && !checkingPostfix.NameHasToContain.Contains(postfixToCheck.NameHasToContain.ToLower()))
            {
                valid = false;
                LoggerUtilities.LogWarning("Road name fail " + postfixToCheck.NameHasToContain + ", " + postfixToCheck.NameHasToContain);
            }

            return valid;
        }
    }

    [Serializable()]
    public class RandomRoadPrefix
    {
        [XmlElement(IsNullable = false)]
        public string Name = null;

        [XmlElement(IsNullable = false)]
        public string NameHasToContain = null;
        
        [XmlElement(IsNullable = false)]
        public string NameCannotContain = null;
        
        [XmlElement(IsNullable = false)]
        public int MinimumLanes = -1;

        [XmlElement(IsNullable = false)]
        public int MaximumLanes = -1;
    }

    [Serializable()]
    public class RandomRoadPostfix
    {
        [XmlElement(IsNullable = false)]
        public string Name = null;

        [XmlElement(IsNullable = false)]
        public string NameHasToContain = null;

        [XmlElement(IsNullable = false)]
        public string NameCannotContain = null;

        [XmlElement(IsNullable = false)]
        public int MinimumLanes = -1;

        [XmlElement(IsNullable = false)]
        public int MaximumLanes = -1;
    }

    [Serializable()]
    public class RandomRoadName
    {
        [XmlElement(IsNullable = false)]
        public string Name = null;

        [XmlElement(IsNullable = false)]
        public string NameHasToContain = null;
        
        [XmlElement(IsNullable = false)]
        public string NameCannotContain = null;
        
        [XmlElement(IsNullable = false)]
        public int MinimumLanes = -1;

        [XmlElement(IsNullable = false)]
        public int MaximumLanes = -1;

        [XmlElement(IsNullable = false)]
        public bool NoPrefix = false;

        [XmlElement(IsNullable = false)]
        public bool NoPostfix = false;

        [XmlElement(IsNullable = false)]
        public bool ForcePrefix = false;

        [XmlElement(IsNullable = false)]
        public bool ForcePostfix = false;
    }
}
