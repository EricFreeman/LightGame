using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ButtonSwitch : MonoBehaviour
    {
        public List<GameObject> SwitchableObject;

        private Vector3 _initial;
        private bool _canSwitch { get { return _colliders.Count > 0; } }
        private bool _isOn;
        private readonly List<string> _colliders = new List<string>();

        private float _buttonDistance
        {
            get { return _initial.y - transform.position.y; }
        }

        void Start()
        {
            _initial = transform.position;
        }

        void Update()
        {
            if (IsButtonBeingPressed())
                transform.position = new Vector3(transform.position.x, transform.position.y - .01f, transform.position.z);
            else if (IsButtonBeingReleased())
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + .01f, transform.position.z);
                if (_isOn)
                {
                    SwitchableObject.ForEach(x => x.SendMessage("Switch"));
                    _isOn = false;
                }
            }
            else if (IsButtonPressed())
            {
                SwitchableObject.ForEach(x => x.SendMessage("Switch"));
                _isOn = true;
            }
        }

        private bool IsButtonBeingPressed()
        {
            return _canSwitch && _buttonDistance < .1f;
        }

        private bool IsButtonBeingReleased()
        {
            return !_canSwitch && _buttonDistance > 0;
        }

        private bool IsButtonPressed()
        {
            return _canSwitch && _buttonDistance >= .1f && !_isOn;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag != "Floor") _colliders.Add(col.tag);
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag != "Floor") _colliders.Remove(col.tag);
        }
    }
}