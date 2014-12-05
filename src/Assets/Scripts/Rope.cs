using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Rope : MonoBehaviour
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;

        public Rigidbody2D Base;
        public GameObject RopeSection;

        // TODO: Figure this out from gameobject and not hardcoded
        private const float Size = .12f;

        private double SegmentCount
        {
            get { return Math.Round(Vector3.Distance(StartPosition, EndPosition) / Size, 0); }
        }

        private List<GameObject> _ropeSegments;

        void Start()
        {
            _ropeSegments = new List<GameObject>();

            for (var i = 0; i < SegmentCount; i++)
            {
                var section = (GameObject) Instantiate(RopeSection);
                if (i == 0)
                {
                    if (Base == null)
                        section.rigidbody2D.isKinematic = true;
                    else
                        section.GetComponent<HingeJoint2D>().connectedBody = Base;
//                        section.transform.SetParent(Base);
                }
                else
                {
                    section.GetComponent<HingeJoint2D>().connectedBody = _ropeSegments[i - 1].rigidbody2D;
//                    section.transform.SetParent(_ropeSegments[i - 1].transform);
                }

                section.transform.position = Vector3.Lerp(StartPosition, EndPosition, Size * i);
                _ropeSegments.Add(section);
            }
        }
    }
}