using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ButtonSwitch : MonoBehaviour
    {
        public List<GameObject> SwitchableObject;

        private Vector3 _initial;
        private bool _canSwitch;
        private bool _hasSentMessage;

        void Start()
        {
            _initial = transform.position;
        }

        void Update()
        {
            if (_canSwitch && _initial.y - transform.position.y < .1f)
                transform.position = new Vector3(transform.position.x, transform.position.y - .01f, transform.position.z);
            else if (!_canSwitch && _initial.y - transform.position.y > 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + .01f, transform.position.z);
                _hasSentMessage = false;
            }
            else if (_canSwitch && _initial.y - transform.position.y >= .1f && !_hasSentMessage)
            {
                SwitchableObject.ForEach(x => x.SendMessage("Switch"));
                _hasSentMessage = true;
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag != "Floor") _canSwitch = true;
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag != "Floor") _canSwitch = false;
        }
    }
}