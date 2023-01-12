using System.Collections.Generic;
using System.Linq;
using PixelCrew.Model.Definitions.Localization;
using PixelCrew.UI.Widgets;
using UnityEngine;

namespace PixelCrew.UI.Windows.Localization
{
    public class LocalizationWindow : AnimatedWindow
    {
        [SerializeField] private LocaleItemWidget _prefab;
        [SerializeField] private Transform _container;
        
        private DataGroup<LocaleInfo, LocaleItemWidget> _dataGroup;

        private readonly string[] _supportedLocales = {"en", "ru"};

        protected override void Start()
        {
            base.Start();

            _dataGroup = new DataGroup<LocaleInfo, LocaleItemWidget>(_prefab, _container);
            _dataGroup.SetData(ComposeData());
        }   

        private List<LocaleInfo> ComposeData()
        {
            return _supportedLocales.Select(locale => new LocaleInfo {LocaleId = locale}).ToList();
        }

        public void OnSelected(string selectedLocale)
        {
            LocalizationManager.I.SetLocale(selectedLocale);
        }
    }
}