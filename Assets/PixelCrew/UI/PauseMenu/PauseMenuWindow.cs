using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCrew.UI.PauseMenu
{
    public class PauseMenuWindow : AnimatedWindow
    {
        
        private Action _closeAction;
        
        public void OnShowSettings()
        {
            var window = Resources.Load<GameObject>("UI/SettingsWindow");
            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }

        public void OnRestartGame()
        {
            var currentScene = SceneManager.GetActiveScene();
            _closeAction = () => { SceneManager.LoadScene(currentScene.name); };
            Close();
        }

        public void OnExit()
        {
            _closeAction = () => { SceneManager.LoadScene("MainMenu"); };
            Close();
        }

        public override void OnCloseAnimationComplete()
        {
            _closeAction?.Invoke();
            base.OnCloseAnimationComplete();
        }
    }
}