using UnityEngine;

namespace Assets.Scripts
{
    public class Switch : MonoBehaviour
    {
        public GameObject SwitchableObject;

        private bool _canSwitch;

        void Update()
        {
            if (_canSwitch && Input.GetKeyDown(KeyCode.LeftShift))
                SwitchableObject.SendMessage("Switch");
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