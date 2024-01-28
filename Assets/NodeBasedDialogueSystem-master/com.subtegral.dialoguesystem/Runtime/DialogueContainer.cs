﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
        public List<ConditionNodeData> ConditionNodeData = new List<ConditionNodeData>();
        public List<SetBoolNodeData> SetBoolNodeData = new List<SetBoolNodeData>();
        public List<SetAffinityData> SetAffinityNodeData = new List<SetAffinityData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        public List<CommentBlockData> CommentBlockData = new List<CommentBlockData>();
    }
}