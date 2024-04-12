using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence.Voxel
{
    public class VoxelFaceData
    {
        public bool rendering;
        public VoxelFacing facing;
        public Vector3[] vertices;
        public int[] triangles;

        public VoxelFaceData(VoxelFacing facing, Vector3[] vertices, int[] triangles, bool rendering)
        {
            this.facing = facing;
            this.vertices = vertices;
            this.triangles = triangles;
            this.rendering = rendering;
        }
    }
}
