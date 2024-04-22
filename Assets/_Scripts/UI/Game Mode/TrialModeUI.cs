using TMPro;
using UnityEngine;

namespace Essence
{
    public class TrialModeUI : MonoBehaviour
    {
        public CanvasGroup cg;
        public TextMeshProUGUI timer;
        public TextMeshProUGUI currentScore;
        public TextMeshProUGUI previousScore;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
        }
    }
}
