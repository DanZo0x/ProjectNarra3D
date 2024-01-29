using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class DialoguesManager : MonoBehaviour
    {
        [SerializeField] [TextArea] private string[] _textsToRead;
        [SerializeField] private GameObject _uiTextTyperGameObject;
        private IUITextTyper _uiTextTyper;

        private int _textIndex = 0;

        private void Awake()
        {
            _uiTextTyper = _uiTextTyperGameObject.GetComponent<IUITextTyper>();
        }

        public void Start()
        {
            if (_textsToRead.Length == 0) return;
            _uiTextTyper.ReadText(_textsToRead[_textIndex]);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (!_uiTextTyper.IsReadingText) {
                    _NextText();
                }
            }
        }

        private void _NextText()
        {
            _textIndex++;
            if (_textIndex < _textsToRead.Length) {
                _uiTextTyper.ReadText(_textsToRead[_textIndex]);
            }
        }
    }
}