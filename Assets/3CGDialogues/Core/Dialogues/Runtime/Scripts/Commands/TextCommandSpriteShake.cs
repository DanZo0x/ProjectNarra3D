using System.Globalization;
using TCG.Dialogues.Core.Camera;

namespace TCG.Core.Dialogues
{
    public class TextCommandSpriteShake : TextCommand
    {
        public override bool NeedsClosingTag => true;

        private float _shakePower = 0f;
        private float _shakeDuration = -1f;
        
        public override void SetupData(string strCommandData)
        {
            string[] args = strCommandData.Split("|");
            if (args.Length >= 1) {
                _shakePower = float.Parse(args[0], CultureInfo.InvariantCulture);
            }
            if (args.Length >= 2) {
                _shakeDuration = float.Parse(args[1], CultureInfo.InvariantCulture);
            }
        }

        public override void OnEnter()
        {
            SpriteShaker.Instance.Shake(_shakePower, _shakeDuration);
        }

        public override void OnExit()
        {
            if (_shakeDuration < 0f) {
                SpriteShaker.Instance.ShakeStop();
            }
        }

        public override void OnReadEnd()
        {
            if (_shakeDuration > 0f) {
                SpriteShaker.Instance.ShakeStop();
            }
        }
    }
}