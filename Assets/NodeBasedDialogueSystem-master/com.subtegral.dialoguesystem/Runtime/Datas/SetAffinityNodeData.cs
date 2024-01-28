using System;
using UnityEngine;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class SetAffinityData
    {
        public string NodeGUID;
        public string TargetName;
        public int Value;
        public Vector2 Position;
    }
}