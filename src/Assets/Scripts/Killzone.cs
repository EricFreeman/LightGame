using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts
{
    public class Killzone : MonoBehaviour
    {
        public DeathType Type;

        void OnTriggerEnter2D(Collider2D col)
        {
            var die = col.GetComponent<Die>();
            if (die == null) return;
            die.SetDeath(Type);
        }
    }
}