using ColossalFramework;
using RoadNamer.Utilities;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer.Managers
{
    public class RandomNameManager
    {
        public static string GenerateRandomRoadName(ushort netSegmentId)
        {
            string returnRoadName = null;

            if(netSegmentId != 0)
            {
                NetManager netManager = Singleton<NetManager>.instance;
                NetSegment netSegment = netManager.m_segments.m_buffer[(int)netSegmentId];
                NetSegment.Flags segmentFlags = netSegment.m_flags;

                if(segmentFlags.IsFlagSet(NetSegment.Flags.Created))
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
                    }
                }
            }

            return returnRoadName;
        }

        public static void LoadRandomNames()
        {
            string modPath = FileUtilities.GetModPath();

            if (modPath != null && Directory.Exists(modPath + "/Names"))
            {
                foreach (string filePath in Directory.GetFiles(modPath + "/Names/"))
                {
                    string fileExtension = Path.GetExtension(filePath);

                    Debug.Log(fileExtension);

                    if (fileExtension == ".xml")
                    {
                        if (File.Exists(filePath))
                        {
                            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(RandomNameUtility));
                            StreamReader reader = new StreamReader(filePath);

                            RandomNameUtility nameUtility = xmlSerialiser.Deserialize(reader) as RandomNameUtility;
                            reader.Close();

                            if (nameUtility != null)
                            {
                                Debug.Log(nameUtility.m_alwaysAssignPostfixes.ToString());
                                Debug.Log(nameUtility.m_alwaysAssignPrefixes.ToString());

                                RandomNameUtility.SetInstance(nameUtility);
                            }
                            else
                            {
                                Debug.LogError("Road Namer: Couldn't load random names!");
                            }

                            Debug.Log("Road Namer: Loaded name XML \"" + filePath + "\"");
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Road Namer: Couldn't find the \"Names\" directory!");
            }
        }
    }
}
