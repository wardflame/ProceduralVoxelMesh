using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ProceduralGridMesh : MonoBehaviour
    {
        private Mesh mesh;
        private Vector3[] vertices;
        private int[] triangles;

        public float cellSize = 1;
        public Vector3 gridOffset;
        public int gridSize;

        private void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }

        private void Start()
        {
            MakeContiguousProceduralGrid();
            UpdateMesh();
        }

        private void MakeDiscreteProceduralGrid()
        {
            vertices = new Vector3[gridSize * gridSize * 4];
            triangles = new int[gridSize * gridSize * 6];

            int v = 0;
            int t = 0;

            float vertexOffset = cellSize * 0.5f;

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    Vector3 cellOffset = new Vector3(x * cellSize, y * cellSize, 0);

                    vertices[v + 0] = new Vector3(-vertexOffset, -vertexOffset, 0) + gridOffset + cellOffset;
                    vertices[v + 1] = new Vector3(-vertexOffset, vertexOffset, 0) + gridOffset + cellOffset;
                    vertices[v + 2] = new Vector3(vertexOffset, -vertexOffset, 0) + gridOffset + cellOffset;
                    vertices[v + 3] = new Vector3(vertexOffset, vertexOffset, 0) + gridOffset + cellOffset;

                    triangles[t + 0] = v + 0;
                    triangles[t + 1] = triangles[t + 4] = v + 1;
                    triangles[t + 2] = triangles[t + 3] = v + 2;
                    triangles[t +5] = v + 3;

                    v += 4;
                    t += 6;
                }
            }
        }

        private void MakeContiguousProceduralGrid()
        {
            vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
            triangles = new int[gridSize * gridSize * 6];

            int v = 0;
            int t = 0;

            float vertexOffset = cellSize * 0.5f;

            for (int x = 0; x <= gridSize; x++)
            {
                for (int y = 0; y <= gridSize; y++)
                {
                    vertices[v] = new Vector3
                        (
                            (x * cellSize) - vertexOffset,
                            (y * cellSize) - vertexOffset,
                            0
                        );

                    v++;
                }
            }

            v = 0;

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    triangles[t + 0] = v + 0;
                    triangles[t + 1] = triangles[t + 4] = v + 1;
                    triangles[t + 2] = triangles[t + 3] = v + (gridSize + 1);
                    triangles[t + 5] = v + (gridSize + 1) + 1;
                    v++;
                    t += 6;
                }
                v++;
            }
        }

        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
        }
    }
}
