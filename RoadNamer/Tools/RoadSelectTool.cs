using ColossalFramework;
using UnityEngine;
using RoadNamer.Managers;
using ColossalFramework.UI;

namespace RoadNamer.Tools
{
    class RoadSelectTool : DefaultTool
    {
        private ushort m_lastSegmentId = 0;

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

            NetManager netManager = Singleton<NetManager>.instance;
            NetSegment[] netSegments = netManager.m_segments.m_buffer;
            UIRenderData uiRenderData = UIRenderData.Obtain();

            Debug.Log("Rendering");

            try
            {
                uiRenderData.Clear();

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

                            PoolList<Vector3> vertices = uiRenderData.vertices;
                            PoolList<Vector3> normals = uiRenderData.normals;
                            PoolList<Color32> colors = uiRenderData.colors;
                            PoolList<Vector2> uvs = uiRenderData.uvs;
                            PoolList<int> triangles = uiRenderData.triangles;

                            int originalNormalCount = normals.Count;

                            UIDynamicFont font = ScriptableObject.CreateInstance<UIDynamicFont>();
                            UIFontRenderer fontRenderer = font.ObtainRenderer();

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

                            for(int normalCount = originalNormalCount; normalCount < normals.Count; ++normalCount)
                            {
                                normals[normalCount] = segmentLocation;
                            }

                            for(int normalCount = normals.Count; normalCount < vertices.Count; ++normalCount)
                            {
                                normals.Add(segmentLocation);
                            }

                            Debug.Log("Rendering 3 " + roadName + " at " + segmentLocation.ToString());
                        }
                    }
                }
            }
            catch
            {

            }

            uiRenderData.Release();
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
