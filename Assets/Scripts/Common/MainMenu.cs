using Manage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Common
{
    /// <summary>
    /// Main menu operate.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button loadGameBtn;

        #region Zenject

        private SaveLoadSystem saveLoadSystem;

        #endregion
        [Inject]
        private void Construct(SaveLoadSystem system)
        {
            this.saveLoadSystem = system;
        }
        private void OnEnable()
        {
            this.loadGameBtn.onClick.AddListener(this.LoadGame);
        }
        /// <summary>
        /// Load level.
        /// </summary>
        public void NewGame()
        {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
        private void LoadGame()
        {
            
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
        /// <summary>
        /// Close application.
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }
    }
}
