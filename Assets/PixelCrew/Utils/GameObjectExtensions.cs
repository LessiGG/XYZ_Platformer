﻿using UnityEngine;

namespace PixelCrew.Utils
{
    public static class GameObjectExtensions
    {
        public static bool IsInLayer(this GameObject gameObject, LayerMask layer)
        {
            return layer == (layer | 1 << gameObject.layer);
        }

        public static TInterfaceType GetInterface<TInterfaceType>(this GameObject go)
        {
            var components = go.GetComponents<Component>();

            foreach (var component in components)
            {
                if (component is TInterfaceType type)
                    return type;
            }
            return default;
        }
    }
}