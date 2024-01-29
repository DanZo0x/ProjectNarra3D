using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class UITextTyper_Exercise3 : MonoBehaviour, IUITextTyper
    {
        [SerializeField] private TextMeshProUGUI _textField;
        [SerializeField] private int _charactersPerSecond = 5;

        public bool IsReadingText { get; private set; } = false;

        private float _readCharacterOffset = 0;
        private int _readMaxCharacters = 0;

        private TextCommand[] _commands;

        public TextMeshProUGUI TextField => _textField;

        public void ReadText(string text)
        {
            //TODO: Manage open and closing tags inside GenerateCommands
            //Example <camshake=0.2>BOO!</camshake> instead of <camshake=0.2|0.1>BOO!
            _commands = _GenerateCommands(text);
            foreach (TextCommand command in _commands) {
                command.Init(this);
            }

            //TODO: Implements _RemoveCustomTags
            text = _RemoveCustomTags(text);
            _textField.text = text;
            _textField.ForceMeshUpdate();
            _readCharacterOffset = 0f;
            _readMaxCharacters = _textField.GetParsedText().Length;
            _textField.maxVisibleCharacters = 0;

            foreach (TextCommand command in _commands) {
                command.OnReadStart();
            }

            IsReadingText = true;
        }

        public void GoToEnd()
        {
            if (!IsReadingText) return;
            IsReadingText = false;
            _textField.maxVisibleCharacters = _readMaxCharacters;
            foreach (TextCommand command in _commands) {
                command.OnReadEnd();
            }
        }

        private void Update()
        {
            _UpdateReadText();
        }

        private void _UpdateReadText()
        {
            if (!IsReadingText) return;

            float startOffset = _readCharacterOffset;
            float endOffset = startOffset + _charactersPerSecond * Time.deltaTime;

            int startIndex = Mathf.FloorToInt(startOffset);
            int endIndex = Mathf.FloorToInt(endOffset);

            TextCommand[] commandsToEnter = TextCommandUtils.FindCommandsToEnter(_commands, startIndex, endIndex);
            foreach (TextCommand command in commandsToEnter) {
                command.OnEnter();
            }

            TextCommand[] commandsToExit = TextCommandUtils.FindCommandsToExit(_commands, startIndex, endIndex);
            foreach (TextCommand command in commandsToExit) {
                command.OnExit();
            }

            _GoToCharacter(endOffset);
            if (_readCharacterOffset >= _readMaxCharacters) {
                GoToEnd();
            }
        }

        private void _GoToCharacter(float characterOffset)
        {
            _readCharacterOffset = characterOffset;
            _textField.maxVisibleCharacters = Mathf.FloorToInt(_readCharacterOffset);
        }

        private static TextCommand[] _GenerateCommands(string text)
        {
            int startIndex = 0;
            TextCommandsFactory factory = new TextCommandsFactory();
            List<TextCommand> commands = new List<TextCommand>();
            //TODO: Copy from Exercise 2 + Manage closing tags
            //Example <camshake=0.2>BOO!</camshake> instead of <camshake=0.2|0.1>BOO!
            for (int i = 0; i < text.Length; ++i)
            {
                char character = text[i];
                switch (character)
                {
                    case '<':

                        startIndex = i;
                        break;
                    case '>':
                        string tagName = TagsUtils.ExtractTagName(text.Substring(startIndex, i - startIndex));
                        string tagArg = TagsUtils.ExtractTagArgs(text.Substring(startIndex, i - startIndex));
                        
                        if (TagsUtils.IsCustomTag(tagName))
                        {
                            if (!commands.Contains(commands.Find(x => x.TagName == tagName)))
                            {
                                TextCommand command = factory.CreateCommand(tagName);
                                if (tagArg.Contains('|'))
                                    command.SetupData(tagArg);
                                else
                                    command.SetupData(tagArg+"|10");

                                command.TagName = tagName;
                                command.EnterIndex = startIndex;

                                commands.Add(command);
                            }
                            else
                            {
                                commands.Find(x => x.TagName==tagName).ExitIndex = startIndex-1;                            }
                        }
                        break;
                }
            }

            return commands.ToArray();
        }

        private static string _RemoveCustomTags(string text)
        {
            int startIndex = 0;

            //TODO: Copy From Exercise 2
            string tagName = TagsUtils.ExtractTagName(text);
            for (int i = 0; i < text.Length; ++i)
            {
                char character = text[i];
                switch (character)
                {
                    case '<':
                        startIndex = i;
                        break;
                    case '>':
                        text = text.Remove(startIndex, i - startIndex + 1);
                        break;

                }
            }
            return text;
        }
    }
}