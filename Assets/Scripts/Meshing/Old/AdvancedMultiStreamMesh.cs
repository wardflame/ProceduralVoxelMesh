using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence.Meshing
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class AdvancedMultiStreamMesh : MonoBehaviour
    {
        private void OnEnable()
        {
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);

            Mesh mesh = new Mesh
            {
                name = "Advanced Mesh"
            };

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
