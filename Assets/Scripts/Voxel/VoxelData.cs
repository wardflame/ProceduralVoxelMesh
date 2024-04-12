using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;

namespace Essence.Voxel
{
    public class VoxelData
    {
        public Vector3Int coordinates;
        public List<VoxelData> neighbours;

        public VoxelFaceData[] faces;

        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();

        private VoxelMeshManager manager;

        public VoxelData(int posX, int posY, int posZ, float voxelSize, Vector3Int meshDimensions, VoxelMeshManager manager)
        {
            this.manager = manager;
            coordinates = new Vector3Int(posX, posY, posZ);
            CreateVoxel(voxelSize, meshDimensions);
        }

        private void CreateVoxel(float voxelSize, Vector3Int meshDimensions)
        {
            var manPos = manager.transform.position;

            var voxelPos = new Vector3
                (
                    manPos.x + (voxelSize * coordinates.x),
                    manPos.y + (voxelSize * coordinates.y),
                    manPos.z + (voxelSize * coordinates.z)
                );

            Debug.Log(voxelPos);

            if (coordinates.z + 1 >= meshDimensions.z)
            {
                var backFace = new Vector3[]
                {
                    new Vector3(voxelPos.x + manager.voxelHalfSize, voxelPos.y - manager.voxelHalfSize, voxelPos.z + manager.voxelHalfSize), // 0
                    new Vector3(voxelPos.x + manager.voxelHalfSize, voxelPos.y + manager.voxelHalfSize, voxelPos.z + manager.voxelHalfSize), // 1
                    new Vector3(voxelPos.x - manager.voxelHalfSize, voxelPos.y - manager.voxelHalfSize, voxelPos.z + manager.voxelHalfSize), // 2
                    new Vector3(voxelPos.x - manager.voxelHalfSize, voxelPos.y + manager.voxelHalfSize, voxelPos.z + manager.voxelHalfSize)  // 3
                };

                vertices.AddRange(backFace);

                var trianglesOffset = coordinates.x + meshDimensions.y * coordinates.y + meshDimensions.z * coordinates.z;

                var backTriangles = new int[]
                {
                    manager.vertices.Count,
                    manager.vertices.Count + 1,
                    manager.vertices.Count + 2,
                    manager.vertices.Count + 2,
                    manager.vertices.Count + 1,
                    manager.vertices.Count + 3
                };

                triangles.AddRange(backTriangles);
            }
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
