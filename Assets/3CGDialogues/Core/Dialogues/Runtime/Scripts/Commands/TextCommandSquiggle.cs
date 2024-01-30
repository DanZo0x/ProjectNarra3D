using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandSquiggle : TextCommand
    {
        private float _shakePower = 1f;
        private float _timer=0;

        public override bool AlwaysUpdated => true;

        public override bool NeedsCacheTextMesh => true;

        public override bool NeedsClosingTag => true;

        
        public override void SetupData(string strCommandData)
        {
            _shakePower = float.Parse(strCommandData, CultureInfo.InvariantCulture);
        }

        public override void OnUpdate()
        {
            _timer += Time.deltaTime;
            for (int i = EnterIndex; i <= ExitIndex; ++i) {
                Vector3 shakeOffset = Vector3.zero;
                shakeOffset.y = Mathf.Sin(_timer*3+i/3) * _shakePower;
                AnimateCharacter(i, shakeOffset, Quaternion.identity, Vector3.one);
            }
            ApplyChangesToMesh();
        }
    }
}