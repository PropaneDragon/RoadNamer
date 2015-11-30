using ColossalFramework;
using UnityEngine;
using RoadNamer.Managers;
using ColossalFramework.UI;
using System;
using RoadNamer.Panels;
using RoadNamer.Utilities;

namespace RoadNamer.Tools
{
    public class RoadSelectTool : DefaultTool
    {
        public RoadNamePanel m_roadNamePanel = null;
        public UsedNamesPanel m_usedNamesPanel = null;

        protected override void Awake()
        {
            LoggerUtilities.Log("Tool awake");

            base.Awake();
        }

        protected override void OnToolGUI()
        {
            base.OnToolGUI();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnToolUpdate()
        {
            if (m_toolController != null && !m_toolController.IsInsideUI && Cursor.visible)
            {
                RaycastOutput raycastOutput;

                if (RaycastRoad(out raycastOutput))
                {
                    ushort netSegmentId = raycastOutput.m_netSegment;

                    if (netSegmentId != 0)
                    {
                        NetManager netManager = Singleton<NetManager>.instance;
                        NetSegment netSegment = netManager.m_segments.m_buffer[(int)netSegmentId];

                        if (netSegment.m_flags.IsFlagSet(NetSegment.Flags.Created))
                        {
                            if (Event.current.type == EventType.MouseDown /*&& Event.current.button == (int)UIMouseButton.Left*/)
                            {
                                ShowToolInfo(false, null, new Vector3());

                                if (m_roadNamePanel != null)
                                {
#if DEBUG
                                    NetNode startNode = netManager.m_nodes.m_buffer[netSegment.m_startNode]; //Not used yet, but there just incase. This isn't final
                                    NetNode endNode = netManager.m_nodes.m_buffer[netSegment.m_endNode];
                                    Vector3 rotation = Vector3.Cross(startNode.m_position, endNode.m_position);
                                    LoggerUtilities.LogToConsole(rotation.ToString());
#endif
                                    RandomNameManager.LoadRandomNames();
                                    m_roadNamePanel.m_initialRouteNum = RoadNameManager.Instance().getRouteNum(netSegmentId);
                                    m_roadNamePanel.m_initialRoutePrefix = RoadNameManager.Instance().getRouteType(netSegmentId);
                                    m_roadNamePanel.initialRoadName = RoadNameManager.Instance().GetRoadName(netSegmentId);

                                    m_roadNamePanel.m_netSegmentId = netSegmentId;
                                    m_roadNamePanel.m_netSegmentName = netSegment.Info.name.Replace(" ","");
                                    m_roadNamePanel.Show();
                                    m_usedNamesPanel.RefreshList();
                                    m_usedNamesPanel.Show();

                                    OptionsManager.m_hasOpenedPanel = true;
                                    OptionsManager.SaveOptions();
                                }
                            }
                            else
                            {
                                ShowToolInfo(true, "Click to name this road segment", netSegment.m_bounds.center);
                            }
                        }
                    }
                }
            }
            else
            {
                ShowToolInfo(false, null, new Vector3());
            }
        }

        bool RaycastRoad(out RaycastOutput raycastOutput)
        {
            RaycastInput raycastInput = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            raycastInput.m_netService.m_service = ItemClass.Service.Road;
            raycastInput.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
            raycastInput.m_ignoreNodeFlags = NetNode.Flags.None;
            raycastInput.m_ignoreTerrain = true;

            return RayCast(raycastInput, out raycastOutput);
        }
    }
}
