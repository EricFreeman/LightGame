using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public float Speed = 5;
        public float JumpForce = 20;

        private bool _isGrounded;

        void FixedUpdate()
        {
            _isGrounded = VerifyIsGrounded();

            // Horizontal movement
            rigidbody2D.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, rigidbody2D.velocity.y);

            // Jumping
            if (_isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)))
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpForce);
        }

        private bool VerifyIsGrounded()
        {
            var hit = Physics2D.Raycast(transform.position - new Vector3(0, .16f), -Vector2.up, .2f);
            return hit.collider != null;
        }
    }
}