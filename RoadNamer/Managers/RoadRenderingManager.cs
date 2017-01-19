using ColossalFramework;
using ColossalFramework.UI;
using RoadNamer.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoadNamer.Managers
{
    /// <summary>
    /// Handles all ingame rendering.
    /// </summary>
    public class RoadRenderingManager : SimulationManagerBase<RoadRenderingManager, DistrictProperties>, IRenderableManager, ISimulationManager
    {
        private Material m_nameMaterial = null;
        private Material m_iconMaterial = null;

        private int m_lastCount = 0;
        private bool textHidden = false;

        public float m_renderHeight = 1000f;
        public float m_textScale = 0.5f;
        public float m_textQuality = 20f;
        public float m_textHeightOffset = -2f;
        public bool m_alwaysShowText = false;
        public bool m_registered = false;
        public bool m_textEnabled = true;
        public bool m_routeEnabled = true;

        protected override void Awake()
        {
            base.Awake();

            LoggerUtilities.Log("Initialising RoadRenderingManager");

            DistrictManager districtManager = Singleton<DistrictManager>.instance;

            this.m_nameMaterial = new Material(districtManager.m_properties.m_areaNameShader);
            this.m_nameMaterial.CopyPropertiesFromMaterial(districtManager.m_properties.m_areaNameFont.material);

            this.m_iconMaterial = new Material(districtManager.m_properties.m_areaIconShader);
            this.m_iconMaterial.CopyPropertiesFromMaterial(districtManager.m_properties.m_areaIconAtlas.material);
        }

        protected override void BeginOverlayImpl(RenderManager.CameraInfo cameraInfo)
        {
            DistrictManager districtManager = Singleton<DistrictManager>.instance;

            if (m_lastCount != RoadNameManager.Instance().m_roadDict.Count)
            {
                m_lastCount = RoadNameManager.Instance().m_roadDict.Count;

                try
                {
                    RenderText();
                }
                catch (Exception ex)
                {
                    LoggerUtilities.LogException(ex);
                }
            }

            if (!textHidden && cameraInfo.m_height > m_renderHeight)
            {
                foreach (RoadContainer road in RoadNameManager.Instance().m_roadDict.Values)
                {
                    road.m_textMesh.GetComponent<Renderer>().enabled = false;
                }
                textHidden = true;
            }
            else if (textHidden  && cameraInfo.m_height <= m_renderHeight && (districtManager.NamesVisible || m_alwaysShowText)) //This is a mess, and I'll sort it soon :)
            {
                if (m_textEnabled)
                {
                    foreach (RoadContainer road in RoadNameManager.Instance().m_roadDict.Values)
                    {
                        road.m_textMesh.GetComponent<Renderer>().enabled = true;
                    }
                }
                textHidden = false;
            }
        }

        /// <summary>
        /// Redraw the text to be drawn later with a mesh. Use sparingly, as 
        /// this is an expensive task.
        /// </summary>
        private void RenderText()
        {
            DistrictManager districtManager = Singleton<DistrictManager>.instance;

            if (districtManager.m_properties.m_areaNameFont != null)
            {
                UIFontManager.Invalidate(districtManager.m_properties.m_areaNameFont);

                NetManager netManager = Singleton<NetManager>.instance;
                float scaleMultiplier = m_textQuality / 20f;

                foreach (RoadContainer road in RoadNameManager.Instance().m_roadDict.Values)
                {
                    if (road.m_segmentId != 0)
                    {
                        string roadName = road.m_roadName;

                        if (roadName != null)
                        {
                            NetSegment netSegment = netManager.m_segments.m_buffer[road.m_segmentId];
                            NetSegment.Flags segmentFlags = netSegment.m_flags;

                            if (segmentFlags.IsFlagSet(NetSegment.Flags.Created))
                            {
                                NetNode startNode = netManager.m_nodes.m_buffer[netSegment.m_startNode]; //Not used yet, but there just incase. This isn't final
                                NetNode endNode = netManager.m_nodes.m_buffer[netSegment.m_endNode];

                                Vector3 segmentLocation = netSegment.m_bounds.center;
                                road.m_textMesh.anchor = TextAnchor.MiddleCenter;
                                road.m_textMesh.font = districtManager.m_properties.m_areaNameFont.baseFont;
                                road.m_textMesh.GetComponent<Renderer>().material = road.m_textMesh.font.material;
                                road.m_textMesh.GetComponent<Renderer>().receiveShadows = true;
                                road.m_textMesh.fontSize = (int)Math.Round(m_textQuality);
                                road.m_textMesh.transform.position = startNode.m_position;
                                road.m_textMesh.transform.LookAt(endNode.m_position, Vector3.up);
                                road.m_textMesh.transform.Rotate(90f, 0f, 90f);
                                road.m_textMesh.transform.position = (startNode.m_position + endNode.m_position) / 2f;
                                road.m_textMesh.transform.localScale = new Vector3(m_textScale / scaleMultiplier, m_textScale / scaleMultiplier, m_textScale / scaleMultiplier); road.m_textMesh.offsetZ = m_textHeightOffset;
                                road.m_textMesh.offsetZ = m_textHeightOffset;
                                road.m_textMesh.richText = true;
                                road.m_textMesh.text = roadName.Replace("color#", "color=#"); //Convert from Colossal to Unity tags
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Forces rendering to update immediately. Use sparingly, as it
        /// can be quite expensive.
        /// </summary>
        public void ForceUpdate()
        {
            m_lastCount = -1;
        }
    }
}
