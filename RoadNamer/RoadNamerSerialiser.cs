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
        const string dataKey = "RoadNamerTool";

        public override void OnSaveData()
        {
            LoggerUtilities.Log("Saving road names");

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            try
            {
                RoadContainer[] roadNames = RoadNameManager.Instance().Save();

                if (roadNames != null)
                {
                    binaryFormatter.Serialize(memoryStream, roadNames);
                    serializableDataManager.SaveData(dataKey, memoryStream.ToArray());
                    LoggerUtilities.Log("Road names have been saved!");
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
                memoryStream.Close();
            }
        }

        public override void OnLoadData()
        {
            LoadRoadNames();
            RandomNameManager.LoadRandomNames();           
        }

        private void LoadRoadNames()
        {
            LoggerUtilities.Log("Loading road names");

            byte[] loadedData = serializableDataManager.LoadData(dataKey);

            if (loadedData != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(loadedData, 0, loadedData.Length);
                memoryStream.Position = 0;

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                try
                {
                    RoadContainer[] roadNames = binaryFormatter.Deserialize(memoryStream) as RoadContainer[];

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
                    memoryStream.Close();
                }
            }
            else
            {
                LoggerUtilities.LogWarning("Found no data to load");
            }
        }
    }
}
