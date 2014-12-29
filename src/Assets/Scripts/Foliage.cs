using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Foliage : MonoBehaviour
    {
        public float MinScale = .3f;
        public float MaxScale = 1;

        public float MaxHeightLower = .1f;

        public GameObject Sprite;

        void Start()
        {
            Sprite.transform.position -= new Vector3(0, Random.Range(0, MaxHeightLower), 0);
            var scale = Random.Range(MinScale, MaxScale);
            var isFlipped = Random.Range(0, 2) == 0 ? -1 : 1;
            transform.localScale = new Vector3(scale * isFlipped, scale, scale);
        }

        public void Place(Vector3 startPosition, Vector3 endPosition)
        {
            do
            {
                transform.position = Vector3.MoveTowards(startPosition, endPosition, 
                    Random.Range(0f, Vector3.Distance(startPosition, endPosition)));
            } while (!IsWithinBounds(startPosition, endPosition));
        }

        private bool IsWithinBounds(Vector3 startPosition, Vector3 endPosition)
        {
            var isWithinBounds = true;

            var width = Sprite.renderer.bounds.extents.x;

            if ((transform.position - new Vector3(width, 0, 0)).x < startPosition.x || (transform.position + new Vector3(width, 0, 0)).x > endPosition.x)
                isWithinBounds = false;

            return isWithinBounds;
        }
    }
}