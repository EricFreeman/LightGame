using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [HideInInspector]
        public List<Cue> Cues;

        public float Speed = .5f;

        void Start()
        {
            Cues = FindObjectsOfType<Cue>().ToList();
        }

        void Update()
        {
            var current = Cues.FirstOrDefault(x => x.IsActive);
            var position = current != null ? current.Offset : Vector3.zero;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, position, Speed * Time.deltaTime);
        }
    }
}