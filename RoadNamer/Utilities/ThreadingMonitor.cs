using ICities;
using System.Collections.Generic;

namespace RoadNamer.Managers
{
    public class ThreadingMonitor : ThreadingExtensionBase
    {
        public override void OnCreated(IThreading threading)
        {
            base.OnCreated(threading);

        }

        public override void OnAfterSimulationTick()
        {
            HashSet<ushort> segmentsToBeDeleted = new HashSet<ushort>();
            foreach (ushort segment in RoadNameManager.Instance().m_roadDict.Keys)
            {
                if ((NetManager.instance.m_segments.m_buffer[segment].m_flags) == NetSegment.Flags.None)
                {
                    segmentsToBeDeleted.Add(segment);
                }
            }

            foreach( ushort segment in segmentsToBeDeleted)
            {
                RoadNameManager.Instance().DelRoadName(segment);
            }

            base.OnAfterSimulationTick();
        }
    }
}
