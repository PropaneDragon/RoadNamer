using CimTools.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadNamer.Utilities
{
    class CimToolHolder
    {
        private static CimToolSettings settings = new CimToolSettings("RoadNamer");
        public static CimToolBase toolBase = new CimToolBase(settings);
    }
}
