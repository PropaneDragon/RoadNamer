using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadNamer.Utilities
{
    static class LoggerUtilities
    {
        private static readonly string TAG = "Road Name: ";

        public static void Log(string message)
        {
            Debug.Log(TAG + message);
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning(TAG + message);
        }

        public static void LogError(string message)
        {
            Debug.LogError(TAG + message);
        }

        public static void LogException(Exception e)
        {
            Debug.LogException(e);
        }

        public static void LogToConsole(string message)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, message);
        }

        public static void LogWarningToConsole(string message)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Warning, message);
        }

        public static void LogErrorToConsole(string message)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, message);
        }
    }
}
