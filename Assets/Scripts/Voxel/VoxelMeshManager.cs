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

        public System.Action initComplete;

        private MeshCollider meshCollider;

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

            meshCollider = GetComponent<MeshCollider>();

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

            initComplete?.Invoke();
        }

        private void OptimiseVertices()
        {
            vertices.Clear();
            triangles.Clear();

            Debug.Log(voxels.Count());

            List<Vector3> newVerts = new List<Vector3>
            {
                GetVoxel(0,0,0).GetFacebyFacing(VoxelFacing.Zm).vertices[0],
                GetVoxel(0,dimensions.y - 1,0).GetFacebyFacing(VoxelFacing.Zm).vertices[1],
                GetVoxel(dimensions.x - 1,0,0).GetFacebyFacing(VoxelFacing.Zm).vertices[2],
                GetVoxel(dimensions.x - 1,dimensions.y - 1,0).GetFacebyFacing(VoxelFacing.Zm).vertices[3]
            };

            List<int> newTris = new List<int>
            {
                0, 1, 2, 2, 1, 3
            };

            vertices = newVerts;
            triangles = newTris;
        }

        private void FindVerticesForMesh()
        {

        }

        private void GenerateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();

            if (vertices.Count > 0) meshCollider.sharedMesh = mesh;
            else meshCollider.enabled = false;
        }

        public VoxelData GetVoxel(int x, int y, int z)
        {
            int index = x + dimensions.x * (y + dimensions.y * z);

            if (index >= voxels.Length || index < 0) return null;

            return voxels[index];
        }

        public VoxelData GetVoxel(Vector3Int coords)
        {
            return GetVoxel(coords.x, coords.y, coords.z);
        }

        public VoxelData LocateVoxel(Vector3 worldPosition)
        {
            var startPos = transform.position + offset;

            var distanceX = Mathf.Abs(worldPosition.x - startPos.x);
            var distanceY = Mathf.Abs(worldPosition.y - startPos.y);
            var distanceZ = Mathf.Abs(worldPosition.z - startPos.z);

            Debug.Log($"ABS: {distanceX}, {distanceY}, {distanceZ}");

            int x = Mathf.RoundToInt(distanceX * worldToGrid);
            int y = Mathf.RoundToInt(distanceY * worldToGrid);
            int z = Mathf.RoundToInt(distanceZ * worldToGrid);

            Debug.Log($"ROUNDED: {x}, {y}, {z}");

            return GetVoxel(x, y, z);
        }

        public void DisableVoxel(Vector3 worldPosition)
        {
            VoxelData target = LocateVoxel(worldPosition);

            target.rendering = false;

            //target.DisableFaces();

            RefreshRenderData();
            GenerateMesh();
        }

        private void RefreshRenderData()
        {
            vertices.Clear();
            triangles.Clear();

            foreach (var voxel in voxels)
            {
                voxel.SupplyRenderData();
            }
        }
    }
}
