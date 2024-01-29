using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class UITextTyper_Exercise2 : MonoBehaviour, IUITextTyper
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
            //TODO: Implements _GenerateCommands
            _commands = _GenerateCommands(text);
            foreach (TextCommand command in _commands) {
                 command.Init(command.Typer);
            }

            //TODO: Implements _RemoveCustomTags
            text = _RemoveCustomTags(text);
            _textField.text = text;
            _textField.ForceMeshUpdate();
            _readCharacterOffset = 0f;
            _readMaxCharacters = _textField.GetParsedText().Length;
            _textField.maxVisibleCharacters = 0;

            foreach (TextCommand command in _commands) {
                command.Init(this);
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
            for (int i = 0; i < commandsToEnter.Length; ++i) {
                TextCommand command = commandsToEnter[i];
                command.OnEnter();
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
            //TODO: Detect Custom tags inside text and generate associated commands
            //You can use :
            //TagsUtils.ExtractTagName(tag)
            //Get tag name.
            //Example : TagsUtils.ExtractTagName("<camshake=0.1|1>") => "camshake"

            //TagsUtils.ExtractTagArgs(tag)
            //Get tag arguments.
            //Example : TagsUtils.ExtractTagName("<camshake=0.1|1>") => "0.1|1"

            //TagsUtils.IsCustomTag(tagName) => detect if custom tag or tag already managed by TextMeshPro
            //Example :
            //TagsUtils.IsCustomTag("camshake") => true
            //TagsUtils.IsCustomTag("color") => false
            //TextCommandsFactory.CreateCommand(tagName) => return TextCommand according to tagName
            //Example : factory.CreateCommand("camshake") => return TextCommandCameraShake

            for (int i = 0; i < text.Length; ++i) {
                char character = text[i];
                switch (character) {
                    case '<':
                        startIndex = i;
                        break;
                    case '>':
                       string tagName= TagsUtils.ExtractTagName(text.Substring(startIndex, i - startIndex));
                       string tagArg= TagsUtils.ExtractTagArgs(text.Substring(startIndex, i - startIndex));
                        if (TagsUtils.IsCustomTag(tagName))
                        {
                            commands.Add(factory.CreateCommand(tagName));

                            commands[commands.Count-1].SetupData(tagArg);
                            commands[commands.Count - 1].TagName = tagName;
                            commands[commands.Count - 1].EnterIndex = startIndex;
                        }

                        break;
                }
            }

            return commands.ToArray();
        }

        private static string _RemoveCustomTags(string text)
        {
            int startIndex= 0;
            //TODO: Remove Custom tags inside text
            //You can use :
            //TagsUtils.ExtractTagName(tag)
            //Get tag name.
            //Example : TagsUtils.ExtractTagName("<camshake=0.1|1>") => "camshake"
            string tagName = TagsUtils.ExtractTagName(text);
            //TagsUtils.IsCustomTag(tagName) => detect if custom tag or tag already managed by TextMeshPro
            //Example :
            //TagsUtils.IsCustomTag("camshake") => true
            //TagsUtils.IsCustomTag("color") => false
            for (int i = 0; i < text.Length; ++i) {
                char character = text[i];
                switch (character)
                {
                    case '<':
                        startIndex = i;
                        break;
                    case '>':
                        text=text.Remove(startIndex, i - startIndex+1);
                        break;

                 }
            }
            return text;
        }
    }
}