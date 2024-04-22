using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Essence.UI.Player
{
    public class PlayerPrompt : MonoBehaviour
    {
        private CanvasGroup cg;
        private TextMeshProUGUI text;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            text = GetComponent<TextMeshProUGUI>();
        }

        public void ShowPrompt(string prompt)
        {
            text.text = prompt;
            cg.alpha = 1;
        }

        public void HidePrompt()
        {
            cg.alpha = 0;
            text.text = "";
        }
    }
}
