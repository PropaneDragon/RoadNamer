using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoadNamer.Managers
{
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

            if (cameraInfo.m_height < m_renderHeight)
            {
                DrawMesh();
            }
        }

        private void DrawMesh()
        {
            DistrictManager districtManager = Singleton<DistrictManager>.instance;

            if (districtManager.NamesVisible || m_alwaysShowText) //Camera mode - It only gets set in Cities classes, so I can't really get it any other way
            {
                if (this.m_nameMesh != null && this.m_nameMaterial != null)
                {
                    this.m_nameMaterial.color = districtManager.m_properties.m_areaNameColor;
                    if (this.m_nameMaterial.SetPass(0))
                    {
                        Graphics.DrawMeshNow(this.m_nameMesh, Matrix4x4.identity);
                    }
                }

                if (this.m_iconMesh != null && this.m_iconMaterial != null && this.m_iconMaterial.SetPass(0))
                {
                    Graphics.DrawMeshNow(this.m_iconMesh, Matrix4x4.identity);
                }
            }
        }

        private void RenderText()
        {
            DistrictManager districtManager = Singleton<DistrictManager>.instance;

            if (districtManager.m_properties.m_areaNameFont != null)
            {
                int lastFontSize = districtManager.m_properties.m_areaNameFont.size;

                UIFontManager.Invalidate(districtManager.m_properties.m_areaNameFont);
                UIRenderData uiRenderData = UIRenderData.Obtain();
                UIRenderData dynamicFontRenderData = UIRenderData.Obtain();

                NetManager netManager = Singleton<NetManager>.instance;
                NetSegment[] netSegments = netManager.m_segments.m_buffer;

                uiRenderData.Clear();
                dynamicFontRenderData.Clear();

                PoolList<Vector3> vertices = uiRenderData.vertices;
                PoolList<Vector3> normals = uiRenderData.normals;
                PoolList<Color32> colors = uiRenderData.colors;
                PoolList<Vector2> uvs = uiRenderData.uvs;
                PoolList<int> triangles = uiRenderData.triangles;

                PoolList<Vector3> dynamicFontVertices = dynamicFontRenderData.vertices;
                PoolList<Vector3> dynamicFontNormals = dynamicFontRenderData.normals;
                PoolList<Color32> dynamicFontColors = dynamicFontRenderData.colors;
                PoolList<Vector2> dynamicFontUvs = dynamicFontRenderData.uvs;
                PoolList<int> dynamicFontTriangles = dynamicFontRenderData.triangles;

                foreach (RoadContainer road in RoadNameManager.Instance().m_roadList)
                {
                    if (road.m_segmentId != 0)
                    {
                        string roadName = road.m_roadName;

                        if (roadName != null)
                        {
                            int originalNormalCount = normals.Count;
                            int originalDynamicFontNormalCount = dynamicFontNormals.Count;

                            NetSegment netSegment = netManager.m_segments.m_buffer[road.m_segmentId];
                            NetSegment.Flags segmentFlags = netSegment.m_flags;

                            if (segmentFlags.IsFlagSet(NetSegment.Flags.Created))
                            {
                                UIFontRenderer fontRenderer = districtManager.m_properties.m_areaNameFont.ObtainRenderer();
                                UIDynamicFont.DynamicFontRenderer dynamicFontRenderer = fontRenderer as UIDynamicFont.DynamicFontRenderer;

                                if (dynamicFontRenderer != null)
                                {
                                    dynamicFontRenderer.spriteAtlas = districtManager.m_properties.m_areaIconAtlas;
                                    dynamicFontRenderer.spriteBuffer = dynamicFontRenderData;
                                }

                                fontRenderer.defaultColor = new Color32(255, 255, 255, 64);
                                fontRenderer.textScale = m_textScale;
                                fontRenderer.pixelRatio = 1f;
                                fontRenderer.processMarkup = true;
                                fontRenderer.wordWrap = true;
                                fontRenderer.multiLine = true;
                                fontRenderer.textAlign = UIHorizontalAlignment.Center;
                                fontRenderer.maxSize = new Vector2(450f, 900f);
                                fontRenderer.shadow = false;
                                fontRenderer.shadowColor = (Color32)Color.black;
                                fontRenderer.shadowOffset = Vector2.one;

                                Vector2 stringSize = fontRenderer.MeasureString(roadName);

                                vertices.Add(new Vector3(-stringSize.x, -stringSize.y, 1f));
                                vertices.Add(new Vector3(-stringSize.x, stringSize.y, 1f));
                                vertices.Add(new Vector3(stringSize.x, stringSize.y, 1f));
                                vertices.Add(new Vector3(stringSize.x, -stringSize.y, 1f));

                                colors.Add(new Color32(0, 0, 0, 255));
                                colors.Add(new Color32(0, 0, 0, 255));
                                colors.Add(new Color32(0, 0, 0, 255));
                                colors.Add(new Color32(0, 0, 0, 255));

                                uvs.Add(new Vector2(-1f, -1f));
                                uvs.Add(new Vector2(-1f, 1f));
                                uvs.Add(new Vector2(1f, 1f));
                                uvs.Add(new Vector2(1f, -1f));

                                triangles.Add(vertices.Count - 4);
                                triangles.Add(vertices.Count - 3);
                                triangles.Add(vertices.Count - 1);
                                triangles.Add(vertices.Count - 1);
                                triangles.Add(vertices.Count - 3);
                                triangles.Add(vertices.Count - 2);

                                fontRenderer.vectorOffset = new Vector3(-225f, stringSize.y * 0.5f, 0f);
                                fontRenderer.Render(roadName, uiRenderData);

                                Vector3 segmentLocation = netSegment.m_bounds.center;

                                for (int normalCount = originalNormalCount; normalCount < normals.Count; ++normalCount)
                                {
                                    normals[normalCount] = segmentLocation;
                                }

                                for (int normalCount = normals.Count; normalCount < vertices.Count; ++normalCount)
                                {
                                    normals.Add(segmentLocation);
                                }

                                for (int normalCount = originalDynamicFontNormalCount; normalCount < dynamicFontNormals.Count; ++normalCount)
                                {
                                    dynamicFontNormals[normalCount] = segmentLocation;
                                }

                                for (int normalCount = normals.Count; normalCount < dynamicFontVertices.Count; ++normalCount)
                                {
                                    dynamicFontNormals.Add(segmentLocation);
                                }
                            }
                        }
                    }
                }

                if (this.m_nameMesh == null)
                {
                    this.m_nameMesh = new Mesh();
                }

                this.m_nameMesh.Clear();
                this.m_nameMesh.vertices = vertices.ToArray();
                this.m_nameMesh.normals = normals.ToArray();
                this.m_nameMesh.colors32 = colors.ToArray();
                this.m_nameMesh.uv = uvs.ToArray();
                this.m_nameMesh.triangles = triangles.ToArray();
                this.m_nameMesh.bounds = new Bounds(Vector3.zero, new Vector3(9830.4f, 1024f, 9830.4f));

                if (this.m_iconMesh == null)
                {
                    this.m_iconMesh = new Mesh();
                }

                this.m_iconMesh.Clear();
                this.m_iconMesh.vertices = dynamicFontVertices.ToArray();
                this.m_iconMesh.normals = dynamicFontNormals.ToArray();
                this.m_iconMesh.colors32 = dynamicFontColors.ToArray();
                this.m_iconMesh.uv = dynamicFontUvs.ToArray();
                this.m_iconMesh.triangles = dynamicFontTriangles.ToArray();
                this.m_iconMesh.bounds = new Bounds(Vector3.zero, new Vector3(9830.4f, 1024f, 9830.4f));

                uiRenderData.Release();
            }
        }

        public void ForceUpdate()
        {
            m_lastCount = -1;
        }
    }
}
