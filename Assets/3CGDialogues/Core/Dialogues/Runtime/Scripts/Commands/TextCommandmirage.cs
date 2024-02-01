using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandMirage : TextCommand
    {
        private float _shakePowerx = 1f;
        private float _shakePowery = 1f;
        private float _timer=0;

        public override bool AlwaysUpdated => true;

        public override bool NeedsCacheTextMesh => true;

        public override bool NeedsClosingTag => true;

        
        public override void SetupData(string strCommandData)
        {
            string[] args = strCommandData.Split("|");
            if (args.Length >= 1)
            {
                _shakePowerx = float.Parse(args[0], CultureInfo.InvariantCulture);
            }
            if (args.Length >= 2)
            {
                _shakePowery = float.Parse(args[1], CultureInfo.InvariantCulture);
            }
        }

        public override void OnUpdate()
        {
            _timer += Time.deltaTime;
            for (int i = EnterIndex; i <= ExitIndex; ++i) {
                Vector3 shakeOffset = Vector3.zero;

                shakeOffset.x = Mathf.Sin(_timer*3) * _shakePowerx;
                shakeOffset.y = Mathf.Sin(_timer * 3 + i / 3) * _shakePowery;
                AnimateCharacter(i, shakeOffset, Quaternion.identity, Vector3.one);
            }
            ApplyChangesToMesh();
        }
    }
}