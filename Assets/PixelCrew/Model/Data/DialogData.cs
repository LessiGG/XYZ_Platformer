using System;
using PixelCrew.Model.Definitions.Repositories.DialogIcons;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    [Serializable]
    public struct DialogData
    {
        [SerializeField] private DialogType _type;
        [SerializeField] private Sentence[] _sentences;

        public Sentence[] Sentences => _sentences;
        public DialogType Type => _type;
    }

    [Serializable]
    public struct Sentence
    {
        [SerializeField] private string _value;
        [Character] [SerializeField] private string _character;
        [SerializeField] private Side _side;

        public string Value => _value;
        public string Character => _character;
        public Side Side => _side;
    }

    public enum Side
    {
        Left, 
        Right
    }

    public enum DialogType
    {
        Simple, 
        Personalized
    }
}