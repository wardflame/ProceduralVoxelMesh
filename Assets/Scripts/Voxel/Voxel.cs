using System;
using System.Collections.Generic;
using UnityEngine;

namespace Essence.Voxel
{
    public class Voxel
    {
        public Vector3Int gridLoc;
        public Voxel[] neighbours = new Voxel[6];

        public VoxelFace[] faces = new VoxelFace[6];

        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();

        public bool rendering = true;

        private VoxelMeshManager manager;

        private float voxelSize;
        private float voxelHalfSize;

        public Voxel(int posX, int posY, int posZ, VoxelMeshManager manager)
        {
            this.manager = manager;
            voxelSize = manager.voxelSize;
            voxelHalfSize = manager.voxelHalfSize;
            gridLoc = new Vector3Int(posX, posY, posZ);
            manager.initComplete += InitialiseVoxel;
        }

        private void InitialiseVoxel()
        {
            InitFaces();
            InitNeighbours();
            SupplyVisibleFaces();
        }

        private void InitFaces()
        {
            var voxelPos = new Vector3
                (
                    voxelSize * gridLoc.x + manager.offset.x,
                    voxelSize * gridLoc.y + manager.offset.y,
                    voxelSize * gridLoc.z + manager.offset.z
                );

            faces[0] = new VoxelFace
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

            faces[1] = new VoxelFace
                (
                    VoxelFacing.Zn,
                    new Vector3[]
                    {
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 0
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 1
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 2
                        new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize)  // 3
                    }
                );

            faces[2] = new VoxelFace
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

            faces[3] = new VoxelFace
                (
                    VoxelFacing.Xn,
                    new Vector3[]
                    {
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 0
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize), // 1
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 2
                        new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize)  // 3
                    }
                );

            faces[4] = new VoxelFace
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

            faces[5] = new VoxelFace
                (
                    VoxelFacing.Yn,
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

            /*if (gridLoc.x == 0 && gridLoc.y == 1)
            {
                Debug.Log(manager.GetVoxel(gridLoc.x - 1, gridLoc.y, gridLoc.z));
                Debug.Log(neighbours[3]);
            }*/
        }

        public void SupplyVisibleFaces()
        {
            if (!rendering) return;

            List<VoxelFacing> faces = new();

            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] == null || !neighbours[i].rendering)
                {
                    faces.Add((VoxelFacing)i);
                }
            }

            foreach (var face in faces)
            {
                int[] newTris = new int[]
                {
                    manager.vertices.Count,
                    manager.vertices.Count + 1,
                    manager.vertices.Count + 2,
                    manager.vertices.Count + 2,
                    manager.vertices.Count + 1,
                    manager.vertices.Count + 3,
                };

                manager.vertices.AddRange(GetFacebyFacing(face).vertices);
                manager.triangles.AddRange(newTris);
            }
        }

        public void DetermineEfficientVertices()
        {
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] == null || !neighbours[i].rendering)
                {
                    switch ((VoxelFacing)i)
                    {
                        case VoxelFacing.Z:


                            break;

                        case VoxelFacing.X:


                            break;
                    }
                }
            }
        }

        public VoxelFace GetFacebyFacing(VoxelFacing facing)
        {
            return faces[(int)facing];
        }

        private Voxel GetNeighbourByFacing(VoxelFacing facing)
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
        Zn,
        X,
        Xn,
        Y,
        Yn
    }
}
