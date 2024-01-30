﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Subtegral.DialogueSystem.DataContainers;
using TCG.Core.Dialogues;
using Subtegral.DialogueSystem.Editor;

namespace Subtegral.DialogueSystem.Runtime
{
    public class DialogueParser : MonoBehaviour
    {
        [SerializeField] private DialogueContainer dialogue;
        [SerializeField] private GameObject dialogueUI;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private Image charaSprite;
        private Image backGroundSprite;
        private Image buttonSprite;
        private DialogConfig dialogConfig;
        List<ExposedProperty> propertyList = new List<ExposedProperty>();
        DateData dateData;
        private void Start()
        {
            
        }

        public void NewDialogue(string dateName)
        {

            dialogConfig = GetComponent<DialogConfig>();
            dateData = DataManager.Instance.FindFromName(dateName);
            dialogConfig.csvDialog = dateData.table;

            dialogue = dateData.dialogue;

            backGroundSprite.sprite = dateData.background;
            

            dialogueUI.SetActive(true);
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
                    else
                    {
                        dialogueUI.SetActive(false);
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
                    else
                    {
                        dialogueUI.SetActive(false);
                    }
                    break;
                case "Condition":
                    Debug.Log("Condition");
                    var conditionNode = dialogue.ConditionNodeData.Find(x => x.NodeGUID == narrativeDataGUID);

                    nextNodeGUID = FindNextNode(narrativeDataGUID, DataManager.Instance.BoolPropertyDict[conditionNode.NodeProperty]);
                    
                    
                    break;
                case "Dialogue":
                    
                    
                    var text = DialogConfig.Instance.table.Find_KEY(dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).NodeKeyText).FR;
                    
                    var typer = dialogueText.GetComponent<UITextTyper>();
                    var speaker = DialogConfig.Instance.speakerDatabases[0].speakerDatas.Find(x => x.id == dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).KeySpeaker);
                    speakerText.font = speaker.font;
                    typer.TextField1.font = speaker.font;
                    charaSprite.sprite = speaker.statuses[dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).SpeakerEmotion].icon;
                    if (speaker != null)
                    {
                        speakerText.text = speaker.label;
                    }
                    
                    var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
                    typer.ReadText(text);
                    //dialogueText.text = ProcessProperties(text);
                    foreach (var choice in choices)
                    {
                        AddButton(choice, nextNodeGUID, speaker.font);
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



        private void AddButton(NodeLinkData choice, string nextNodeGUID, TMP_FontAsset font)
        {
            var newButton = Instantiate(choicePrefab, buttonContainer);
            Button buttonComponent = newButton.transform.Find("BG").GetComponent<Button>();
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = ProcessProperties(choice.PortName);
            newButton.GetComponentInChildren<TextMeshProUGUI>().font = font;
            newButton.transform.Find("BG").GetComponent<Image>().sprite = dateData.buttonSprite;
            
            if (nextNodeGUID != "")
            {
                var nextNode = dialogue.NodeData.Find(x => x.nodeGUID == choice.TargetNodeGUID);
                if (nextNode.nodeType == "Condition")
                {
                    var nextConditionNode = dialogue.ConditionNodeData.Find(x => x.NodeGUID == nextNode.nodeGUID);
                    foreach (var link in dialogue.NodeLinks)
                    {

                        if (link.BaseNodeGUID == nextNode.nodeGUID)
                        {
                            bool value = DataManager.Instance.BoolPropertyDict[nextConditionNode.NodeProperty];
                            if (link.PortName == "True")
                            {
                                if (value)
                                {
                                    nextNodeGUID = link.TargetNodeGUID;
                                    newButton.transform.Find("Chains").gameObject.SetActive(true);
                                }
                                else
                                {
                                    buttonComponent.interactable = false;
                                    newButton.transform.Find("Chains").gameObject.SetActive(true);
                                }
                                
                            }

                            else if (link.PortName == "False" )
                            {
                                if (!value)
                                {
                                    nextNodeGUID = link.TargetNodeGUID;
                                }
                                else
                                {
                                    buttonComponent.interactable = false;
                                    newButton.transform.Find("Chains").gameObject.SetActive(true);
                                }
                                
                            }

                        }
                    }
                }
                buttonComponent.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
            }
            else
            {
                dialogueUI.SetActive(false);
            }

        }

        private string FindNextNode(string baseNodeGUID, bool valueFromCondition)
        {
            string nextNodeGUID = "";
            foreach(var link in dialogue.NodeLinks)
            {

                if(link.BaseNodeGUID == baseNodeGUID )
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