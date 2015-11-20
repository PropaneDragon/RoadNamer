using ICities;
using UnityEngine;

namespace RoadNamer.Utilities
{
    class SaveUtility : SerializableDataExtensionBase
    {
        const string dataKey = "RoadNamerTool";

        public override void OnLoadData()
        {
            byte[] data = serializableDataManager.LoadData(dataKey);

            if (data != null)
            {

            }
            else
            {
                Debug.Log("Road Namer found no data to load");
            }
        }
    }
}
