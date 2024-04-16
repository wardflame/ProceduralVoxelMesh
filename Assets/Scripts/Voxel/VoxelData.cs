using System.Collections.Generic;
using UnityEngine;

namespace Essence.Voxel
{
    public class VoxelData
    {
        public Vector3Int gridLoc;
        public VoxelData[] neighbours = new VoxelData[6];

        public VoxelFaceData[] faces = new VoxelFaceData[6];

        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();

        public bool rendering;

        private VoxelMeshManager manager;

        private float voxelSize;
        private float voxelHalfSize;

        private List<VoxelData> neighbourVoxels = new List<VoxelData>();

        public VoxelData(int posX, int posY, int posZ, VoxelMeshManager manager)
        {
            this.manager = manager;
            voxelSize = manager.voxelSize;
            voxelHalfSize = manager.voxelHalfSize;
            gridLoc = new Vector3Int(posX, posY, posZ);
            rendering = true;
            manager.initComplete += InitialiseVoxel;
        }

        private void InitialiseVoxel()
        {
            InitFaces();
            InitNeighbours();
            SupplyRenderData();
        }

        private void InitFaces()
        {
            var voxelPos = new Vector3
                (
                    voxelSize * gridLoc.x + manager.offset.x,
                    voxelSize * gridLoc.y + manager.offset.y,
                    voxelSize * gridLoc.z + manager.offset.z
                );

            faces[0] = new VoxelFaceData
                (
                    VoxelFacing.Z,
                    new Vector3[]
                    {
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 0
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize), // 1
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 2
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize)  // 3
                    }
                );

            faces[1] = new VoxelFaceData
                (
                    VoxelFacing.Zm,
                    new Vector3[]
                    {
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 0
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 1
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 2
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize)  // 3
                    }
                );

            faces[2] = new VoxelFaceData
                (
                    VoxelFacing.X,
                    new Vector3[]
                    {
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 0
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 1
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 2
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize)  // 3
                    }
                );

            faces[3] = new VoxelFaceData
                (
                    VoxelFacing.Xm,
                    new Vector3[]
                    {
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 0
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize), // 1
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 2
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize)  // 3
                    }
                );

            faces[4] = new VoxelFaceData
                (
                    VoxelFacing.Y,
                    new Vector3[]
                    {
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 0
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize), // 1
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 2
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize)  // 3
                    }
                );

            faces[5] = new VoxelFaceData
                (
                    VoxelFacing.Ym,
                    new Vector3[]
                    {
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 0
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 1
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 2
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize)  // 3
                    }
                );
        }

        private void InitNeighbours()
        {
            neighbours[0] = manager.GetVoxel(gridLoc.x, gridLoc.y, gridLoc.z + 1);
            neighbours[1] = manager.GetVoxel(gridLoc.x, gridLoc.y, gridLoc.z - 1);
            neighbours[2] = manager.GetVoxel(gridLoc.x + 1, gridLoc.y, gridLoc.z);
            neighbours[3] = manager.GetVoxel(gridLoc.x - 1, gridLoc.y, gridLoc.z);
            neighbours[4] = manager.GetVoxel(gridLoc.x, gridLoc.y + 1, gridLoc.z);
            neighbours[5] = manager.GetVoxel(gridLoc.x, gridLoc.y - 1, gridLoc.z);
        }

        public void SupplyRenderData()
        {
            if (!rendering) return;

            foreach (var face in faces)
            {
                int[] newTris = new int[]
                {
                    manager.vertices.Count,
                    manager.vertices.Count + 1,
                    manager.vertices.Count + 2,
                    manager.vertices.Count + 2,
                    manager.vertices.Count + 1,
                    manager.vertices.Count + 3
                };

                int[] triIndexes = new int[6];

                for (int i = 0; i < newTris.Length; i++)
                {
                    manager.triangles.Add(newTris[i]);
                    triIndexes[i] = manager.triangles.Count - 1;
                }

                face.UpdateTriangles(triIndexes);

                manager.vertices.AddRange(face.vertices);
            }
        }

        private void UpdateRenderData()
        {






            /*bool outOfBounds;

            switch (targetFacing)
            {
                case VoxelFacing.Z:


                    break;
            }

            if (gridLoc.z + 1 >= meshDimensions.z)
            {
                SupplyRenderData(newVerts);
            }
            else
            {
                VoxelData zVoxel = manager.GetVoxel(gridLoc.x, gridLoc.y, gridLoc.z + 1);

                if (zVoxel != null)
                {
                    neighbourVoxels.Add(zVoxel);

                    if (zVoxel.rendering == false) SupplyRenderData(newVerts);
                }
            }

            if (gridLoc.x - 1 < 0)
            {
                SupplyRenderData(newVerts);
            }
            else
            {
                VoxelData zVoxel = manager.GetVoxel(gridLoc.x - 1, gridLoc.y, gridLoc.z);

                if (zVoxel != null)
                {
                    neighbourVoxels.Add(zVoxel);

                    if (zVoxel.rendering == false) SupplyRenderData(newVerts);
                }
            }*/
        }

        private void UpdateFaceVertices(VoxelFacing facing)
        {
            VoxelFaceData face = GetFacebyFacing(facing);
            VoxelData neighbour = GetNeighbourByFacing(facing);


        }

        public VoxelFaceData GetFacebyFacing(VoxelFacing facing)
        {
            return faces[(int)facing];
        }

        private VoxelData GetNeighbourByFacing(VoxelFacing facing)
        {
            return neighbours[(int)facing];
        }
    }

    /// <summary>
    /// Refers to which direction the facing considers forward. m = minus.
    /// </summary>
    public enum VoxelFacing
    {
        Z,
        Zm,
        X,
        Xm,
        Y,
        Ym
    }
}
