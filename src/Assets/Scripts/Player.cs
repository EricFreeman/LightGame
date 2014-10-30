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

        public GameObject Dust;
        public GameObject Blood;

        private int _dustCounter;
        private bool _isDead;
        private int _remainingBlood = 10;

        void FixedUpdate()
        {
            if (_isDead) 
            {
                if (_remainingBlood-- > 0)
                    for (var i = 0; i < 5; i++)
                    {
                        var b = (GameObject)Instantiate(Blood);
                        b.transform.position = transform.position;
                        b.rigidbody2D.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 2f)) * 50);
                    }
                return;
            }

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

                if (Mathf.Abs(rigidbody2D.velocity.x) > 0)
                    SpawnDust();
                else
                    _dustCounter = 0;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "KillZone")
            {
                _isDead = true;
                rigidbody2D.gravityScale = 0;
                rigidbody2D.velocity = Vector2.zero;
                transform.SetParent(null);
                _ropeSegments.Clear();
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
            if(col.gameObject.tag == "Floor") SpawnDust(true);
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
            // Check if right side of player or left side of player is touching.
            // This will account for being slightly over a ledge or something.
            RaycastHit2D[] hits =
            {
                Physics2D.Raycast(transform.position - new Vector3(-.16f, .18f), -Vector2.up, .05f),
                Physics2D.Raycast(transform.position - new Vector3(.16f, .18f), -Vector2.up, .05f)
            };

            return hits.Where(x => x.collider != null).Any(x => x.collider.tag != "Player");
        }

        private void SpawnDust(bool isOverride = false)
        {
            if (isOverride)
                _dustCounter = 0;
            else
            {
                var hit = Physics2D.Raycast(transform.position - new Vector3(0, .18f), -Vector2.up, .05f);
                if (hit.collider == null || hit.collider.tag != "Floor") return;
            }

            _dustCounter--;
            if (_dustCounter <= 0)
            {
                var d = (GameObject) Instantiate(Dust);
                d.transform.position = transform.position - new Vector3(0, .08f, 0);
                _dustCounter = 15;
            }
        }
    }
}