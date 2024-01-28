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
        [SerializeField] private Transform speakerText;
        List<ExposedProperty> propertyList = new List<ExposedProperty>();

        private void Start()
        {
            NewDialogue();
            
        }

        public void NewDialogue()
        {
            var narrativeData = dialogue.NodeLinks.First(); //Entrypoint node
            Debug.Log(dialogue.NodeLinks.Last());
            ProceedToNarrative(narrativeData.TargetNodeGUID);
        }

        private void ProceedToNarrative(string narrativeDataGUID)
        {
            
            var text = DialogConfig.Instance.table.Find_KEY(dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).NodeKeyText).FR;
            
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
            dialogueText.text = ProcessProperties(text);
            var buttons = buttonContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }

            foreach (var choice in choices)
            {
                AddButton(choice);
                /*var button = Instantiate(choicePrefab, buttonContainer);
                button.GetComponentInChildren<TextMeshProUGUI>().text = ProcessProperties(choice.PortName);
                button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));*/
            }
        }

        private string ProcessProperties(string text)
        {
            foreach (var exposedProperty in dialogue.ExposedProperties)
            {
                text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue.ToString());
            }
            return text;
        }

        private void AddButton(NodeLinkData choice)
        {
            var button = Instantiate(choicePrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = ProcessProperties(choice.PortName);
            button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
        }

        private void CheckAndProceedNextNode()
        {

        }
    }
}