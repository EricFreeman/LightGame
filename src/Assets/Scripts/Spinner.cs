using UnityEngine;

namespace Assets.Scripts
{
    public class Spinner : MonoBehaviour
    {
        public float RotationSpeed;

        void Update()
        {
            transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
        }
    }
}