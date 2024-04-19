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

        public Voxel[] voxels;

        public List<Vector3> vertices;
        public List<int> triangles;

        [Header("OPTIONS")]
        public bool optimiseMesh;

        private float WorldToGrid => 1 / voxelSize;

        public System.Action initComplete;

        private MeshCollider meshCollider;

        private int renderingVoxels;

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

            voxels = new Voxel[dimensions.x * dimensions.y * dimensions.z];
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
                        Voxel newVoxel = new Voxel(x, y, z, this);
                        voxels[index] = newVoxel;

                        index++;
                    }
                }
            }

            renderingVoxels = voxels.Length;

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
                GetVoxel(dimensions.x - 1,dimensions.y - 1,0).GetFacebyFacing(VoxelFacing.Zm).vertices[3],

                GetVoxel(dimensions.x - 1,0,0).GetFacebyFacing(VoxelFacing.X).vertices[0],
                GetVoxel(dimensions.x - 1,dimensions.y - 1,0).GetFacebyFacing(VoxelFacing.X).vertices[1],
                GetVoxel(dimensions.x - 1,0,dimensions.z - 1).GetFacebyFacing(VoxelFacing.X).vertices[2],
                GetVoxel(dimensions.x - 1,dimensions.y - 1,dimensions.z - 1).GetFacebyFacing(VoxelFacing.X).vertices[3],

                /*GetVoxel(0,0,0).GetFacebyFacing(VoxelFacing.Zm).vertices[0],
                GetVoxel(0,dimensions.y - 1,0).GetFacebyFacing(VoxelFacing.Zm).vertices[1],
                GetVoxel(dimensions.x - 1,0,0).GetFacebyFacing(VoxelFacing.Zm).vertices[2],
                GetVoxel(dimensions.x - 1,dimensions.y - 1,0).GetFacebyFacing(VoxelFacing.Zm).vertices[3],

                GetVoxel(0,0,0).GetFacebyFacing(VoxelFacing.Zm).vertices[0],
                GetVoxel(0,dimensions.y - 1,0).GetFacebyFacing(VoxelFacing.Zm).vertices[1],
                GetVoxel(dimensions.x - 1,0,0).GetFacebyFacing(VoxelFacing.Zm).vertices[2],
                GetVoxel(dimensions.x - 1,dimensions.y - 1,0).GetFacebyFacing(VoxelFacing.Zm).vertices[3]*/
            };

            List<int> newTris = new List<int>
            {
                0, 1, 2, 2, 1, 3,
                4, 5, 6, 6, 5, 7
            };

            vertices = newVerts;
            triangles = newTris;
        }

        private void FindRenderingVoxel()
        {
            // METHOD TO FIND OPTIMISED VERTICES?

            Voxel startVoxel = null;

            for (int z = 0; z < dimensions.z; z++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    for (int x = 0; x < dimensions.x; x++)
                    {
                        startVoxel = GetVoxel(x, y, z);
                        Debug.Log(startVoxel.gridLoc);
                        if (startVoxel != null && startVoxel.rendering)
                        {
                            x = dimensions.x;
                            y = dimensions.y;
                            z = dimensions.z;
                        }
                    }
                }
            }

            Debug.Log($"Start Voxel: {startVoxel.gridLoc}");

            if (startVoxel == null || !startVoxel.rendering) Debug.Log("Couldn't find Voxel");

            List<Voxel> voxelGroup = new();
            var startLoc = startVoxel.gridLoc;
            Voxel nextVoxel = null;

            int xExtent = startLoc.x;
            int yExtent = startLoc.y;
            int zExtent = startLoc.z;

            for (int z = startLoc.z; z < dimensions.z; ++z)
            {
                for (int y = startLoc.y; y < dimensions.y; ++y)
                {
                    for (int x = startLoc.x; x < dimensions.x; ++x)
                    {
                        nextVoxel = GetVoxel(x, y, z);
                        Debug.Log($"nV: {nextVoxel}");
                        if (nextVoxel != null && nextVoxel.rendering)
                        {
                            voxelGroup.Add(nextVoxel);

                            if (z == startLoc.z && nextVoxel.gridLoc.x > xExtent) xExtent = nextVoxel.gridLoc.x;
                            if (z == startLoc.z && nextVoxel.gridLoc.x == xExtent) yExtent = nextVoxel.gridLoc.y;
                            if (nextVoxel.gridLoc.x == xExtent && nextVoxel.gridLoc.y == yExtent) zExtent = nextVoxel.gridLoc.z;
                        }
                        else break;
                    }
                }
            }

            Debug.Log($"xExt: {xExtent} yExt: {yExtent} zExt: {zExtent}");

            voxelGroup.RemoveAll(v => v.gridLoc.x > xExtent || v.gridLoc.y > yExtent || v.gridLoc.z > zExtent);

            for (int i = 0; i < voxelGroup.Count; i++)
            {
                Debug.Log(voxelGroup[i].gridLoc);
            }

            //Debug.Log($"nb.z = {newBounds.z}");

            var voxelsNBZ = voxelGroup.FindAll(v => v.gridLoc.z == startLoc.z);

            for (int i = 0; i < voxelsNBZ.Count; i++)
            {
                Debug.Log(voxelsNBZ[i].gridLoc);
            }

            // GET VERTEXES

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

        public Voxel GetVoxel(int x, int y, int z)
        {
            if (x < 0 || x >= dimensions.x) return null;
            if (y < 0 || y >= dimensions.y) return null;
            if (z < 0 || z >= dimensions.z) return null;

            int index = x + y * dimensions.x + z * dimensions.x * dimensions.y;

            return voxels[index];
        }

        public Voxel GetVoxel(Vector3Int coords)
        {
            return GetVoxel(coords.x, coords.y, coords.z);
        }

        public Voxel LocateVoxel(Vector3 worldPosition)
        {
            var startPos = transform.position + offset;

            var distanceX = Mathf.Abs(worldPosition.x - startPos.x);
            var distanceY = Mathf.Abs(worldPosition.y - startPos.y);
            var distanceZ = Mathf.Abs(worldPosition.z - startPos.z);

            Debug.Log($"ABS: {distanceX}, {distanceY}, {distanceZ}");

            int x = Mathf.RoundToInt(distanceX * WorldToGrid);
            int y = Mathf.RoundToInt(distanceY * WorldToGrid);
            int z = Mathf.RoundToInt(distanceZ * WorldToGrid);

            Debug.Log($"ROUNDED: {x}, {y}, {z}");

            return GetVoxel(x, y, z);
        }

        public void DisableVoxel(Vector3 worldPosition)
        {
            Voxel target = LocateVoxel(worldPosition);

            target.rendering = false;

            renderingVoxels--;

            RefreshRenderData();
        }

        private void RefreshRenderData()
        {
            vertices.Clear();
            triangles.Clear();

            renderingVoxels = 0;

            FindRenderingVoxel();

            for (int i = 0; i < voxels.Length; i++)
            {
                voxels[i].SupplyVisibleFaces();
                renderingVoxels++;
            }

            GenerateMesh();
        }
    }
}
