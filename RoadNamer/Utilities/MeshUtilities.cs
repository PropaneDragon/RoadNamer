using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadNamer.Utilities
{
    public static class MeshUtilities
    {
        public static Mesh CreateRectMesh(float width, float height)
        {

            Mesh mesh = new Mesh();
            float maxValue = width > height ? width : height;

            width = (width / (maxValue)) * 4;
            height = (height / (maxValue)) * 4;
            Vector3[] vertices = new Vector3[]
            {
            new Vector3( width, height,  0),
            new Vector3( width, -height, 0),
            new Vector3(-width, height, 0),
            new Vector3(-width, -height, 0),
            };

            Vector2[] uv = new Vector2[]
            {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 0),
            };

            int[] triangles = new int[]
            {
            0, 1, 2,
            2, 1, 3,
            };

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }

}
