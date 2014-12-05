using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Rope : MonoBehaviour
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public GameObject RopeSection;
        public bool BothSidesConnected;

        // TODO: Figure this out from gameobject and not hardcoded
        private const float Size = .12f;

        private int SegmentCount
        {
            get { return (int)Math.Round(Vector3.Distance(StartPosition, EndPosition) / Size, 0); }
        }

        private List<GameObject> _ropeSegments;

        void Start()
        {
            _ropeSegments = new List<GameObject>();

            for (var i = 0; i < SegmentCount; i++)
            {
                var section = (GameObject)Instantiate(RopeSection);

                if (i == 0 || (BothSidesConnected && i == SegmentCount - 1)) section.rigidbody2D.isKinematic = true;
                if(i != 0) section.GetComponent<HingeJoint2D>().connectedBody = _ropeSegments[i - 1].rigidbody2D;

                section.transform.position = Vector3.Lerp(StartPosition, EndPosition, Size * i);
                section.transform.SetParent(transform);
                _ropeSegments.Add(section);
            }
        }
    }
}