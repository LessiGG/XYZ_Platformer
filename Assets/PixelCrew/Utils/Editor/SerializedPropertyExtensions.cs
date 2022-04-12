using System;
using UnityEditor;

namespace PixelCrew.Utils.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static bool GetEnum<TEnumType>(this SerializedProperty property, out TEnumType returnValue)
            where TEnumType : Enum
        {
            returnValue = default;
            var names = property.enumNames;
            
            if (names == null || names.Length == 0)
                return false;

            var enumName = names[property.enumValueIndex];
            returnValue = (TEnumType) Enum.Parse(typeof(TEnumType), enumName);
            return true;
        }
    }
}