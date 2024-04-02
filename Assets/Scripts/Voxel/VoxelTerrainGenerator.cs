using UnityEngine;

namespace Essence.Voxel
{
    public class VoxelTerrainGenerator : MonoBehaviour
    {
        public VoxelMesh voxelMesh;
        public GameObject voxelPrefab;

        private void Start()
        {
            voxelMesh = GetComponent<VoxelMesh>();
            FabricateTerrain();
        }

        private void FabricateTerrain()
        {
            GameObject voxelHouser = new GameObject();
            voxelHouser.name = "VoxelHouser";

            foreach (var meshPoint in voxelMesh.gridPoints)
            {
                var worldPoint = voxelMesh.PointToWorldPosition(meshPoint);

                var newVoxel = Instantiate(voxelPrefab);
                newVoxel.transform.position = worldPoint;

                var voxelSize = voxelMesh.halfSize * 2;
                newVoxel.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

                newVoxel.transform.parent = voxelHouser.transform;
            }

            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}
