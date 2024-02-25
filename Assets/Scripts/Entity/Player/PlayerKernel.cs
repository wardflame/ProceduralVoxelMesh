using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthetic.Entity.Player
{
    public class PlayerKernel : MonoBehaviour
    {
        public PlayerMovement movement;
        public EssenceInput input;

        private void Awake()
        {
            input = new EssenceInput();
            movement = GetComponent<PlayerMovement>();
        }
    }
}
