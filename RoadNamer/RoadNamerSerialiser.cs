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
            Debug.Log("Road Namer: Saving road names");

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            try
            {
                RoadContainer[] roadNames = RoadNameManager.Instance().Save();

                if (roadNames != null)
                {
                    binaryFormatter.Serialize(memoryStream, roadNames);
                    serializableDataManager.SaveData(dataKey, memoryStream.ToArray());
                    Debug.Log("Road Namer: Road names have been saved!");
                }
                else
                {
                    Debug.LogWarning("Road Namer: Couldn't save road names, as the array is null!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
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
            Debug.Log("Road Namer: Loading road names");

            byte[] loadedData = serializableDataManager.LoadData(dataKey);

            if (loadedData != null)
            {
                Debug.Log("Road Namer: Found road names");

                try
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
                            Debug.LogWarning("Road Namer: Couldn't load road names, as the array is null!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Couldn't deserialise the road names!");
                        Debug.LogException(ex);
                    }
                    finally
                    {
                        memoryStream.Close();
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else
            {
                Debug.LogError("Couldn't insert the road names into memory!");
                Debug.LogWarning("Road Namer: Found no data to load");
            }
        }
    }
}
