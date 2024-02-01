using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandTextPTSD : TextCommand
    {
        private float _shakePower = 1f;
        private float _timer = 0f;

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
            if (_timer % 5 > 2)
            {
                for (int i = EnterIndex; i <= ExitIndex; ++i)
                {
                    Vector3 shakeOffset = Vector3.zero;
                    shakeOffset.x = Random.Range(-1f, 1f) * _shakePower*_timer;
                    shakeOffset.y = Random.Range(-1f, 1f) * _shakePower*_timer;
                    AnimateCharacter(i, shakeOffset, Quaternion.identity, Vector3.one);
                }
                ApplyChangesToMesh();
                if (_timer > 4.5)
                {
                    _timer = 0;
                }
            }
        }
    }
}