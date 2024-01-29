using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandRdmPause : TextCommand
    {
        public override bool IsBlocking => true;

        private float _duration = 0f;
        private float _timer = 0f;
        private float _min = 0f;
        private float _max = 1f;

        public override void SetupData(string strCommandData)
        {
            string[] args = strCommandData.Split("|");
            if (args.Length == 1)
            {
                _max = float.Parse(args[0], CultureInfo.InvariantCulture);
            }
            else
            {
                _min = float.Parse(args[0], CultureInfo.InvariantCulture);
                _max = float.Parse(args[1], CultureInfo.InvariantCulture);
            }
        }

        public override void OnEnter()
        {
            _timer = 0f;
            _duration = Random.Range(_min, _max);
            Debug.Log(_duration);
            IsRunning = true;
        }

        public override void OnUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration / 10)
            {
                IsRunning = false;
            }
        }
    }
}