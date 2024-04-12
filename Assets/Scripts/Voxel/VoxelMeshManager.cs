using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Essence.Voxel
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class VoxelMeshManager : MonoBehaviour
    {
        //public GameObject cube;

        public Mesh mesh;

        [Header("MESH DIMENSIONS (in voxels)")]
        public Vector3Int dimensions;

        [Header("VOXEL DATA")]
        public float voxelSize = 1;
        public float voxelHalfSize => voxelSize * 0.5f;

        public VoxelData[] voxels;

        public List<Vector3> vertices;
        public List<int> triangles;

        private void Awake()
        {
            GenerateData();
            GenerateVoxels();
            GenerateMesh();
        }

        private void GenerateData()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            mesh.name = "Procedural Voxel Mesh";

            voxels = new VoxelData[dimensions.x * dimensions.y * dimensions.z];
            vertices = new List<Vector3>();
            triangles = new List<int>();
        }

        private void GenerateVoxels()
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    for (int z = 0; z < dimensions.z; z++)
                    {
                        VoxelData newVoxel = new VoxelData(x, y, z, this);
                        voxels[x + dimensions.y * y + dimensions.z * z] = newVoxel;

                        //vertices.AddRange(newVoxel.vertices);
                        //triangles.AddRange(newVoxel.triangles);
                    }
                }
            }
        }

        private void GenerateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();

            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }
}
