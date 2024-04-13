using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;

namespace Essence.Voxel
{
    public class VoxelData
    {
        public Vector3Int coordinates;
        public List<VoxelData> neighbours;

        public VoxelFaceData[] faces = new VoxelFaceData[6];

        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();

        public bool rendering;

        private VoxelMeshManager manager;

        private float voxelSize;
        private float voxelHalfSize;

        public VoxelData(int posX, int posY, int posZ, VoxelMeshManager manager)
        {
            this.manager = manager;
            voxelSize = manager.voxelSize;
            voxelHalfSize = manager.voxelHalfSize;
            coordinates = new Vector3Int(posX, posY, posZ);
            InitialiseVoxel();
        }

        private void InitialiseVoxel()
        {
            var meshDimensions = manager.dimensions;

            var voxelPos = new Vector3
                (
                    voxelSize * coordinates.x + manager.offset.x,
                    voxelSize * coordinates.y + manager.offset.y,
                    voxelSize * coordinates.z + manager.offset.z
                );

            Vector3[] newVerts;

            #region Create Vertices
            // Front face   (+Z)
            if (coordinates.z + 1 >= meshDimensions.z)
            {
                newVerts = new Vector3[]
                {
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 0
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize), // 1
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 2
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize)  // 3
                };

                AddRenderData(newVerts, 0, VoxelFacing.Z);
            }

            // Back face    (-Z)
            if (coordinates.z - 1 < 0)
            {
                newVerts = new Vector3[]
                {
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 0
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 1
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 2
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize)  // 3
                };

                AddRenderData(newVerts, 1, VoxelFacing.Zm);
            }

            // Left face    (-X)
            if (coordinates.x - 1 < 0)
            {
                newVerts = new Vector3[]
                {
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 0
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize), // 1
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 2
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize)  // 3
                };

                AddRenderData(newVerts, 2, VoxelFacing.Xm);
            }

            // Right face   (+X)
            if (coordinates.x + 1 >= meshDimensions.x)
            {
                newVerts = new Vector3[]
                {
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 0
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 1
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 2
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize)  // 3
                };

                AddRenderData(newVerts, 3, VoxelFacing.X);
            }

            // Top face     (+Y)
            if (coordinates.y + 1 >= meshDimensions.y)
            {
                newVerts = new Vector3[]
                {
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 0
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize), // 1
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z - voxelHalfSize), // 2
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y + voxelHalfSize, voxelPos.z + voxelHalfSize)  // 3
                };

                AddRenderData(newVerts, 4, VoxelFacing.Y);
            }

            // Bottom face  (-Y)
            if (coordinates.y - 1 < 0)
            {
                newVerts = new Vector3[]
                {
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 0
                    new Vector3(voxelPos.x - voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize), // 1
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z + voxelHalfSize), // 2
                    new Vector3(voxelPos.x + voxelHalfSize, voxelPos.y - voxelHalfSize, voxelPos.z - voxelHalfSize)  // 3
                };

                AddRenderData(newVerts, 5, VoxelFacing.Ym);
            }
            #endregion Create Vertices
            
            rendering = true;
        }

        private void AddRenderData(Vector3[] newVerts, int facesIndex, VoxelFacing facing)
        {
            var newTris = new int[]
            {
                manager.vertices.Count,
                manager.vertices.Count + 1,
                manager.vertices.Count + 2,
                manager.vertices.Count + 2,
                manager.vertices.Count + 1,
                manager.vertices.Count + 3
            };

            manager.triangles.AddRange(newTris);
            manager.vertices.AddRange(newVerts);

            faces[facesIndex] = new VoxelFaceData(facing, newVerts, newTris, true);
        }

        public VoxelFaceData GetVoxelFaceByFacing(VoxelFacing facing)
        {
            for (int i = 0; i < faces.Length; i++)
            {
                if (faces[i] == null) continue;
                if (faces[i].facing == facing) return faces[i];
            }

            return null;
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
