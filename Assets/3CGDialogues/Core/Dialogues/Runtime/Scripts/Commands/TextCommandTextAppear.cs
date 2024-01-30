using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandTextAppear : TextCommand
    {
        private float _appearSpeed = 1f;
        private float _timer = 0f;

        public override bool AlwaysUpdated => true;

        public override bool NeedsCacheTextMesh => true;

        public override bool NeedsClosingTag => true;

        
        public override void SetupData(string strCommandData)
        {
            _appearSpeed = float.Parse(strCommandData, CultureInfo.InvariantCulture);
        }
        public override void OnUpdate()
        {
            if (_timer < ExitIndex+_appearSpeed)
            {
                _timer += Time.deltaTime;
                for (int i = EnterIndex; i <= ExitIndex; ++i)
                {
                    int a = (int)Mathf.Lerp(0, 255, (_timer / _appearSpeed)-i);
                    setCharAlpha(i, a);
                }
                ApplyChangesToMesh();
            }
        }
    }
}