using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Essence
{
    public class MainMenu : MonoBehaviour
    {
        public TextMeshProUGUI version;

        private void Awake()
        {
            version.text = Application.version;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void LoadGame()
        {
            SceneManager.LoadScene(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
