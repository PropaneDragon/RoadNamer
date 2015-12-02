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
                foreach( RouteContainer route in RoadNameManager.Instance().m_routeMap.Values )
                {
                    route.m_shieldMesh.GetComponent<Renderer>().enabled = false;
                    route.m_numMesh.GetComponent<Renderer>().enabled = false;
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
                if (m_routeEnabled)
                {
                    foreach (RouteContainer route in RoadNameManager.Instance().m_routeMap.Values)
                    {
                        route.m_shieldMesh.GetComponent<Renderer>().enabled = true;
                        route.m_numMesh.GetComponent<Renderer>().enabled = true;
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
                                road.m_textMesh.fontSize = (int)Math.Round(m_textQuality);
                                road.m_textMesh.transform.position = startNode.m_position;
                                road.m_textMesh.transform.LookAt(endNode.m_position, Vector3.up);
                                Vector3 rotation = Vector3.Cross(startNode.m_position, endNode.m_position);
                                road.m_textMesh.transform.Rotate(90f, 0f, 90f);
                                road.m_textMesh.transform.position = (startNode.m_position + endNode.m_position) / 2f;
                                road.m_textMesh.transform.localScale = new Vector3(m_textScale / scaleMultiplier, m_textScale / scaleMultiplier, m_textScale / scaleMultiplier);
                                road.m_textMesh.offsetZ = m_textHeightOffset;
                                road.m_textMesh.richText = true;
                                road.m_textMesh.text = roadName.Replace("color#", "color=#"); //Convert from Colossal to Unity tags
                            }
                        }
                    }
                }
                foreach (RouteContainer route in RoadNameManager.Instance().m_routeMap.Values)
                {
                    if (route.m_segmentId != 0)
                    {
                        int routeNum = route.m_routeNum;

                        if (routeNum != 0)
                        {
                            NetSegment netSegment = netManager.m_segments.m_buffer[route.m_segmentId];
                            NetSegment.Flags segmentFlags = netSegment.m_flags;

                            if (segmentFlags.IsFlagSet(NetSegment.Flags.Created))
                            {
                                NetNode startNode = netManager.m_nodes.m_buffer[netSegment.m_startNode]; //Not used yet, but there just incase. This isn't final
                                NetNode endNode = netManager.m_nodes.m_buffer[netSegment.m_endNode];
                                //TODO: Make texture addition/selection based on prefix type
                                Material mat = SpriteUtilities.m_textureStore["ON"];
                                route.m_shieldObject.GetComponent<Renderer>().material = mat;
                                //TODO: Make mesh size dependent on text size
                                route.m_shieldMesh.mesh = MeshUtilities.CreateRectMesh(mat.mainTexture.width, mat.mainTexture.height);
                                Vector3 startNodePosition = startNode.m_position;
                                route.m_shieldMesh.transform.position = startNodePosition;
                                route.m_shieldMesh.transform.LookAt(endNode.m_position, Vector3.up);
                                route.m_shieldMesh.transform.Rotate(90f, 0f, 90f);
                                //TODO: Bind the elevation of the mesh to the text z offset
                                route.m_shieldMesh.transform.position += (Vector3.up * (0.5f));
                                route.m_shieldMesh.transform.localScale = new Vector3(m_textScale / scaleMultiplier, m_textScale / scaleMultiplier, m_textScale / scaleMultiplier);
                                route.m_shieldObject.GetComponent<Renderer>().sortingOrder = 1000;

                                route.m_numMesh.anchor = TextAnchor.MiddleCenter;
                                route.m_numMesh.font = districtManager.m_properties.m_areaNameFont.baseFont;
                                route.m_numMesh.GetComponent<Renderer>().material = route.m_numMesh.font.material;
                                //TODO: Tie the font size to the font size option
                                route.m_numMesh.fontSize = 50;
                                route.m_numMesh.transform.position = startNode.m_position;
                                route.m_numMesh.transform.parent = route.m_shieldObject.transform;
                                route.m_numMesh.transform.LookAt(endNode.m_position, Vector3.up);
                                route.m_numMesh.transform.Rotate(90f, 0f, 90f);
                                route.m_numMesh.transform.position = route.m_shieldObject.GetComponent<Renderer>().bounds.center;
                                //Just a hack, to make sure the text actually shows up above the shield
                                route.m_numMesh.offsetZ = 0.001f;
                                //TODO: Definitely get a map of the texture to the required text offsets 
                                route.m_numMesh.transform.localPosition += (Vector3.up * -0.61f);
                                route.m_numMesh.transform.localPosition += (Vector3.left * 0f);
                                route.m_numMesh.transform.localScale = new Vector3(m_textScale / (scaleMultiplier * 2f), m_textScale / (scaleMultiplier * 2f), m_textScale / (scaleMultiplier * 2f));
                                route.m_numMesh.color = Color.black;
                                route.m_numMesh.text = route.m_routeNum.ToString();
                                route.m_numTextObject.GetComponent<Renderer>().sortingOrder = 1001;

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
