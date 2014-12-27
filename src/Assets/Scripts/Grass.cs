using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class Grass : MonoBehaviour
    {
        public float Length;
        public List<Foliage> GameObjects;
        public float Density;

        void Start()
        {
            var count = Length * Density;
            var rand = new Random();
            
            for (var i = 0; i < count; i++)
            {
                var t = GameObjects.ToList()[rand.Next(0, GameObjects.Count())];
                var obj = (GameObject) Instantiate(t.gameObject);
                obj.transform.position = transform.position + new Vector3(i/Density, 0, 0);
                obj.transform.parent = transform;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Length, 0, 0));
            for (float i = 0; i < Length; i += .1f)
                Gizmos.DrawLine(transform.position + new Vector3(i, 0, 0), transform.position + new Vector3(i, .25f, 0));
        }
    }
}