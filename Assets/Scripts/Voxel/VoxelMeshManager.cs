using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Essence.Voxel
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class VoxelMeshManager : MonoBehaviour
    {
        //public GameObject cube;

        public Mesh mesh;

        [Header("MESH DIMENSIONS (in voxels)")]
        [Min(1)]
        public Vector3Int dimensions;

        [Header("MESH OFFSET (in units)")]
        public Vector3 offset;

        [Header("VOXEL DATA")]
        public float voxelSize = 1;
        public float voxelHalfSize => voxelSize * 0.5f;

        public VoxelData[] voxels;

        public List<Vector3> vertices;
        public List<int> triangles;

        [Header("OPTIONS")]
        public bool optimiseMesh;

        private float worldToGrid => 1 / voxelSize;

        private void Awake()
        {
            GenerateData();
            GenerateVoxels();
            if (optimiseMesh) OptimiseVertices();
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
            int index = 0;
            for (int z = 0; z < dimensions.z; z++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    for (int x = 0; x < dimensions.x; x++)
                    {
                        VoxelData newVoxel = new VoxelData(x, y, z, this);
                        voxels[index] = newVoxel;

                        index++;
                    }
                }
            }
        }

        private void OptimiseVertices()
        {
            vertices.Clear();
            triangles.Clear();

            Debug.Log(voxels.Count());

            List<Vector3> newVerts = new List<Vector3>
            {
                GetVoxel(0,0,0).GetVoxelFaceByFacing(VoxelFacing.Zm).vertices[0],
                GetVoxel(0,2,0).GetVoxelFaceByFacing(VoxelFacing.Zm).vertices[1],
                GetVoxel(4,0,0).GetVoxelFaceByFacing(VoxelFacing.Zm).vertices[2],
                GetVoxel(4,2,0).GetVoxelFaceByFacing(VoxelFacing.Zm).vertices[3]
            };

            List<int> newTris = new List<int>
            {
                0, 1, 2, 2, 1, 3
            };

            vertices = newVerts;
            triangles = newTris;
        }

        private void GenerateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();

            GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        private VoxelData GetVoxel(int x, int y, int z)
        {
            int index = x + dimensions.x * (y + dimensions.y * z);
            Debug.Log(index);
            return voxels[index];
        }

        public void LocateVoxel(Vector3 worldPosition)
        {
            var startPos = transform.position + offset;

            var distanceX = Mathf.Abs(worldPosition.x - startPos.x + 0.01f);
            var distanceY = Mathf.Abs(worldPosition.y - startPos.y + 0.01f);
            var distanceZ = Mathf.Abs(worldPosition.z - startPos.z + 0.01f);

            Debug.Log($"ABS: {distanceX}, {distanceY}, {distanceZ}");

            Debug.Log
                (
                    $"ROUNDED: " +
                    $"{Mathf.Round(distanceX * worldToGrid)}, " +
                    $"{Mathf.Round(distanceY * worldToGrid)}, " +
                    $"{Mathf.Round(distanceZ * worldToGrid)}"
                );
        }
    }
}
