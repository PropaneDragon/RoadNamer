using ColossalFramework;
using UnityEngine;
using RoadNamer.Managers;
using ColossalFramework.UI;
using System;
using RoadNamer.Panels;
using RoadNamer.Utilities;
using ColossalFramework.Math;

namespace RoadNamer.Tools
{
    public class RoadSelectTool : DefaultTool
    {
        NetManager netManager = NetManager.instance;

        public RoadNamePanel m_roadNamePanel = null;
        public UsedNamesPanel m_usedNamesPanel = null;

        ushort netSegmentId = 0;
        private Randomizer r1;

        protected override void Awake()
        {
            LoggerUtilities.Log("Tool awake");

            base.Awake();
        }

        protected override void OnToolGUI(UnityEngine.Event e)
        {
            base.OnToolGUI(e);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBusManager.Instance().Publish("closeAll", null);
        }

        protected override void OnToolUpdate()
        {
            if (m_toolController != null && !m_toolController.IsInsideUI && Cursor.visible)
            {
                RaycastOutput raycastOutput;

                if (RaycastRoad(out raycastOutput))
                {
                    netSegmentId = raycastOutput.m_netSegment;

                    if (netSegmentId != 0)
                    {
                        NetSegment netSegment = netManager.m_segments.m_buffer[netSegmentId];

                        if (netSegment.m_flags.IsFlagSet(NetSegment.Flags.Created))
                        {
                            if (Event.current.type == EventType.MouseDown && Event.current.button == (int)UIMouseButton.None)
                            {
                                ShowToolInfo(false, null, new Vector3());

                                if (m_roadNamePanel != null)
                                {
                                    RandomNameManager.LoadRandomNames();
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
                            else if(Event.current.type == EventType.MouseDown && Event.current.button == (int)UIMouseButton.Left)
                            {
                                RoadNameManager.Instance().DelRoadName(netSegmentId);
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

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            Randomizer r2 = new Randomizer((int)Singleton<PropManager>.instance.m_props.NextFreeItem(ref r1));

            if (netSegmentId != 0 && (!this.m_toolController.IsInsideUI && Cursor.visible))
            {
                Color toolColor = this.GetToolColor(false, true);
                RenderOverlay(cameraInfo, netSegmentId, toolColor);
            }
            base.RenderOverlay(cameraInfo);
        }

        private void RenderOverlay(RenderManager.CameraInfo cameraInfo, ushort netSegmentId, Color toolColor)
        {
            NetSegment segment = netManager.m_segments.m_buffer[netSegmentId];

            NetInfo info = segment.Info;
            if (info == null || (segment.m_flags & NetSegment.Flags.Untouchable) != NetSegment.Flags.None && !info.m_overlayVisible)
                return;
            Bezier3 bezier;
            bezier.a = Singleton<NetManager>.instance.m_nodes.m_buffer[(int)segment.m_startNode].m_position;
            bezier.d = Singleton<NetManager>.instance.m_nodes.m_buffer[(int)segment.m_endNode].m_position;
            NetSegment.CalculateMiddlePoints(bezier.a, segment.m_startDirection, bezier.d, segment.m_endDirection, false, false, out bezier.b, out bezier.c);
            bool flag1 = false;
            bool flag2 = false;
            Color color = this.GetToolColor(false, true);
            ++ToolManager.instance.m_drawCallData.m_overlayCalls;
            RenderManager.instance.OverlayEffect.DrawBezier(cameraInfo, color, bezier, info.m_halfWidth * 2f, !flag1 ? -100000f : info.m_halfWidth, !flag2 ? -100000f : info.m_halfWidth, -1f, 1280f, false, false);
        }

        bool RaycastRoad(out RaycastOutput raycastOutput)
        {
            RaycastInput raycastInput = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            raycastInput.m_netService.m_service = ItemClass.Service.Road;
            raycastInput.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.PublicTransport | ItemClass.Layer.WaterPipes;
            raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
            raycastInput.m_ignoreNodeFlags = NetNode.Flags.None;
            raycastInput.m_ignoreBuildingFlags = Building.Flags.None;
            raycastInput.m_ignoreTerrain = true;

            return RayCast(raycastInput, out raycastOutput);
        }
    }
}
