﻿using System;
using PixelCrew.Utils.Editor;
using UnityEditor;

namespace PixelCrew.Components.Dialogs.Editor
{
    [CustomEditor(typeof(ShowDialogComponent))]
    public class ShowDialogComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty _modeProperty;
        private SerializedProperty _onCompleteProperty;
        private void OnEnable()
        {
            _modeProperty = serializedObject.FindProperty("_mode");
            _onCompleteProperty = serializedObject.FindProperty("_onComplete");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_modeProperty);
            ShowDialogComponent.Mode mode;
            
            if (_modeProperty.GetEnum(out mode))
            {
                switch (mode)
                {
                    case ShowDialogComponent.Mode.Bound:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_bound"));
                        break;
                    case ShowDialogComponent.Mode.External:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_external"));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            EditorGUILayout.PropertyField(_onCompleteProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}