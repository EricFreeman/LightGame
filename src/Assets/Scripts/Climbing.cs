using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Climbing : MonoBehaviour
    {
        public float Speed = 5;
        public float JumpForce = 20;

        public bool IsOnRope
        {
            get { return _ropeSegments.Any(); }
        }
        private readonly List<GameObject> _ropeSegments = new List<GameObject>();

        private Vector3 _prevPosition;
        private GameObject _prevMiddle;

        private GameObject _moveTowardsCenter;
        private bool _centerPlayer;

        void Update()
        {
            if (IsOnRope)
            {
                rigidbody2D.gravityScale = 0;

                // Jump off rope
                if (IsOnRope && Input.GetKeyDown(KeyCode.Space))
                    Jump();

                // Grab the middle rope segment player collides with since this would be closest to player's center
                var all = _ropeSegments.OrderByDescending(x => x.transform.position.y);
                GameObject middle = null;
                if (all.Any()) middle = _ropeSegments[_ropeSegments.Count/2];

                if (middle != null)
                {
                    if (_centerPlayer)
                    {
                        transform.position = new Vector3(middle.transform.position.x, transform.position.y,
                            transform.position.z);
                        _centerPlayer = false;
                    }

                    // Climb rope
                    rigidbody2D.velocity = CanMoveUp(middle)
                        ? new Vector2(
                            middle.transform.up.x*Input.GetAxisRaw("Vertical"),
                            Input.GetAxisRaw("Vertical")*Speed*middle.transform.up.y)
                        : new Vector2(rigidbody2D.velocity.x, 0);

                    // Swing on rope
                    middle.rigidbody2D.AddForce(new Vector2(Input.GetAxisRaw("Horizontal")*5, 0));
                    transform.rotation = middle.transform.rotation;

                    // Move player with swing of rope
                    if (_prevMiddle == middle) transform.position += middle.transform.position - _prevPosition;
                    _prevMiddle = middle;
                    _prevPosition = middle.transform.position;
                }

                // Move towards middle of rope if you've somehow moved off of it
                if (_moveTowardsCenter != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        _moveTowardsCenter.transform.position - _moveTowardsCenter.transform.up*.24f, .1f);

                    if (Vector3.Distance(transform.position, _moveTowardsCenter.transform.position) < 1)
                        _moveTowardsCenter = null;
                }
            }
        }

        private bool CanMoveUp(GameObject middle)
        {
            if (Input.GetAxisRaw("Vertical") > 0) return true;

            return _ropeSegments.Count > 2;
        }

        private void Jump()
        {
            transform.SetParent(null);
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpForce);
            rigidbody2D.gravityScale = 1;
            _ropeSegments.Clear();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Rope")
            {
                if (_ropeSegments.Count() == 1 && col.gameObject != _moveTowardsCenter)
                {
                    _moveTowardsCenter = null;
                    _ropeSegments.Add(col.gameObject);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == "Rope")
            {
                if (_ropeSegments.Count > 1)
                    _ropeSegments.Remove(col.gameObject);
                else if (_ropeSegments.Count == 1)
                    _moveTowardsCenter = _ropeSegments[0].gameObject;
            }
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if ((IsOnRope || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && col.tag == "Rope")
            {
                if (!IsOnRope) _centerPlayer = true;
                if (!_ropeSegments.Contains(col.gameObject))
                    _ropeSegments.Add(col.gameObject);
            }
        }

        public void Clear()
        {
            _ropeSegments.Clear();
        }
    }
}