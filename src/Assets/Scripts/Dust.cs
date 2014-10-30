using UnityEngine;

namespace Assets.Scripts
{
    public class Dust : MonoBehaviour
    {
        private Material _material
        {
            get { return GetComponent<SpriteRenderer>().material; }
        }

        void Update()
        {
            transform.localScale *= 1.0125f;
            _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, _material.color.a - .01f);

            if(_material.color.a <= 0) Destroy(gameObject);
        }
    }
}