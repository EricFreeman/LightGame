using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Switchable
{
    public class SwitchEnableScript : MonoBehaviour, ISwitchable
    {
        public bool IsOn;
        public string ScriptToEnable;

        private MonoBehaviour _script;

        public void Start()
        {
            _script = (MonoBehaviour)GetComponent(ScriptToEnable);
        }

        public void Switch()
        {
            IsOn = !IsOn;
            _script.enabled = IsOn;
        }
    }
}