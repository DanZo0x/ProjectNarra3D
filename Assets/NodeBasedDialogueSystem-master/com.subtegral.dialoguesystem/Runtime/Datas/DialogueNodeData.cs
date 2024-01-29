using System;
using UnityEngine;
namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class DialogueNodeData
    {
        public string NodeGUID;
        public string KeySpeaker;
        public string NodeKeyText;
        public int SpeakerEmotion;
        public Vector2 Position;
        
    }
}