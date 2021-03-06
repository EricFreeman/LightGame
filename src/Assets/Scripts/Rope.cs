﻿using System;
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

        public GameObject ConnectedObject;

        // TODO: Figure this out from gameobject and not hardcoded
        private const float Size = .115f;

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
                if (ConnectedObject != null && i == SegmentCount - 1)
                {
                    var hinge = ConnectedObject.AddComponent<HingeJoint2D>();
                    hinge.connectedBody = section.rigidbody2D;
                    hinge.useLimits = true;
                    hinge.limits = new JointAngleLimits2D { min = 0, max = 0 };
                };

                section.transform.SetParent(transform);
                section.transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, Size * i);
                _ropeSegments.Add(section);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + StartPosition, transform.position + EndPosition);
        }
    }
}