using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence.Interfaces
{
    public interface IInteractable
    {
        public void OnInteract(GameObject interactor);
    }
}
