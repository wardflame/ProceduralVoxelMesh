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
            Debug.Log($"Start Voxel: {startVoxel.gridLoc}");

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
                        else break;
                    }
                }
            }

            Debug.Log($"xExt: {extent.x} yExt: {extent.y} zExt: {extent.z}");

            // VET ALL VOXELS NOT WITHIN BOUNDS
            voxelGroup.RemoveAll(v => v.gridLoc.x > extent.x || v.gridLoc.y > extent.y || v.gridLoc.z > extent.z);

            for (int i = 0; i < voxelGroup.Count; i++)
            {
                Debug.Log(voxelGroup[i].gridLoc);
            }

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

            /*Vector3 bLZ = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.y == startLoc.y && v.gridLoc.z == zExtent).faces[0].vertices[0];
            Vector3 tLZ = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.y == yExtent && v.gridLoc.z == zExtent).faces[0].vertices[1];
            Vector3 bRZ = voxelGroup.Find(v => v.gridLoc.x == startLoc.x && v.gridLoc.y == startLoc.y && v.gridLoc.z == zExtent).faces[0].vertices[2];
            Vector3 tRZ = voxelGroup.Find(v => v.gridLoc.x == startLoc.x && v.gridLoc.y == yExtent && v.gridLoc.z == zExtent).faces[0].vertices[3];

            Vector3 bLZm = voxelGroup.Find(v => v.gridLoc == startLoc).faces[1].vertices[0];
            Vector3 tLZm = voxelGroup.Find(v => v.gridLoc.z == startLoc.z && v.gridLoc.x == startLoc.x && v.gridLoc.y == yExtent).faces[1].vertices[1];
            Vector3 bRZm = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.y == startLoc.y && v.gridLoc.z == startLoc.z).faces[1].vertices[2];
            Vector3 tRZm = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.y == xExtent && v.gridLoc.z == startLoc.z).faces[1].vertices[3];*/

            /* GET VERTICES
            // /* Z FACE 
            Vector3 z1 = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.y == startLoc.y && v.gridLoc.z == zExtent).faces[0].vertices[0];
            Vector3 z2 = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.y == yExtent && v.gridLoc.z == zExtent).faces[0].vertices[1];
            Vector3 z3 = voxelGroup.Find(v => v.gridLoc.x == startLoc.x && v.gridLoc.y == startLoc.y && v.gridLoc.z == zExtent).faces[0].vertices[2];
            Vector3 z4 = voxelGroup.Find(v => v.gridLoc.x == startLoc.x && v.gridLoc.y == yExtent && v.gridLoc.z == zExtent).faces[0].vertices[3];
            //


            // /* ZM FACE
            Vector3 zm1 = voxelGroup.Find(v => v.gridLoc == startLoc).faces[1].vertices[0];
            Vector3 zm2 = voxelGroup.Find(v => v.gridLoc.x == startLoc.x && v.gridLoc.y == yExtent && v.gridLoc.z == startLoc.z).faces[1].vertices[1];
            Vector3 zm3 = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.z == startLoc.z).faces[1].vertices[2];
            Vector3 zm4 = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.y == yExtent && v.gridLoc.z == startLoc.z).faces[1].vertices[3];
            //

            // /* X FACE
            Vector3 x1 = voxelGroup.Find(v => v.gridLoc.x == xExtent).faces[3].vertices[0];
            Vector3 x2 = voxelGroup.Find(v => v.gridLoc.x == startLoc.x && v.gridLoc.y == yExtent && v.gridLoc.z == startLoc.z).faces[3].vertices[1];
            Vector3 x3 = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.z == startLoc.z).faces[3].vertices[2];
            Vector3 x4 = voxelGroup.Find(v => v.gridLoc.x == xExtent && v.gridLoc.y == yExtent && v.gridLoc.z == startLoc.z).faces[3].vertices[3];
            // */

            vertices = new()
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
                voxBLZn.faces[5].vertices[3]
            };

            triangles = new()
            {
                // Z
                0, 1, 2, 2, 1, 3,

                // Zm
                4, 5, 6, 6, 5, 7,

                // X
                8, 9, 10, 10, 9, 11,

                // Xm
                12, 13, 14, 14, 13, 15,

                // Y
                16, 17, 18, 18, 17, 19,

                // Ym
                20, 21, 22, 22, 21, 23
            };
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
