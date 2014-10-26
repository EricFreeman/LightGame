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
        private List<GameObject> _ropeSegments = new List<GameObject>(); 

        void FixedUpdate()
        {
            UpdateJumping();

            if (_isOnRope)
            {
                rigidbody2D.gravityScale = 0;

                // Climb rope
                rigidbody2D.velocity = new Vector2(0, Input.GetAxisRaw("Vertical") * Speed);
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
            if (col.tag == "Rope") _ropeSegments.Remove(col.gameObject);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (_isOnRope || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if(!_ropeSegments.Contains(col.gameObject))
                    _ropeSegments.Add(col.gameObject);
            }
        }

        private void UpdateJumping()
        {
            if ((IsGrounded() || _isOnRope) && Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpForce);
                rigidbody2D.gravityScale = 1;
                _ropeSegments.Clear();
            }
        }

        private bool IsGrounded()
        {
            var hit = Physics2D.Raycast(transform.position - new Vector3(0, .18f), -Vector2.up, .16f);
            return hit.collider != null;
        }
    }
}