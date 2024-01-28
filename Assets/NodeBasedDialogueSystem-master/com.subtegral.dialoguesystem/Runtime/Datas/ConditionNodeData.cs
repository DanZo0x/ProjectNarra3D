using System;
using UnityEngine;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class ConditionNodeData
    {
        public string NodeGUID;
        public string NodeProperty;
        public bool PropertyValue;
        public Vector2 Position;
        
    }
}