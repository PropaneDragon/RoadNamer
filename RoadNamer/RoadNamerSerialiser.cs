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
        const string routeDataKey = "RoadNamerToolRoutes";

        public override void OnSaveData()
        {
            Debug.Log("Road Namer: Saving road names");

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream roadMemoryStream = new MemoryStream();
            MemoryStream routeMemoryStream = new MemoryStream();

            try
            {
                RoadContainer[] roadNames = RoadNameManager.Instance().SaveRoads();
                RouteContainer[] routeNames = RoadNameManager.Instance().SaveRoutes();
                if (roadNames != null)
                {
                    binaryFormatter.Serialize(roadMemoryStream, roadNames);
                    serializableDataManager.SaveData(roadDataKey, roadMemoryStream.ToArray());
                    binaryFormatter.Serialize(routeMemoryStream, routeNames);
                    serializableDataManager.SaveData(routeDataKey, routeMemoryStream.ToArray());
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
                roadMemoryStream.Close();
                routeMemoryStream.Close();
            }
        }

        public override void OnLoadData()
        {
            LoadNames();
            RandomNameManager.LoadRandomNames();           
        }

        private void LoadNames()
        {
            Debug.Log("Road Namer: Loading road names");

            byte[] loadedRoadData = serializableDataManager.LoadData(roadDataKey);
            byte[] loadedRouteData = serializableDataManager.LoadData(routeDataKey);

            if (loadedRoadData != null)
            {
                MemoryStream roadMemoryStream = new MemoryStream();
                MemoryStream routeMemoryStream = new MemoryStream();
                roadMemoryStream.Write(loadedRoadData, 0, loadedRoadData.Length);
                roadMemoryStream.Position = 0;
                routeMemoryStream.Write(loadedRouteData, 0, loadedRouteData.Length);
                routeMemoryStream.Position = 0;

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                try
                {
                    RoadContainer[] roadNames = binaryFormatter.Deserialize(roadMemoryStream) as RoadContainer[];
                    RouteContainer[] routeNames = binaryFormatter.Deserialize(routeMemoryStream) as RouteContainer[];

                    if (roadNames != null)
                    {
                        RoadNameManager.Instance().Load(roadNames,routeNames);
                    }
                    else
                    {
                        Debug.LogWarning("Road Namer: Couldn't load road names, as the array is null!");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                finally
                {
                    roadMemoryStream.Close();
                    routeMemoryStream.Close();
                }
            }
            else
            {
                Debug.LogWarning("Road Namer: Found no data to load");
            }
        }
    }
}
