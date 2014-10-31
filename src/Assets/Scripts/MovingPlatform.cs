using UnityEngine;

namespace Assets.Scripts
{
    public class MovingPlatform : MonoBehaviour
    {
        public bool MoveDirection;
        public float Speed = 3f;

        public Vector3 StartPosition;
        public Vector3 EndPosition;

        void Update()
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                MoveDirection ? StartPosition : EndPosition,
                Speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, StartPosition) < .01f) MoveDirection = false;
            if (Vector3.Distance(transform.position, EndPosition) < .01f) MoveDirection = true;
        }
    }
}