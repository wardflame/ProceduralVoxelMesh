using UnityEngine;

namespace Essence
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SimpleProceduralMesh : MonoBehaviour
    {
        public Mesh procMesh;

        private void OnEnable()
        {
            Mesh mesh = new Mesh {
                name = "Procedural Mesh"
            };

            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
