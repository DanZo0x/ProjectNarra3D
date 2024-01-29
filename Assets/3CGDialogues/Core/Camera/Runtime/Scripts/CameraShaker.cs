using UnityEngine;

namespace TCG.Dialogues.Core.Camera
{
    public class CameraShaker : MonoBehaviour
    {
        public static CameraShaker Instance { get; private set; } = null;

        [SerializeField] private float _shakePeriod = 0.05f;

        public bool IsShaking { get; private set; } = false;

        private float _shakePower = 1f;
        private float _shakeDuration = 0f;
        
        private float _shakeTimer = 0f;

        private Vector3 _shakeOffset = Vector3.zero;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            _UpdateShake();
        }

        public void Shake(float power, float duration)
        {
            _shakePower = power;
            _shakeDuration = duration;
            _shakeTimer = 0f;
            IsShaking = true;
        }

        public void ShakeStop()
        {
            transform.position -= _shakeOffset;
            _shakeOffset = Vector3.zero;
            IsShaking = false;
        }

        private void _UpdateShake()
        {
            if (!IsShaking) return;

            transform.position -= _shakeOffset;

            _shakeTimer += Time.deltaTime;

            if (_shakeTimer < _shakeDuration || _shakeDuration < 0f) {
                _shakeOffset.x = (Mathf.PingPong(_shakeTimer, _shakePeriod) / _shakePeriod) * _shakePower;
            } else {
                _shakeOffset = Vector3.zero;
                IsShaking = false;
            }

            transform.position += _shakeOffset;
        }
    }
}