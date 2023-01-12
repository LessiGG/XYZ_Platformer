using System.Linq;
using PixelCrew.Model.Definitions.Repositories.DialogIcons;
using UnityEditor;
using UnityEngine;

namespace PixelCrew.Model.Definitions.Editor
{
    [CustomPropertyDrawer(typeof(CharacterAttribute))]
    public class CharacterAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var defs = DefsFacade.I.Characters.ItemsForEditor;
            var ids = defs.Select(itemDef => itemDef.Id).ToList();

            var index = Mathf.Max(ids.IndexOf(property.stringValue), 0);
            index = EditorGUI.Popup(position, property.displayName, index, ids.ToArray());
            property.stringValue = ids[index];
        }
    }
}