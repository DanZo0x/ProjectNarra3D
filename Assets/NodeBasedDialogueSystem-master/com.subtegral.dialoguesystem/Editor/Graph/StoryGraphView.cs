using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using Subtegral.DialogueSystem.DataContainers;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;
using Button = UnityEngine.UIElements.Button;

namespace Subtegral.DialogueSystem.Editor
{
    public class StoryGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();
        private List<string> Emotions = new List<string>()
        {
            "Neutral",
            "Happy",
            "Angry",
            "Flustered",
            "Flabbergasted"
        };
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow;
        
        public StoryGraphView(StoryGraph editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GetEntryPointNodeInstance());

            AddSearchWindow(editorWindow);
        }


        private void AddSearchWindow(StoryGraph editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }


        public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
        {
            if(commentBlockData==null)
                commentBlockData = new CommentBlockData();
            var group = new Group
            {
                autoUpdateGeometry = true,
                title = commentBlockData.Title
            };
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }

        public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
        {
            var localPropertyName = property.PropertyName;
            var localPropertyValue = property.PropertyValue;
            if (!loadMode)
            {
                while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                    localPropertyName = $"{localPropertyName}(1)";
            }

            var item = ExposedProperty.CreateInstance();
            item.PropertyName = localPropertyName;
            item.PropertyValue = localPropertyValue;
            ExposedProperties.Add(item);

            var container = new VisualElement();
            var field = new BlackboardField {text = localPropertyName, typeText = "bool"};
            container.Add(field);

            var toggle = new UnityEngine.UIElements.Toggle("Value:")
            {
                value = localPropertyValue
            };

            toggle.RegisterValueChangedCallback(evt =>
            {
                var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                ExposedProperties[index].PropertyValue = evt.newValue;
            });
            var sa = new BlackboardRow(field, toggle);
            container.Add(sa);
            Blackboard.Add(container);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void CreateNewDialogueNode(int emotionId, string keyText, string keySpeaker, Vector2 position)
        {
            AddElement(CreateDialogueNode(emotionId, keyText, keySpeaker, position));
        }

        public void CreateNewConditionNode(string property, Vector2 position)
        {
            AddElement(CreateConditionNode(property, position));
        }

        public void CreateNewSetBoolNode(string property, bool value, Vector2 position)
        {
            AddElement(CreateSetBoolNode(property, value, position));
        }

        public void CreateNewSetAffinityNode(string target, int value, Vector2 position)
        {
            AddElement(CreateSetAffinityNode(target, value, position));
        }

        public DialogueNode CreateDialogueNode(int emotionId, string keyText, string keySpeaker, Vector2 position)
        {
            var tempDialogueNode = new DialogueNode()
            {
                title = "Dialogue",
                KeyText = keyText,
                KeySpeaker = keySpeaker,
                Emotion = emotionId,
                GUID = Guid.NewGuid().ToString()
            };
            tempDialogueNode.name = "Dialogue";
            tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("DialogueNodeStyle"));
            var inputPort = GetPortInstanceDialogue(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Previous";
            tempDialogueNode.inputContainer.Add(inputPort);
            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize)); //To-Do: implement screen center instantiation positioning


            

            var textField = new TextField("Text Key: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.KeyText = evt.newValue;

            });
            textField.SetValueWithoutNotify(tempDialogueNode.KeyText);
            tempDialogueNode.mainContainer.Add(textField);

            var speakerField = new TextField("Speaker Key:");
            speakerField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.KeySpeaker = evt.newValue;

            });

            
            speakerField.SetValueWithoutNotify(tempDialogueNode.KeySpeaker);
            tempDialogueNode.mainContainer.Add(speakerField);

            var speakerEmotion = new DropdownField("Speaker emotion: ", new List<string> { "Neutral", "Happy", "Angry", "Flustered", "Flabbergasted" }, 0);
           
            speakerEmotion.RegisterValueChangedCallback(evt =>
            {
                
                tempDialogueNode.Emotion = Emotions.IndexOf(evt.newValue);
            });

            speakerEmotion.SetValueWithoutNotify(Emotions[tempDialogueNode.Emotion]);
            tempDialogueNode.mainContainer.Add(speakerEmotion);

            

            var button = new Button(() => { AddChoicePort(tempDialogueNode); })
            {
                text = "Add Choice"
            };
            tempDialogueNode.titleButtonContainer.Add(button);

            /*var generatedPort = GetPortInstanceDialogue(tempDialogueNode, Direction.Output);
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            var outputPortCount = tempDialogueNode.outputContainer.Query("connector").ToList().Count();
            var outputPortName = "Next";


            var outputBasePort = new TextField()
            {
                name = string.Empty,
                value = outputPortName,
                style = { width = 80, height = 20 }
            };
            outputBasePort.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);


            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);
            tempDialogueNode.outputContainer.Add(generatedPort);
            generatedPort.portName = outputPortName;

            tempDialogueNode.RefreshPorts();
            tempDialogueNode.RefreshExpandedState();*/
            return tempDialogueNode;
        }

        public ConditionNode CreateConditionNode(string property, Vector2 position)
        {
            var tempConditionNode = new ConditionNode()
            {
                title = "Condition",
                Property = property,
                
                GUID = Guid.NewGuid().ToString()
            };
            tempConditionNode.name = "Condition";
            tempConditionNode.styleSheets.Add(Resources.Load<StyleSheet>("ConditionNodeStyle"));
            var inputPort = GetPortInstanceCondition(tempConditionNode, Direction.Input, "float", Port.Capacity.Multi);
            inputPort.portName = "Previous";
            tempConditionNode.inputContainer.Add(inputPort);
            tempConditionNode.RefreshExpandedState();
            tempConditionNode.RefreshPorts();
            tempConditionNode.SetPosition(new Rect(position,
                DefaultNodeSize));
            

            var textField = new TextField("Bool");
            textField.RegisterValueChangedCallback(evt =>
            {
                tempConditionNode.Property = evt.newValue;

            });
            textField.SetValueWithoutNotify(tempConditionNode.Property);
            tempConditionNode.mainContainer.Add(textField);

            AddConditionPort(tempConditionNode, true);
            AddConditionPort(tempConditionNode, false);

            
            return tempConditionNode;
        }

        public SetBoolNode CreateSetBoolNode(string property, bool propertyValue, Vector2 position)
        {
            var tempSetNode = new SetBoolNode()
            {
                title = "Set Property",
                Property = property,
                Value = propertyValue,
                GUID = Guid.NewGuid().ToString()
            };
            tempSetNode.name = "SetBool";
            tempSetNode.styleSheets.Add(Resources.Load<StyleSheet>("BoolPropertyNodeStyle"));
            var inputPort = tempSetNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            var inputPortLabel = inputPort.contentContainer.Q<Label>("type");

            inputPort.portName = "Previous";
            tempSetNode.inputContainer.Add(inputPort);
            tempSetNode.RefreshExpandedState();
            tempSetNode.RefreshPorts();
            tempSetNode.SetPosition(new Rect(position,
                DefaultNodeSize));
            var textField = new TextField("Property name: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                tempSetNode.Property = evt.newValue;
                tempSetNode.title = $"Set {textField.value}";
            });
            textField.SetValueWithoutNotify(tempSetNode.Property);
            tempSetNode.mainContainer.Add(textField);

            var toggle = new UnityEngine.UIElements.Toggle("Value: ");
            toggle.RegisterValueChangedCallback(evt =>
            {
                tempSetNode.Value = evt.newValue;
            });
            toggle.SetValueWithoutNotify(tempSetNode.Value);
            tempSetNode.mainContainer.Add(toggle);
            AddSetBoolPort(tempSetNode);
            return tempSetNode;
        }

        public SetAffinityNode CreateSetAffinityNode(string target, int value, Vector2 position)
        {
            var tempSetNode = new SetAffinityNode()
            {
                title = "Add Affinity",
                TargetName = target,
                Value = value,
                GUID = Guid.NewGuid().ToString()
            };
            tempSetNode.name = "SetAffinity";
            tempSetNode.styleSheets.Add(Resources.Load<StyleSheet>("AffinityNodeStyle"));
            var inputPort = tempSetNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            var inputPortLabel = inputPort.contentContainer.Q<Label>("type");
            inputPort.portName = "Previous";
            tempSetNode.inputContainer.Add(inputPort);
            tempSetNode.RefreshExpandedState();
            tempSetNode.RefreshPorts();
            tempSetNode.SetPosition(new Rect(position,
                DefaultNodeSize));
            var targetName = new DropdownField("Target name: ", new List<string> { "Hecat", "Muller", "Lilith", "Noharnaak"}, 0);
            targetName.RegisterValueChangedCallback(evt =>
            {
                tempSetNode.TargetName = evt.newValue;
            });
            targetName.SetValueWithoutNotify(tempSetNode.TargetName);
            tempSetNode.mainContainer.Add(targetName);

            var valueField = new IntegerField("Value: ");
            valueField.RegisterValueChangedCallback(evt =>
            {
                tempSetNode.Value = evt.newValue;
            });
            valueField.SetValueWithoutNotify(tempSetNode.Value);
            tempSetNode.mainContainer.Add(valueField);
            AddSetAffinityPort(tempSetNode);
            return tempSetNode;
        }
        public void AddConditionPort(ConditionNode nodeCache, bool value)
        {
            var generatedPort = nodeCache.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            

            if (value)
            {
                generatedPort.portName = "True";
            }
            else
            {
                generatedPort.portName = "False";
            }

           
            nodeCache.outputContainer.Add(generatedPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();

        }

        public void AddChoicePort(DialogueNode nodeCache, string overriddenPortName = "")
        {
            var generatedPort = GetPortInstanceDialogue(nodeCache, Direction.Output);
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
            var outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;


            var textField = new TextField()
            {
                name = string.Empty,
                value = outputPortName,
                style = {width = 100, height = 20}
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);


            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = outputPortName;
            
            nodeCache.outputContainer.Add(generatedPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        public void AddSetBoolPort(SetBoolNode nodeCache)
        {
            var outputPort = nodeCache.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            var portLabel = outputPort.contentContainer.Q<Label>("type");
            outputPort.portName = "Next";
            nodeCache.outputContainer.Add(outputPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        public void AddSetAffinityPort(SetAffinityNode nodeCache)
        {
            var outputPort = nodeCache.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            var portLabel = outputPort.contentContainer.Q<Label>("type");
            outputPort.portName = "Next";
            nodeCache.outputContainer.Add(outputPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        private void RemovePort(Node node, Port socket)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GetPortInstanceDialogue(DialogueNode node, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        private Port GetPortInstanceCondition(ConditionNode node, Direction nodeDirection, string type,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            Port tempPort = null;
            switch (type)
            {
                case "bool":
                    tempPort = node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(bool));
                    break;
                case "float":
                    tempPort = node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
                    break;
            }
            return tempPort;
            
        }

        private DialogueNode GetEntryPointNodeInstance()
        {
            var nodeCache = new DialogueNode()
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                KeyText = "ENTRYPOINT",
                EntyPoint = true
            };

            var generatedPort = GetPortInstanceDialogue(nodeCache, Direction.Output);
            generatedPort.portName = "Next";
            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;

            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 100, 150));
            return nodeCache;
        }
    }
}