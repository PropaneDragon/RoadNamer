using ColossalFramework;
using UnityEngine;
using RoadNamer.Managers;
using ColossalFramework.UI;
using System;

namespace RoadNamer.Tools
{
    class RoadSelectTool : DefaultTool
    {
        private ushort m_lastSegmentId = 0;
        public UIFont m_uiFont = null;

        private Mesh m_nameMesh = null;
        private Mesh m_iconMesh = null;

        private Material m_nameMaterial = null;
        private Material m_iconMaterial = null;

        protected override void Awake()
        {
            base.Awake();

            Debug.Log("Tool awake");

            //m_uiFont = UIDynamicFont.FindByName("Calibri");
        }

        protected override void OnToolGUI()
        {
            base.OnToolGUI();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_uiFont != null)
            {
                DistrictManager districtManager = Singleton<DistrictManager>.instance;

                this.m_nameMaterial = new Material(districtManager.m_properties.m_areaNameShader);
                this.m_nameMaterial.CopyPropertiesFromMaterial(districtManager.m_properties.m_areaNameFont.material);

                this.m_iconMaterial = new Material(districtManager.m_properties.m_areaIconShader);
                this.m_iconMaterial.CopyPropertiesFromMaterial(districtManager.m_properties.m_areaIconAtlas.material);
            }
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
                    m_lastSegmentId = netSegmentId;

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
                        RoadNameManager.SetRoadName(netSegmentId, "Test Road " + netSegmentId.ToString());
                    }
                }
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (m_uiFont != null)
            {
                UIFontManager.Invalidate(m_uiFont);
                NetManager netManager = Singleton<NetManager>.instance;
                NetSegment[] netSegments = netManager.m_segments.m_buffer;
                UIRenderData uiRenderData = UIRenderData.Obtain();
                UIRenderData dynamicFontRenderData = UIRenderData.Obtain();

                Debug.Log("Rendering");

                try
                {
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

                    int originalNormalCount = normals.Count;
                    int originalDynamicFontNormalCount = dynamicFontNormals.Count;

                    if (m_lastSegmentId != 0)
                    {
                        string roadName = RoadNameManager.GetRoadName(m_lastSegmentId);

                        if (roadName != null)
                        {
                            NetSegment netSegment = netManager.m_segments.m_buffer[m_lastSegmentId];
                            NetSegment.Flags segmentFlags = netSegment.m_flags;

                            if (segmentFlags.IsFlagSet(NetSegment.Flags.Created))
                            {
                                Debug.Log("Rendering 2 " + roadName);

                                UIFontRenderer fontRenderer = m_uiFont.ObtainRenderer();
                                UIDynamicFont.DynamicFontRenderer dynamicFontRenderer = fontRenderer as UIDynamicFont.DynamicFontRenderer;

                                if (dynamicFontRenderer != null)
                                {
                                    //dynamicFontRenderer.spriteAtlas = this.m_properties.m_areaIconAtlas;
                                    dynamicFontRenderer.spriteBuffer = dynamicFontRenderData;
                                }

                                fontRenderer.defaultColor = new Color32(255, 255, 255, 64);
                                fontRenderer.textScale = 2f;
                                fontRenderer.pixelRatio = 1f;
                                fontRenderer.wordWrap = true;
                                fontRenderer.multiLine = true;
                                fontRenderer.textAlign = UIHorizontalAlignment.Center;
                                fontRenderer.maxSize = new Vector2(450f, 900f);

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

                                Debug.Log("First");

                                for (int normalCount = originalNormalCount; normalCount < normals.Count; ++normalCount)
                                {
                                    normals[normalCount] = segmentLocation;
                                }

                                Debug.Log("Second");

                                for (int normalCount = normals.Count; normalCount < vertices.Count; ++normalCount)
                                {
                                    normals.Add(segmentLocation);
                                }

                                Debug.Log("Third");

                                for (int normalCount = originalDynamicFontNormalCount; normalCount < dynamicFontNormals.Count; ++normalCount)
                                {
                                    dynamicFontNormals[normalCount] = segmentLocation;
                                }

                                Debug.Log("Fourth");

                                for (int normalCount = normals.Count; normalCount < dynamicFontVertices.Count; ++normalCount)
                                {
                                    dynamicFontNormals.Add(segmentLocation);
                                }

                                Debug.Log("Rendering 3 " + roadName + " at " + segmentLocation.ToString());
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

                    if (this.m_nameMesh != null && this.m_nameMaterial != null)
                    {
                        this.m_nameMaterial.color = new Color(255, 255, 255, 255);
                        if (this.m_nameMaterial.SetPass(0))
                        {
                            DistrictManager expr_5E_cp_0 = Singleton<DistrictManager>.instance;
                            expr_5E_cp_0.m_drawCallData.m_overlayCalls = expr_5E_cp_0.m_drawCallData.m_overlayCalls + 1;
                            Graphics.DrawMeshNow(this.m_nameMesh, Matrix4x4.identity);
                        }
                    }

                    if (this.m_iconMesh != null && this.m_iconMaterial != null && this.m_iconMaterial.SetPass(0))
                    {
                        DistrictManager expr_B8_cp_0 = Singleton<DistrictManager>.instance;
                        expr_B8_cp_0.m_drawCallData.m_overlayCalls = expr_B8_cp_0.m_drawCallData.m_overlayCalls + 1;
                        Graphics.DrawMeshNow(this.m_iconMesh, Matrix4x4.identity);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message + "\n\n" + ex.StackTrace.ToString() + "\n" + ex.Source);
                }

                uiRenderData.Release();
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
