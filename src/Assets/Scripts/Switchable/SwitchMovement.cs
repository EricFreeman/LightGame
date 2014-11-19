using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Switchable
{
    public class SwitchMovement : MonoBehaviour, ISwitchable
    {
        private bool _isOn;
        private Vector3 _initial;

        public Vector3 EndPosition;
        public float Speed = 1f;

        void Start()
        {
            _initial = transform.localPosition;
        }

        void Update()
        {
            transform.localPosition = transform.localPosition.MoveTowards(_isOn ? EndPosition : _initial, Speed * Time.deltaTime);
        }

        public void Switch()
        {
            _isOn = !_isOn;
        }
    }
}