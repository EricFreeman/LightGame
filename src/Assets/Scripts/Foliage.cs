using UnityEngine;

namespace Assets.Scripts
{
    public class Foliage : MonoBehaviour
    {
        public float MinScale = 1;
        public float MaxScale = 1;

        public float MaxHeightLower = .1f;

        public GameObject Sprite;

        void Start()
        {
            Sprite.transform.position -= new Vector3(0, Random.Range(0, MaxHeightLower), 0);
        }
    }
}