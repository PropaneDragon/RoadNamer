using ICities;
using RoadNamer.Managers;
using RoadNamer.Utilities;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer
{
    public class RoadNamerSerialiser : SerializableDataExtensionBase
    {
        const string roadDataKey = "RoadNamerTool";
        const string routeDataKey = "RoadNamerToolRoute";

        public override void OnSaveData()
        {
            LoggerUtilities.Log("Saving road names");

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream roadMemoryStream = new MemoryStream();

            try
            {
                RoadContainer[] roadNames = RoadNameManager.Instance().SaveRoads();
                if (roadNames != null)
                {
                    binaryFormatter.Serialize(roadMemoryStream, roadNames);
                    serializableDataManager.SaveData(roadDataKey, roadMemoryStream.ToArray());
                    Debug.Log("Road Namer: Road names have been saved!");
                }
                else
                {
                    LoggerUtilities.LogWarning("Couldn't save road names, as the array is null!");
                }
            }
            catch (Exception ex)
            {
                LoggerUtilities.LogException(ex);
            }
            finally
            {
                roadMemoryStream.Close();
            }
        }

        public override void OnLoadData()
        {
            LoadNames();
            RandomNameManager.LoadRandomNames();           
        }

        private void LoadNames()
        {
            LoggerUtilities.Log("Loading road names");

            byte[] loadedRoadData = serializableDataManager.LoadData(roadDataKey);

            if (loadedRoadData != null)
            {
                MemoryStream roadMemoryStream = new MemoryStream();
                roadMemoryStream.Write(loadedRoadData, 0, loadedRoadData.Length);
                roadMemoryStream.Position = 0;

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                try
                {
                    RoadContainer[] roadNames = binaryFormatter.Deserialize(roadMemoryStream) as RoadContainer[];

                    if (roadNames != null)
                    {
                        RoadNameManager.Instance().Load(roadNames);
                    }
                    else
                    {
                        LoggerUtilities.LogWarning("Couldn't load road names, as the array is null!");
                    }
                }
                catch (Exception ex)
                {
                    LoggerUtilities.LogException(ex);

                }
                finally
                {
                    roadMemoryStream.Close();
                }
            }
            else
            {
                LoggerUtilities.LogWarning("Found no data to load");
            }

        }
    }
}
