using System.Collections.Generic;
using UnityEngine;

namespace Essence.Voxel
{
    public class VoxelMesh : MonoBehaviour
    {
        public List<Vector3Int> gridPoints = new List<Vector3Int>();
        public float halfSize = 0.1f;
        public Vector3 localOrigin;

        public Renderer meshRenderer;

        public Bounds b;

        private void Start()
        {
            meshRenderer = GetComponent<Renderer>();
            VoxelizeMesh();
        }

        public Vector3 PointToWorldPosition(Vector3Int point)
        {
            float size = halfSize * 2f;
            Vector3 position =
                new Vector3
                (
                    (halfSize + point.x * size) / transform.localScale.x,
                    (halfSize + point.y * size) / transform.localScale.y,
                    (halfSize + point.z * size) / transform.localScale.z
                );

            return localOrigin + transform.TransformPoint(position);
        }

        public void VoxelizeMesh()
        {
            Bounds bounds = meshRenderer.bounds;
            bounds.extents = Vector3.Scale(bounds.extents, transform.localScale);
            b = bounds;
            Vector3 minExtents = bounds.center - bounds.extents;
            Vector3 count = bounds.extents / halfSize;

            int xMax = Mathf.CeilToInt(count.x);
            int yMax = Mathf.CeilToInt(count.y);
            int zMax = Mathf.CeilToInt(count.z);
                        var objectCollider = GetComponent<Collider>();

            gridPoints.Clear();
            localOrigin = transform.InverseTransformPoint(minExtents);

            for (int x = 0; x < xMax; ++x)
            {
                for (int z = 0; z < zMax; ++z)
                {
                    for (int y = 0; y < yMax; ++y)
                    {
                        Vector3 pos = PointToWorldPosition(new Vector3Int(x, y, z));
                        foreach(var collider in Physics.OverlapBox(pos, Vector3.one * halfSize * 0.95f))
                        {
                            if (collider != objectCollider) continue;

                            gridPoints.Add(new Vector3Int(x, y, z));
                            break;
                        }
                    }
                }
            }
        }
    }
}
