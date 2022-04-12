using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Utils
{
    public static class UnityEventExtensions
    {
        private const BindingFlags BindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        
        public static GameObject[] ToGameObjects(this UnityEvent unityEvent)
        {
            var gos = new List<GameObject>();

            if (unityEvent == null) return gos.ToArray();

            var eventType = typeof(UnityEventBase);
            var persistentCallsField = eventType.GetField("m_PersistentCalls", BindingAttr);
            if (persistentCallsField == null) return gos.ToArray();
            var persistentCallsValue = persistentCallsField.GetValue(unityEvent);

            var callGroupType = persistentCallsValue.GetType();
            var callGroupField = callGroupType.GetField("m_Calls", BindingAttr);
            if (callGroupField == null) return gos.ToArray();
            var callGroupValue = (IEnumerable) callGroupField.GetValue(persistentCallsValue);

            var listType = callGroupField.GetValue(persistentCallsValue).GetType();
            if (!listType.IsGenericType || listType.GetGenericTypeDefinition() != typeof(List<>)) return gos.ToArray();
            var itemType = listType.GetGenericArguments().Single();

            foreach (var pc in callGroupValue)
            {
                var itemField = itemType.GetField("m_Target", BindingAttr);
                if (itemField == null) continue;

                var itemValue = (Object) itemField.GetValue(pc);
                var propertyInfo = itemValue.GetType().GetProperty("gameObject");
                if (propertyInfo == null) continue;

                var go = (GameObject) propertyInfo.GetValue(itemValue);
                gos.Add(go);
            }

            return gos.ToArray();
        }
    }
}