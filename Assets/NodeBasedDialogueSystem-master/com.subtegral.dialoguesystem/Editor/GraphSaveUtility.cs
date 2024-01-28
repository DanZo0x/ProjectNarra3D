using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine.UIElements;

namespace Subtegral.DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<NodeGraph> Nodes => _graphView.nodes.ToList().Cast<NodeGraph>().ToList();

        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        private DialogueContainer _dialogueContainer;
        private StoryGraphView _graphView;

        public static GraphSaveUtility GetInstance(StoryGraphView graphView)
        {
            return new GraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(fileName, dialogueContainerObject)) return;
            SaveExposedProperties(dialogueContainerObject);
            SaveCommentBlocks(dialogueContainerObject);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset)) 
			{
                AssetDatabase.CreateAsset(dialogueContainerObject, $"Assets/Resources/{fileName}.asset");
            }
            else 
			{
                DialogueContainer container = loadedAsset as DialogueContainer;
                container.NodeLinks = dialogueContainerObject.NodeLinks;
                container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                container.ConditionNodeData = dialogueContainerObject.ConditionNodeData;
                container.SetBoolNodeData = dialogueContainerObject.SetBoolNodeData;
                container.SetAffinityNodeData = dialogueContainerObject.SetAffinityNodeData;
                container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                container.CommentBlockData = dialogueContainerObject.CommentBlockData;
                EditorUtility.SetDirty(container);
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(string fileName, DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as NodeGraph);
                var inputNode = (connectedSockets[i].input.node as NodeGraph);
                dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    PortName = connectedSockets[i].output.portName,
                    TargetNodeGUID = inputNode.GUID
                });
            }


            /*foreach (var node in Nodes.Where(node => !node.EntyPoint))
            {
                dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                {
                    NodeGUID = node.GUID,
                    DialogueText = node.DialogueText,
                    Position = node.GetPosition().position
                });
            }*/
            foreach (var node in Nodes.Where(node => !node.EntyPoint))
            {


                switch (node.name)
                {
                    case "Dialogue":

                        dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                        {
                            NodeGUID = node.GUID,
                            NodeKeyText = ((DialogueNode)node).KeyText,
                            KeySpeaker = ((DialogueNode)node).KeySpeaker,
                            Position = node.GetPosition().position
                        });
                        break;
                    case "Condition":

                        dialogueContainerObject.ConditionNodeData.Add(new ConditionNodeData
                        {
                            NodeGUID = node.GUID,
                            NodeProperty = ((ConditionNode)node).Property,
                            PropertyValue = ((ConditionNode)node).PropertyValue,
                            Position = node.GetPosition().position

                        });
                        break;
                    case "SetBool":

                        dialogueContainerObject.SetBoolNodeData.Add(new SetBoolNodeData
                        {
                            NodeGUID = node.GUID,
                            Property = ((SetBoolNode)node).Property,
                            Value = ((SetBoolNode)node).Value,
                            Position = node.GetPosition().position

                        });
                        break;
                    case "SetAffinity":
                        dialogueContainerObject.SetAffinityNodeData.Add(new SetAffinityData
                        {
                            NodeGUID = node.GUID,
                            TargetName = ((SetAffinityNode)node).TargetName,
                            Value = ((SetAffinityNode)node).Value,
                            Position = node.GetPosition().position

                        });
                        break;
                }
            }
            /*foreach (ConditionNode node in Nodes)
            {
                Debug.Log(node.title);
                dialogueContainerObject.ConditionNodeData.Add(new ConditionNodeData
                {
                    NodeGUID = node.GUID,
                    NodeProperty = node.Property,
                    PropertyValue = node.PropertyValue,
                    Position = node.GetPosition().position

                });
            }
            foreach (SetBoolNode node in Nodes)
            {
                Debug.Log(node.title);
                dialogueContainerObject.SetBoolNodeData.Add(new SetBoolNodeData
                {
                    NodeGUID = node.GUID,
                    Property = node.Property,
                    Value = node.Value,
                    Position = node.GetPosition().position

                });
            }*/

            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        private void SaveCommentBlocks(DialogueContainer dialogueContainer)
        {
            foreach (var block in CommentBlocks)
            {
                var nodes = block.containedElements.Where(x => x is DialogueNode).Cast<DialogueNode>().Select(x => x.GUID)
                    .ToList();

                dialogueContainer.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadNarrative(string fileName)
        {
            _dialogueContainer = Resources.Load<DialogueContainer>(fileName);
            if (_dialogueContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            ClearGraph();
            GenerateNodes();
            ConnectDialogueNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(x => x.EntyPoint).GUID = _dialogueContainer.NodeLinks[0].BaseNodeGUID;
            foreach (var perNode in Nodes)
            {
                if (perNode.EntyPoint) continue;
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(perNode);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void GenerateNodes()
        {
            foreach (var perNode in _dialogueContainer.DialogueNodeData)
            {
                var tempNode = _graphView.CreateDialogueNode(perNode.NodeKeyText, perNode.KeySpeaker, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                
                
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }
            foreach (var perNode in _dialogueContainer.ConditionNodeData)
            {
                var tempNode = _graphView.CreateConditionNode(perNode.NodeProperty, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                tempNode.PropertyValue = perNode.PropertyValue;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                
            }
            foreach (var perNode in _dialogueContainer.SetBoolNodeData)
            {
                var tempNode = _graphView.CreateSetBoolNode(perNode.Property, perNode.Value, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                
            }

            foreach (var perNode in _dialogueContainer.SetAffinityNodeData)
            {
                var tempNode = _graphView.CreateSetAffinityNode(perNode.TargetName, perNode.Value, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();

            }
        }



        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
                    switch (targetNode.name)
                    {
                        case "Dialogue":
                            targetNode.SetPosition(new Rect(
                                _dialogueContainer.DialogueNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                                _graphView.DefaultNodeSize));
                            break;
                        case "Condition":
                            targetNode.SetPosition(new Rect(
                                _dialogueContainer.ConditionNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                                _graphView.DefaultNodeSize));
                            break;
                        case "SetBool":
                            targetNode.SetPosition(new Rect(
                                _dialogueContainer.SetBoolNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                                _graphView.DefaultNodeSize));
                            break;
                        case "SetAffinity":
                            targetNode.SetPosition(new Rect(
                                _dialogueContainer.SetAffinityNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                                _graphView.DefaultNodeSize));
                            break;
                    }
                    /*targetNode.SetPosition(new Rect(
                        _dialogueContainer.DialogueNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                        _graphView.DefaultNodeSize));*/
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _dialogueContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _graphView.RemoveElement(commentBlock);
            }

            foreach (var commentBlockData in _dialogueContainer.CommentBlockData)
            {
               var block = _graphView.CreateCommentBlock(new Rect(commentBlockData.Position, _graphView.DefaultCommentBlockSize),
                    commentBlockData);
               block.AddElements(Nodes.Where(x=>commentBlockData.ChildNodes.Contains(x.GUID)));
            }
        }
    }
}