using CimTools.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadNamer.Utilities
{
    class CimToolHolder
    {
        private static readonly ulong WORKSHOP_ID = 558960454;

        private static CimToolSettings settings = new CimToolSettings("RoadNamer",workshopId:WORKSHOP_ID);
        public static CimToolBase toolBase = new CimToolBase(settings);
    }
}
