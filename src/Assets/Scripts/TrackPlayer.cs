using UnityEngine;

namespace Assets.Scripts
{
    public class TrackPlayer : MonoBehaviour
    {
        public Player Player;
        public Vector3 Offset;

        void Update()
        {
            if (Player == null) Player = FindObjectOfType<Player>();
            else transform.position = Player.transform.position + Offset;
        }
    }
}