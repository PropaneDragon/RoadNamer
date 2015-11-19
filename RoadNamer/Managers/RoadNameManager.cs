using System.Collections.Generic;

namespace RoadNamer.Managers
{
    public static class RoadNameManager
    {
        public static Dictionary<ushort, string> m_roadNames = new Dictionary<ushort, string>();

        public static void SetRoadName(ushort segmentId, string name)
        {
            m_roadNames[segmentId] = name;
        }

        public static string GetRoadName(ushort segmentId)
        {
            string returnString = null;

            if(m_roadNames.ContainsKey(segmentId))
            {
                returnString = m_roadNames[segmentId];
            }

            return returnString;
        }
    }
}
