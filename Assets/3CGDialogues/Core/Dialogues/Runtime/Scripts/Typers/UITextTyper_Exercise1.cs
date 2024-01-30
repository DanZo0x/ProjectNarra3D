using TMPro;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class UITextTyper_Exercise1 : MonoBehaviour, IUITextTyper
    {
        [SerializeField] private TextMeshProUGUI _textField;
        
#pragma warning disable 0414
        [SerializeField] private int _charactersPerSecond = 5;
#pragma warning restore 0414

        public bool IsReadingText { get; private set; } = false;

#pragma warning disable 0414
        private float _readCharacterOffset = 0;
        private int _readMaxCharacters = 0;
#pragma warning restore 0414

        public TextMeshProUGUI TextField => _textField;

        public void ReadText(string text)
        {
            //_textField.text = "Ouvre le script <color=#822437>UITextTyper_Exercise1</color> pour commencer.";
            //Set Text into TextMeshPro label
            _textField.text = text;
            _textField.ForceMeshUpdate();
            //Use _textField.ForceMeshUpdate. This will be useful for GetParsedText() (will be updated with the valid number of characters)*
            _readCharacterOffset = 0;
            _readMaxCharacters = TextField.GetParsedText().Length;
            //Initialize _readCharacterOffset
            //Initialize _readMaxCharacters (using GetParsedText() from _textField)
            _textField.maxVisibleCharacters = 0;
            //Reset maxVisibleCharacter
            IsReadingText = true;
            //Change IsReadingText boolean flag
        }

        public void GoToEnd()
        {
            //Do nothing if not Reading Text
            if (IsReadingText == false)
            {
                return;
            }
            IsReadingText = false;
            _textField.maxVisibleCharacters = TextField.GetParsedText().Length;
            //Change IsReadingText boolean flag
            //maxVisibleCharacters must be set to max characters available on text
        }

        private void Update()
        {
            _UpdateReadText();
        }

        private void _UpdateReadText()
        {
            //Do nothing if not Reading Text
            if (IsReadingText == false)
            {
                return;
            }
            //Update _readCharacterOffset using _charactersPerSecond (and deltaTime of course ;) )
            Debug.Log(_readCharacterOffset);
            _readCharacterOffset += _charactersPerSecond * Time.deltaTime;
            //Update maxVisibleCharacters using _readCharacterOffset
            _textField.maxVisibleCharacters = (int)_readCharacterOffset;
            //Call GoToEnd if _readCharacterOffset reached number of MaxCharacters
            if (_readCharacterOffset >=_readMaxCharacters)
            {
                GoToEnd();
            }
        }
    }
}