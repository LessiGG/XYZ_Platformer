﻿using System;
using PixelCrew.UI.LevelsLoader;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.UI.Windows.MainMenu
{
    public class MainMenuWindow : AnimatedWindow
    {
        private Action _closeAction;
        
        public void OnShowSettings()
        {
            WindowUtils.CreateWindow("UI/SettingsWindow");
        }

        public void OnShowLanguages()
        {
            WindowUtils.CreateWindow("UI/LocalizationWindow");
        }
        public void OnShowControls()
        {
            WindowUtils.CreateWindow("UI/ControlsWindow");
        }

        public void OnStartGame()
        {
            _closeAction = () =>
            {
                var loader = FindObjectOfType<LevelLoader>();
                loader.LoadLevel("Level1");
            };
            Close();
        }

        public void OnExit()
        {
            _closeAction = () =>
            {
                Application.Quit();
            
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            };
            Close();
        }

        public override void OnCloseAnimationComplete()
        {
            _closeAction?.Invoke();
            base.OnCloseAnimationComplete();
        }
    }
}