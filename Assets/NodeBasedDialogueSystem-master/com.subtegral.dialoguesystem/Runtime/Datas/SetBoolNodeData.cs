using System;
using UnityEngine;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class SetBoolNodeData
    {
        public string NodeGUID;
        public string Property;
        public bool Value;
        public Vector2 Position;
    }
}