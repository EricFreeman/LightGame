using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public float Speed = 5;
        public float JumpForce = 20;

        private bool _isOnRope
        {
            get { return _ropeSegments.Any(); }
        }
        private readonly List<GameObject> _ropeSegments = new List<GameObject>();

        private Vector3 _prevPosition;
        private GameObject _prevHighest;

        void FixedUpdate()
        {
            UpdateJumping();

            if (_isOnRope)
            {
                rigidbody2D.gravityScale = 0;

                // Climb rope
                rigidbody2D.velocity = new Vector2(0, Input.GetAxisRaw("Vertical") * Speed);

                // Swing on rope
                var highest = _ropeSegments.OrderByDescending(x => x.transform.position.y).FirstOrDefault();
                if (highest != null)
                {
                    transform.SetParent(highest.transform);
                    highest.rigidbody2D.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * 5, 0));
                    transform.rotation = highest.transform.rotation;

                    if (_prevHighest == highest) transform.position += highest.transform.position - _prevPosition;
                    _prevHighest = highest;
                    _prevPosition = highest.transform.position;
                }
            }
            else
            {
                rigidbody2D.gravityScale = 1;

                // Horizontal movement
                rigidbody2D.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, rigidbody2D.velocity.y);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == "Rope" && _ropeSegments.Count > 1) _ropeSegments.Remove(col.gameObject);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (_isOnRope || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if(!_ropeSegments.Contains(col.gameObject))
                    _ropeSegments.Add(col.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if(!_isOnRope) transform.rotation = col.collider.transform.rotation;
            if(col.gameObject.tag == "Bouncy") rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 7.5f);
        }

        private void UpdateJumping()
        {
            if ((IsGrounded() || _isOnRope) && Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpForce);
                rigidbody2D.gravityScale = 1;
                transform.SetParent(null);
                _ropeSegments.Clear();
            }
        }

        private bool IsGrounded()
        {
            var hit = Physics2D.Raycast(transform.position - new Vector3(0, .18f), -Vector2.up, .05f);
            return hit.collider != null;
        }
    }
}