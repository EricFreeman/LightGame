using UnityEngine;

namespace Assets.Scripts
{
    public class Respawn : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Player") SpawnPoint.Point = transform.position;
        }
    }

    public static class SpawnPoint
    {
        public static Vector3? Point;
    }
}