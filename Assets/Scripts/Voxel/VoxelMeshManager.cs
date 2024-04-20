using Essence.Voxel;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

            if (optimiseMesh) CreateOptimisedMeshData();
            else SupplyFaces();

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

        private Voxel FindValidVoxel(List<Voxel> invalidVoxels)
        {
            Voxel startVoxel = null;

            for (int z = 0; z < dimensions.z; z++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    for (int x = 0; x < dimensions.x; x++)
                    {
                        startVoxel = GetVoxel(x, y, z);
                        if (startVoxel != null && startVoxel.rendering && !invalidVoxels.Contains(startVoxel))
                        {
                            x = dimensions.x;
                            y = dimensions.y;
                            z = dimensions.z;
                        }
                    }
                }
            }

            return startVoxel;
        }

        private VoxelPayload FindVoxelsToOptimise(Voxel startVoxel, List<Voxel> invalidVoxels)
        {
            if (startVoxel == null || !startVoxel.rendering) Debug.Log("Couldn't find Voxel");

            // ITERATE AND FIND VOXELS TO MAKE A NEW SUB-CUBE
            List<Voxel> voxelGroup = new();
            var startLoc = startVoxel.gridLoc;
            Voxel nextVoxel = null;

            Vector3Int extent = startLoc;
            Vector3Int groupDimensions = new();

            for (int z = startLoc.z; z < dimensions.z; ++z)
            {
                for (int y = startLoc.y; y < dimensions.y; ++y)
                {
                    for (int x = startLoc.x; x < dimensions.x; ++x)
                    {
                        nextVoxel = GetVoxel(x, y, z);
                        if (nextVoxel != null && nextVoxel.rendering && !invalidVoxels.Contains(nextVoxel))
                        {
                            voxelGroup.Add(nextVoxel);

                            if (z == startLoc.z && y == startLoc.y && nextVoxel.gridLoc.x > extent.x)
                            {
                                extent.x = nextVoxel.gridLoc.x;
                                groupDimensions.x++;
                            }

                            if (z == startLoc.z && nextVoxel.gridLoc.x == extent.x && nextVoxel.gridLoc.y > extent.y)
                            {
                                extent.y = nextVoxel.gridLoc.y;
                                groupDimensions.y++;
                            }

                            if (nextVoxel.gridLoc.x == extent.x && nextVoxel.gridLoc.y == extent.y && nextVoxel.gridLoc.z > extent.z)
                            {
                                extent.z = nextVoxel.gridLoc.z;
                                groupDimensions.z++;
                            }
                        }
                        else
                        {
                            x = dimensions.x;
                            y = dimensions.y;
                            z = dimensions.z;
                        }
                    }
                }
            }

            // VET ALL VOXELS NOT WITHIN BOUNDS
            voxelGroup.RemoveAll(v => v.gridLoc.x > extent.x || v.gridLoc.y > extent.y || v.gridLoc.z > extent.z);

            return new VoxelPayload(voxelGroup, groupDimensions);
        }

        private void CreateVerticesAndTriangles(VoxelPayload vP)
        {
            Voxel voxBLZ =  vP.voxelGroup[vP.groupDimensions.x + vP.groupDimensions.z * vP.bounds.x * vP.bounds.y];
            Voxel voxTLZ =  vP.voxelGroup[vP.voxelGroup.Count - 1];
            Voxel voxBRZ =  vP.voxelGroup[vP.groupDimensions.z * vP.bounds.x * vP.bounds.y];
            Voxel voxTRZ =  vP.voxelGroup[vP.groupDimensions.y * vP.bounds.x + vP.groupDimensions.z * vP.bounds.x * vP.bounds.y];
                                                
            Voxel voxBLZn = vP.voxelGroup[0];
            Voxel voxTLZn = vP.voxelGroup[vP.groupDimensions.y * vP.bounds.x];
            Voxel voxBRZn = vP.voxelGroup[vP.groupDimensions.x];
            Voxel voxTRZn = vP.voxelGroup[vP.groupDimensions.x + vP.groupDimensions.y * vP.bounds.x];

            Vector3[] verts =
            {
                // Zn
                voxBLZ.faces[0].vertices[0],
                voxTLZ.faces[0].vertices[1],
                voxBRZ.faces[0].vertices[2],
                voxTRZ.faces[0].vertices[3],

                // Zn
                voxBLZn.faces[1].vertices[0],
                voxTLZn.faces[1].vertices[1],
                voxBRZn.faces[1].vertices[2],
                voxTRZn.faces[1].vertices[3],

                // X
                voxBRZn.faces[2].vertices[0],
                voxTRZn.faces[2].vertices[1],
                voxBLZ.faces[2].vertices[2],
                voxTLZ.faces[2].vertices[3],

                // Xn
                voxBRZ.faces[3].vertices[0],
                voxTRZ.faces[3].vertices[1],
                voxBLZn.faces[3].vertices[2],
                voxTLZn.faces[3].vertices[3],

                // Y
                voxTLZn.faces[4].vertices[0],
                voxTRZ.faces[4].vertices[1],
                voxTRZn.faces[4].vertices[2],
                voxTLZ.faces[4].vertices[3],

                // Yn
                voxBRZ.faces[5].vertices[0],
                voxBLZn.faces[5].vertices[1],
                voxBLZ.faces[5].vertices[2],
                voxBRZn.faces[5].vertices[3]
            };

            int[] tris =
            {
                // Z
                vertices.Count,
                vertices.Count + 1,
                vertices.Count + 2,
                vertices.Count + 2,
                vertices.Count + 1,
                vertices.Count + 3,

                // Zm
                vertices.Count + 4,
                vertices.Count + 5,
                vertices.Count + 6,
                vertices.Count + 6,
                vertices.Count + 5,
                vertices.Count + 7,

                // X
                vertices.Count + 8,
                vertices.Count + 9,
                vertices.Count + 10,
                vertices.Count + 10,
                vertices.Count + 9,
                vertices.Count + 11,

                // Xm
                vertices.Count + 12,
                vertices.Count + 13,
                vertices.Count + 14,
                vertices.Count + 14,
                vertices.Count + 13,
                vertices.Count + 15,

                // Y
                vertices.Count + 16,
                vertices.Count + 17,
                vertices.Count + 18,
                vertices.Count + 18,
                vertices.Count + 17,
                vertices.Count + 19,

                // Ym
                vertices.Count + 20,
                vertices.Count + 21,
                vertices.Count + 22,
                vertices.Count + 22,
                vertices.Count + 21,
                vertices.Count + 23
            };

            vertices.AddRange(verts);
            triangles.AddRange(tris);
        }

        private void OptimiseMesh(ref int voxelsRendered, ref List<Voxel>invalidVoxels)
        {
            var startVoxel = FindValidVoxel(invalidVoxels);

            VoxelPayload vP = FindVoxelsToOptimise(startVoxel, invalidVoxels);

            if (vP.voxelGroup.Count == 0) return;

            CreateVerticesAndTriangles(vP);

            voxelsRendered += vP.voxelGroup.Count;
            invalidVoxels.AddRange(vP.voxelGroup);
        }

        private void CreateOptimisedMeshData()
        {
            int voxelsRendered = 0;
            List<Voxel> invalidVoxels = new();

            while (voxelsRendered < renderingVoxels)
            {
                OptimiseMesh(ref voxelsRendered, ref invalidVoxels);
                if (renderingVoxels == voxels.Count()) break;
            }
        }

        private void SupplyFaces()
        {
            renderingVoxels = 0;
            for (int i = 0; i < voxels.Length; i++)
            {
                voxels[i].SupplyVisibleFaces();
                renderingVoxels++;
            }
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

            int x = Mathf.RoundToInt(distanceX * WorldToGrid);
            int y = Mathf.RoundToInt(distanceY * WorldToGrid);
            int z = Mathf.RoundToInt(distanceZ * WorldToGrid);

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

            CreateOptimisedMeshData();

            GenerateMesh();
        }
    }
}

public struct VoxelPayload
{
    public VoxelPayload(List<Voxel> voxelGroup, Vector3Int groupDimensions)
    {
        this.voxelGroup = voxelGroup;
        this.groupDimensions = groupDimensions;
        bounds = new(groupDimensions.x + 1, groupDimensions.y + 1, groupDimensions.z + 1);
    }

    public List<Voxel> voxelGroup;
    public Vector3Int groupDimensions;
    public Vector3Int bounds;
}
