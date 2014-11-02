using UnityEngine;

namespace Assets.Scripts
{
    public class Spikes : MonoBehaviour
    {
        public AudioClip ImpaleSoundEffect;

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.tag == "Player")
                AudioSource.PlayClipAtPoint(ImpaleSoundEffect, transform.position);
        }
    }
}