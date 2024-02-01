using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public interface IUITextTyper
    {
        TextMeshProUGUI TextField { get; }
        
        bool IsReadingText { get; }
        
        void ReadText(string text,AudioClip clip);
    }
}