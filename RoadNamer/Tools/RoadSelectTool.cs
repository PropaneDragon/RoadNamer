using ColossalFramework;
using UnityEngine;
using RoadNamer.Managers;
using ColossalFramework.UI;
using System;

namespace RoadNamer.Tools
{
    class RoadSelectTool : DefaultTool
    {
        protected override void Awake()
        {
            base.Awake();

            Debug.Log("Tool awake");
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
            base.OnToolUpdate();
            RaycastOutput raycastOutput;

            if (RaycastRoad(out raycastOutput))
            {
                ushort netSegmentId = raycastOutput.m_netSegment;

                if(netSegmentId != 0)
                {
                    InstanceManager instanceManager = Singleton<InstanceManager>.instance;

                    NetManager netManager = Singleton<NetManager>.instance;
                    NetSegment netSegment = netManager.m_segments.m_buffer[(int)netSegmentId];
                    string roadName = RoadNameManager.GetRoadName(netSegmentId);

                    if(roadName != null)
                    {
                        Debug.Log(roadName);
                    }
                    else
                    {
                        RoadNameManager.SetRoadName(netSegmentId, "Test Road <color#ff6666>" + netSegmentId.ToString() + "</color>");
                    }
                }
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            
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
