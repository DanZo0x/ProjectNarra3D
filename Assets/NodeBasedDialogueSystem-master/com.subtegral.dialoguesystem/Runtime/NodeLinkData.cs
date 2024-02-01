﻿using System;
using System.Linq;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGUID;
        public string PortName;
        public bool isValid;
        public string TargetNodeGUID;
    }
}