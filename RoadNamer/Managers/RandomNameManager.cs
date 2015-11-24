using ColossalFramework;
using RoadNamer.Utilities;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer.Managers
{
    public class RandomNameManager
    {
        public static string m_fileName = null;

        public static string GenerateRandomRoadName(ushort netSegmentId)
        {
            string returnRoadName = null;

            if (netSegmentId != 0)
            {
                NetManager netManager = Singleton<NetManager>.instance;
                NetSegment netSegment = netManager.m_segments.m_buffer[(int)netSegmentId];
                NetSegment.Flags segmentFlags = netSegment.m_flags;

                if (segmentFlags.IsFlagSet(NetSegment.Flags.Created))
                {
                    int forwardLanes = 0, backwardLanes = 0, totalLanes = 0;
                    string roadName = netSegment.Info.name.ToLower();
                    RandomRoadPrefix selectedPrefix = null;
                    RandomRoadName selectedRoadName = null;
                    RandomRoadPostfix selectedPostfix = null;

                    netSegment.CountLanes(netSegmentId, NetInfo.LaneType.Vehicle | NetInfo.LaneType.PublicTransport | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.All, ref forwardLanes, ref backwardLanes);
                    totalLanes = forwardLanes + backwardLanes;

                    Debug.Log(roadName);
                    Debug.Log(forwardLanes.ToString());
                    Debug.Log(backwardLanes.ToString());

                    RandomRoadPrefix checkingPrefix = new RandomRoadPrefix()
                    {
                        NameHasToContain = roadName,
                        MinimumLanes = totalLanes,
                        MaximumLanes = totalLanes
                    };

                    RandomRoadName checkingName = new RandomRoadName()
                    {
                        NameHasToContain = roadName,
                        MinimumLanes = totalLanes,
                        MaximumLanes = totalLanes
                    };

                    RandomRoadPostfix checkingPostfix = new RandomRoadPostfix()
                    {
                        NameHasToContain = roadName,
                        MinimumLanes = totalLanes,
                        MaximumLanes = totalLanes
                    };

                    selectedRoadName = RandomNameUtility.GetRoadName(checkingName);

                    if (selectedRoadName != null)
                    {
                        if (!selectedRoadName.NoPrefix)
                        {
                            selectedPrefix = RandomNameUtility.GetRoadPrefix(checkingPrefix, selectedRoadName.ForcePrefix);
                        }

                        if (!selectedRoadName.NoPostfix)
                        {
                            selectedPostfix = RandomNameUtility.GetRoadPostfix(checkingPostfix, selectedRoadName.ForcePostfix);
                        }

                        returnRoadName = (selectedPrefix != null ? selectedPrefix.Name + " " : "") + selectedRoadName.Name + (selectedPostfix != null ? " " + selectedPostfix.Name : "");
                        returnRoadName = TranslateRoadName(netSegmentId, returnRoadName);
                    }
                }
            }

            return returnRoadName;
        }

        public static void LoadRandomNames()
        {
            if (m_fileName != null)
            {
                string fullFilePath = OptionsManager.m_randomNamesLocation + m_fileName + ".xml";

                if (File.Exists(fullFilePath))
                {
                    XmlSerializer xmlSerialiser = new XmlSerializer(typeof(RandomNameUtility));
                    StreamReader reader = new StreamReader(fullFilePath);

                    RandomNameUtility nameUtility = xmlSerialiser.Deserialize(reader) as RandomNameUtility;
                    reader.Close();

                    if (nameUtility != null)
                    {
                        RandomNameUtility.SetInstance(nameUtility);
                    }
                    else
                    {
                        Debug.LogError("Road Namer: Couldn't load random names!");
                    }

                    Debug.Log("Road Namer: Loaded name XML \"" + fullFilePath + "\"");
                }
            }
        }

        private static string TranslateRoadName(ushort netSegmentId, string roadName)
        {
            NetManager netManager = Singleton<NetManager>.instance;
            NetSegment netSegment = netManager.m_segments.m_buffer[(int)netSegmentId];
            NetSegment.Flags segmentFlags = netSegment.m_flags;

            //Regex randomRegex = new Regex("(%RANDOM\\()([0-9]+)(,)([0-9]+)\\)%"); --Works, but unused.

            if (segmentFlags.IsFlagSet(NetSegment.Flags.Created))
            {
                int random100 = (int)Math.Round(UnityEngine.Random.Range(1f, 100f));
                int random1000 = (int)Math.Round(UnityEngine.Random.Range(1f, 1000f));

                roadName = roadName.Replace("%SEGMENTID%", netSegmentId.ToString());
                roadName = roadName.Replace("%RANDOM100%", random100.ToString());
                roadName = roadName.Replace("%RANDOM100POSTFIX%", random100.ToString() + AddRoadPostfix(random100));
                roadName = roadName.Replace("%RANDOM1000%", random1000.ToString());
                roadName = roadName.Replace("%RANDOM1000POSTFIX%", random1000.ToString() + AddRoadPostfix(random1000));
            }

            return roadName;
        }

        private static string AddRoadPostfix(int roadNumber)
        {
            string returnString = "";

            switch(roadNumber % 10)
            {
                case 1:
                    returnString = "st";
                    break;
                case 2:
                    returnString = "nd";
                    break;
                case 3:
                    returnString = "rd";
                    break;
                default:
                    returnString = "th";
                    break;
            }

            return returnString;
        }
    }
}
