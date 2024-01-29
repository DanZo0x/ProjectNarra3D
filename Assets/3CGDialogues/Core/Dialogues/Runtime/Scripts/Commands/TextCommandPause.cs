using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandPause : TextCommand
    {
        public override bool IsBlocking => true;

        private float _duration = 0f;
        private float _timer = 0f;

        public override void SetupData(string strCommandData)
        {
            _duration = float.Parse(strCommandData, CultureInfo.InvariantCulture);
        }

        public override void OnEnter()
        {
            _timer = 0f;
            IsRunning = true;
        }

        public override void OnUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration/10) {
                IsRunning = false;
            }
        }
    }
}