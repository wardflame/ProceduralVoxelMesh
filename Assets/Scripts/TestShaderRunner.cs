using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Essence
{
    public class TestShaderRunner : MonoBehaviour
    {
        [SerializeField] ComputeShader _computeShader;
        [SerializeField] int _size;

        public RawImage rImage;

        public RenderTexture _renderTexture;

        void Start()
        {
            _renderTexture = new RenderTexture(_size, _size, 24);
            _renderTexture.filterMode = FilterMode.Point;
            _renderTexture.enableRandomWrite = true;
            _renderTexture.Create();

            _computeShader.SetFloat("_Resolution", _renderTexture.width);
            var main = _computeShader.FindKernel("CSMain");
            _computeShader.SetTexture(main, "Result", _renderTexture);
            _computeShader.GetKernelThreadGroupSizes(main, out uint xGroupSize, out uint yGroupSize, out uint zGroupSize);
            _computeShader.Dispatch(main, _renderTexture.width / (int)xGroupSize, _renderTexture.height / (int)yGroupSize, 1);

            Graphics.Blit(_renderTexture, _renderTexture);

            rImage.texture = _renderTexture;
        }
    }
}
