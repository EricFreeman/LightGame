using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Switch : MonoBehaviour
    {
        public List<GameObject> SwitchableObject;
        public Sprite OnSprite;
        public Sprite OffSprite;

        public bool IsOn;

        private bool _canSwitch;

        void Start()
        {
            if(IsOn) Execute();
        }

        void Update()
        {
            if (_canSwitch && Input.GetKeyDown(KeyCode.LeftShift))
                Execute();
        }

        private void Execute()
        {
            IsOn = !IsOn;
            SwitchableObject.ForEach(x => x.SendMessage("Switch"));
            GetComponent<SpriteRenderer>().sprite = IsOn ? OnSprite : OffSprite;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Player") _canSwitch = true;
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == "Player") _canSwitch = false;
        }
    }
}