using ColossalFramework;
using ColossalFramework.UI;
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
        private Mesh m_nameMesh = null;
        private Mesh m_iconMesh = null;

        private Material m_nameMaterial = null;
        private Material m_iconMaterial = null;

        private int m_lastCount = 0;

        public float m_renderHeight = 1000f;
        public float m_textScale = 0.5f;
        public bool m_alwaysShowText = false;
        public bool m_registered = false;
        public bool m_textEnabled = true;

        protected override void Awake()
        {
            base.Awake();

            Debug.Log("Road Namer: Initialising RoadRenderingManager");

            DistrictManager districtManager = Singleton<DistrictManager>.instance;

            this.m_nameMaterial = new Material(districtManager.m_properties.m_areaNameShader);
            this.m_nameMaterial.CopyPropertiesFromMaterial(districtManager.m_properties.m_areaNameFont.material);

            this.m_iconMaterial = new Material(districtManager.m_properties.m_areaIconShader);
            this.m_iconMaterial.CopyPropertiesFromMaterial(districtManager.m_properties.m_areaIconAtlas.material);
        }

        protected override void BeginOverlayImpl(RenderManager.CameraInfo cameraInfo)
        {
            if (m_lastCount != RoadNameManager.Instance().m_roadList.Count)
            {
                m_lastCount = RoadNameManager.Instance().m_roadList.Count;

                try
                {
                    RenderText();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
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
                //NetSegment[] netSegments = netManager.m_segments.m_buffer;

                foreach (RoadContainer road in RoadNameManager.Instance().m_roadList)
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
                                Vector3 segmentLocation = netSegment.m_bounds.center;
                                
                                GameObject Text = new GameObject();
                                TextMesh testTextMesh = Text.AddComponent<TextMesh>();
                                testTextMesh.anchor = TextAnchor.MiddleCenter;
                                testTextMesh.font = districtManager.m_properties.m_areaNameFont.baseFont;
                                testTextMesh.GetComponent<Renderer>().material = testTextMesh.font.material;
                                testTextMesh.fontSize = 20;
                                testTextMesh.transform.position = segmentLocation;
                                testTextMesh.transform.rotation = netSegment.Info.transform.rotation * Quaternion.Euler(90, 0, 90);
                                testTextMesh.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                                testTextMesh.richText = true;
                                testTextMesh.text = roadName;
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
