using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandSquiggle : TextCommand
    {
        private float _shakePower = 1f;
        private float _frequency = 3f;
        private float _timer=0;

        public override bool AlwaysUpdated => true;

        public override bool NeedsCacheTextMesh => true;

        public override bool NeedsClosingTag => true;

        
        public override void SetupData(string strCommandData)
        {
            string[] args = strCommandData.Split("|");
            if (args.Length >= 1)
            {
                _shakePower = float.Parse(args[0], CultureInfo.InvariantCulture);
            }
            if (args.Length >= 2)
            {
                _frequency = float.Parse(args[1], CultureInfo.InvariantCulture);
            }
        }

        public override void OnUpdate()
        {
            _timer += Time.deltaTime;
            for (float i = EnterIndex; i <= ExitIndex; ++i) {
                Vector3 shakeOffset = Vector3.zero;
                shakeOffset.y = Mathf.Sin(_timer*_frequency+i/3) * _shakePower;
                AnimateCharacter((int)i, shakeOffset, Quaternion.identity, Vector3.one);
            }
            ApplyChangesToMesh();
        }
    }
}