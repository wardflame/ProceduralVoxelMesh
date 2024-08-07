using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Essence.Voxel
{
    public class VoxelFace
    {
        public VoxelFacing facing;
        public Vector3[] vertices;
        public List<int> triangles;

        public VoxelFace(VoxelFacing facing, Vector3[] vertices)
        {
            this.facing = facing;
            this.vertices = vertices;
        }

        public void UpdateTriangles(IEnumerable<int> tris)
        {
            triangles = tris.ToList();
        }
    }
}
