using Codice.Client.Common.FsNodeReaders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence.Meshing
{
    [RequireComponent(typeof(MeshFilter))]
    public class BitsProceduralMesh : MonoBehaviour
    {
        private Mesh mesh;

        private Vector3[] vertices;
        private int[] triangles;

        private void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }

        // Start is called before the first frame update
        void Start()
        {
            MakeMeshData();
            CreateMesh();
        }

        private void MakeMeshData()
        {
            vertices = new Vector3[]
            {
                // Quad 1
                new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(1,0,0),
                new Vector3(1,1,0)
            };

            triangles = new int[]
            {
                // Triangle 1
                0, 1, 2, 2, 1, 3
            };
        }

        private void CreateMesh()
        {
            mesh.Clear();
            mesh.name = "BitsMesh";
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
    }
}
