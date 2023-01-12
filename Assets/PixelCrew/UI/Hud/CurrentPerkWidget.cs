using System;
using PixelCrew.Model;
using PixelCrew.Model.Definitions.Repositories;
using UnityEngine;
using UnityEngine.UI;

namespace PixelCrew.UI.Hud
{
    public class CurrentPerkWidget : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _cooldownImage;
        
        private GameSession _session;

        private void Start()
        {
            _session = GameSession.Instance;
        }

        public void Set(PerkDef perkDef)
        {
            _icon.sprite = perkDef.Icon;
        }

        private void Update()
        {
            var cooldown = _session.PerksModel.CoolDown;
            _cooldownImage.fillAmount = cooldown.RemainingTime / cooldown.Value;
        }
    }
}