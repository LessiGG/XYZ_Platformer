using System.Globalization;
using PixelCrew.Model;
using PixelCrew.Model.Definitions;
using PixelCrew.Model.Definitions.Localization;
using PixelCrew.Model.Definitions.Player;
using PixelCrew.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace PixelCrew.UI.Windows.PlayerStats
{
    public class StatWidget : MonoBehaviour, IItemRenderer<StatDef>
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private Text _currentValue;
        [SerializeField] private Text _increaseValue;
        [SerializeField] private ProgressBarWidget _progress;
        [SerializeField] private GameObject _selector;
        
        private GameSession _session;
        private StatDef _data;

        private void Start()
        {
            _session = GameSession.Instance;
            UpdateView();
        }

        public void SetData(StatDef data, int index)
        {
            _data = data;
            if (_session != null)
                UpdateView();
        }

        private void UpdateView()
        {
            var statsModel = _session.StatsModel;
            
            _icon.sprite = _data.Icon;
            _name.text = LocalizationManager.I.Localize(_data.StatName);
            var currentLevelValue = statsModel.GetValue(_data.Id);
            _currentValue.text = currentLevelValue.ToString(CultureInfo.InvariantCulture);

            var currentLevel = statsModel.GetCurrentLevel(_data.Id);
            var nextLevel = currentLevel + 1;
            var nextLevelValue = statsModel.GetValue(_data.Id, nextLevel);
            var increaseValue = nextLevelValue - currentLevelValue;
            _increaseValue.text = $"+ {increaseValue}";
            _increaseValue.gameObject.SetActive(increaseValue > 0);

            var maxLevel = DefsFacade.I.Player.GetStat(_data.Id).Levels.Length - 1;
            _progress.SetProgress(currentLevel / (float) maxLevel);
            
            _selector.SetActive(statsModel.InterfaceSelectedStat.Value == _data.Id);
        }

        public void OnSelect()
        {
            _session.StatsModel.InterfaceSelectedStat.Value = _data.Id;
        }
    }
}