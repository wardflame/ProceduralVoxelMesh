using UnityEditor;
using UnityEngine;

namespace Essence.Voxel
{
    [CustomEditor(typeof(VoxelMesh))]
    public class VoxelizedMeshEditor : Editor
    {
        void OnSceneGUI()
        {
            VoxelMesh voxelizedMesh = target as VoxelMesh;

            Handles.color = Color.green;
            float size = voxelizedMesh.halfSize * 2f;

            foreach (Vector3Int gridPoint in voxelizedMesh.gridPoints)
            {
                Vector3 worldPos = voxelizedMesh.PointToWorldPosition(gridPoint);
                Handles.DrawWireCube(worldPos, new Vector3(size, size, size));
            }

            Handles.color = Color.red;
            if (voxelizedMesh.TryGetComponent(out MeshCollider meshCollider))
            {
                Bounds bounds = meshCollider.bounds;
                Handles.DrawWireCube(bounds.center, bounds.extents * 2);
            }
        }
    }
}
