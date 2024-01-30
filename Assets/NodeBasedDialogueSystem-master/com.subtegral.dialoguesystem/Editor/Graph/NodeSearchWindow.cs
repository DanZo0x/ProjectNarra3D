﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Subtegral.DialogueSystem.Editor
{
    public class NodeSearchWindow : ScriptableObject,ISearchWindowProvider
    {
        private EditorWindow _window;
        private StoryGraphView _graphView;

        private Texture2D _indentationIcon;
        
        public void Configure(EditorWindow window,StoryGraphView graphView)
        {
            _window = window;
            _graphView = graphView;
            
            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeEntry(new GUIContent("Dialogue Node", _indentationIcon))
                {
                    level = 1, userData = new DialogueNode()
                },
                new SearchTreeEntry(new GUIContent("Comment Block",_indentationIcon))
                {
                    level = 1,
                    userData = new Group()
                },
                new SearchTreeEntry(new GUIContent("Condition Node", _indentationIcon))
                {
                    level = 1, userData = new ConditionNode()
                },
                new SearchTreeGroupEntry(new GUIContent("Set"), 1),
                new SearchTreeEntry(new GUIContent("Bool", _indentationIcon))
                {
                    level = 2, userData = new SetBoolNode()
                },
                new SearchTreeEntry(new GUIContent("Affinity", _indentationIcon))
                {
                    level = 2, userData = new SetAffinityNode()
                }
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            switch (SearchTreeEntry.userData)
            {
                case DialogueNode dialogueNode:
                    _graphView.CreateNewDialogueNode(0, "Key Text", "Key Speaker",graphMousePosition);
                    return true;
                case ConditionNode conditionNode:
                    _graphView.CreateNewConditionNode("Property", graphMousePosition);
                    return true;
                case Group group:
                    var rect = new Rect(graphMousePosition, _graphView.DefaultCommentBlockSize);
                     _graphView.CreateCommentBlock(rect);
                    return true;
                case SetBoolNode setBoolNode:
                    _graphView.CreateNewSetBoolNode("Property", false, graphMousePosition);
                    return true;
                case SetAffinityNode setAffinityNode:
                    _graphView.CreateNewSetAffinityNode("Target", 0, graphMousePosition);
                    return true;
            }
            return false;
        }
    }
}