using System;
using PixelCrew.Model.Definitions.Repositories.DialogIcons;
using UnityEngine;

namespace PixelCrew.Model.Definitions.Repositories
{
    [CreateAssetMenu(menuName = "Defs/Repository/Characters", fileName = "Characters")]
    public class CharactersRepository : DefRepository<CharsDef>
    {
#if UNITY_EDITOR
        public CharsDef[] ItemsForEditor => _collection;
#endif
    }

    [Serializable]
    public struct CharsDef : IHaveId
    {
        [Character] [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        
        public string Id => _id;
        public Sprite Icon => _icon;
    }
}