using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Subtegral.DialogueSystem.DataContainers;

namespace Subtegral.DialogueSystem.Runtime
{
    public class DialogueParser : MonoBehaviour
    {
        [SerializeField] private DialogueContainer dialogue;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button choicePrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private Image charaSprite;
        List<ExposedProperty> propertyList = new List<ExposedProperty>();

        private void Start()
        {
            NewDialogue();
        }

        public void NewDialogue()
        {
            var narrativeData = dialogue.NodeLinks.First(); //Entrypoint node
            ProceedToNarrative(narrativeData.TargetNodeGUID);
        }

        private void ProceedToNarrative(string narrativeDataGUID)
        {
            var buttons = buttonContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            var currentNode = dialogue.NodeData.Find(x => x.nodeGUID == narrativeDataGUID);
            var nextNodeGUID = "";
            if (dialogue.NodeLinks.Find(x => x.BaseNodeGUID == currentNode.nodeGUID) != null)
            {
                nextNodeGUID = dialogue.NodeLinks.Find(x => x.BaseNodeGUID == currentNode.nodeGUID).TargetNodeGUID;
            }
            switch (currentNode.nodeType)
            {
                case "SetAffinity":
                    Debug.Log("Affinity");
                    var affinityNode = dialogue.SetAffinityNodeData.Find(x => x.NodeGUID == narrativeDataGUID);
                    var target = DialogConfig.Instance.speakerDatabases[0].speakerDatas.Find(x => x.label == affinityNode.TargetName);
                    target.affection += affinityNode.Value;
                    if (nextNodeGUID != "")
                    {
                        ProceedToNarrative(nextNodeGUID);
                    }

                    break;
                case "SetBool":
                    Debug.Log("Bool");
                    var setBoolNode = dialogue.SetBoolNodeData.Find(x => x.NodeGUID == narrativeDataGUID);
                    DataManager.Instance.BoolPropertyDict[setBoolNode.Property] = setBoolNode.Value;
                    if (nextNodeGUID != "")
                    {
                        ProceedToNarrative(nextNodeGUID);
                    }
                    break;
                case "Condition":
                    Debug.Log("Condition");
                    var conditionNode = dialogue.ConditionNodeData.Find(x => x.NodeGUID == narrativeDataGUID);

                    nextNodeGUID = FindNextNode(narrativeDataGUID, DataManager.Instance.BoolPropertyDict[conditionNode.NodeProperty]);
                    
                    if (nextNodeGUID != "")
                    {
                        ProceedToNarrative(nextNodeGUID);
                    }

                    break;
                case "Dialogue":
                    

                    var text = DialogConfig.Instance.table.Find_KEY(dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).NodeKeyText).FR;
                    var speaker = DialogConfig.Instance.speakerDatabases[0].speakerDatas.Find(x => x.id == dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).KeySpeaker);
                    
                    charaSprite.sprite = speaker.statuses[dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).SpeakerEmotion].icon;
                    if (speaker != null)
                    {
                        speakerText.text = speaker.label;
                    }

                    var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
                    dialogueText.text = ProcessProperties(text);
                    foreach (var choice in choices)
                    {
                        AddButton(choice, nextNodeGUID);
                    }
                    break;

            }
            /*var text = DialogConfig.Instance.table.Find_KEY(dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).NodeKeyText).FR;
            var speaker = DialogConfig.Instance.speakerDatabases[0].speakerDatas.Find(x => x.id == dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).KeySpeaker);
            if(speaker != null)
            {
                speakerText.text = speaker.label;
            }
            
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
            dialogueText.text = ProcessProperties(text);
            foreach (var choice in choices)
            {
                AddButton(choice);
            }*/
        }

        private string ProcessProperties(string text)
        {
            foreach (var exposedProperty in dialogue.ExposedProperties)
            {
                text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue.ToString());
            }
            return text;
        }



        private void AddButton(NodeLinkData choice, string nextNodeGUID)
        {
            var button = Instantiate(choicePrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = ProcessProperties(choice.PortName);
            if(nextNodeGUID != "")
            {
                button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
            }
            
        }

        private string FindNextNode(string baseNodeGUID, bool valueFromCondition)
        {
            string nextNodeGUID = "";
            foreach(var link in dialogue.NodeLinks)
            {
                if(link.BaseNodeGUID == baseNodeGUID)
                {
                    if (valueFromCondition)
                    {
                        if (link.PortName == "True")
                        {
                            nextNodeGUID = link.TargetNodeGUID;
                        }
                    }
                    else
                    {
                        if (link.PortName == "False")
                        {
                            nextNodeGUID = link.TargetNodeGUID;
                        }
                    }
                    
                }
            }
            return nextNodeGUID;
            
        }
    }
}