using UnityEngine;

namespace Essence.Settings
{
    public class SettingsPlaceholder : MonoBehaviour
    {
        private void Awake()
        {
            //var refreshRatio = Screen.currentResolution.refreshRateRatio;
            //var refreshRate = refreshRatio.numerator / refreshRatio.denominator;
            //Application.targetFrameRate = (int)refreshRate;
            //Time.fixedDeltaTime = 1 / refreshRate;
            Application.targetFrameRate = 60;
        }
    }
}
