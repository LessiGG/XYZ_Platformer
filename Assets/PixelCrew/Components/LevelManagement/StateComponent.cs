﻿using PixelCrew.Components.GoBased;
using PixelCrew.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCrew.Components.LevelManagement
{
    [RequireComponent(typeof(DestroyObjectComponent))]
    public class StateComponent : MonoBehaviour
    {
        private string _id;
        private GameSession _gameSession;

        public void StoreStateToDestroy()
        {
            _gameSession.StoreState(_id);
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(_id)) _id = $"{SceneManager.GetActiveScene().name}-{gameObject.name}";
            _gameSession = GameSession.Instance;
            if(_gameSession.IsItemDestroyed(_id)) GetComponent<DestroyObjectComponent>().DestroyObject();
        }
    }
}