using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandTextShake : TextCommand
    {
        private float _shakePower = 1f;

        public override bool AlwaysUpdated => true;

        public override bool NeedsCacheTextMesh => true;

        public override bool NeedsClosingTag => true;

        
        public override void SetupData(string strCommandData)
        {
            _shakePower = float.Parse(strCommandData, CultureInfo.InvariantCulture);
        }

        public override void OnUpdate()
        {
            for (int i = EnterIndex; i <= ExitIndex; ++i) {
                Vector3 shakeOffset = Vector3.zero;
                shakeOffset.x = Random.Range(-1f, 1f)  * _shakePower;
                shakeOffset.y = Random.Range(-1f, 1f)  * _shakePower;
                AnimateCharacter(i, shakeOffset, Quaternion.identity, Vector3.one);
            }
            ApplyChangesToMesh();
        }
    }
}