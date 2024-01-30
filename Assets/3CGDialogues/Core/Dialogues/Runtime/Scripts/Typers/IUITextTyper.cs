using TMPro;

namespace TCG.Core.Dialogues
{
    public interface IUITextTyper
    {
        TextMeshProUGUI TextField { get; }
        
        bool IsReadingText { get; }
        
        void ReadText(string text);
    }
}