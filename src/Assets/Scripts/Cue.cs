using UnityEngine;

namespace Assets.Scripts
{
    public class Cue : MonoBehaviour
    {
        public bool IsActive;
        public Vector3 Offset;

        public void OnTriggerEnter2D(Collider2D col)
        {
            if(col.tag == "Player") IsActive = true;
        }

        public void OnTriggerExit2D(Collider2D col)
        {
            if(col.tag == "Player") IsActive = false;
        }
    }
}