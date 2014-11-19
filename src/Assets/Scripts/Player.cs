using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
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

        private GameObject _moveTowardsCenter;

        private GameObject _dragObject;
        public Vector3 _dragOffset;

        public List<AudioClip> Footsteps;

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

                var all = _ropeSegments.OrderByDescending(x => x.transform.position.y);
                GameObject middle = null;
                if (all.Any()) middle = _ropeSegments[_ropeSegments.Count / 2];

                if (middle != null)
                {
                    if (_centerPlayer)
                    {
                        transform.position = new Vector3(middle.transform.position.x, transform.position.y, transform.position.z);
                        _centerPlayer = false;
                    }

                    // Climb rope
                    rigidbody2D.velocity = CanMoveUp(middle) ?
                        new Vector2(
                            middle.transform.up.x * Input.GetAxisRaw("Vertical"),
                            Input.GetAxisRaw("Vertical") * Speed * middle.transform.up.y) :
                        new Vector2(rigidbody2D.velocity.x, 0);

                    // Swing on rope
                    middle.rigidbody2D.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * 5, 0));
                    transform.rotation = middle.transform.rotation;

                    if (_prevHighest == middle) transform.position += middle.transform.position - _prevPosition;
                    _prevHighest = middle;
                    _prevPosition = middle.transform.position;
                }

                // Move towards middle of rope if you've somehow moved off of it
                if (_moveTowardsCenter != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        _moveTowardsCenter.transform.position - _moveTowardsCenter.transform.up * .24f, .1f);

                    if (Vector3.Distance(transform.position, _moveTowardsCenter.transform.position) < 1)
                        _moveTowardsCenter = null;
                }
            }
            else
            {
                rigidbody2D.gravityScale = 1;

                // Horizontal movement
                rigidbody2D.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, rigidbody2D.velocity.y);

                // Drag objects
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (_dragObject == null)
                    {
                        var left = Physics2D.Raycast(transform.position - new Vector3(.16f, 0, 0), -Vector2.right, .01f);
                        var right = Physics2D.Raycast(transform.position + new Vector3(.16f, 0, 0), Vector2.right, .01f);

                        var obj = (left.collider != null && left.collider.tag == "Draggable")
                            ? left.collider
                            : (right.collider != null && right.collider.tag == "Draggable") ? right.collider : null;

                        if (obj != null)
                        {
                            _dragObject = obj.gameObject;
                            _dragOffset = (transform.position - _dragObject.transform.position).x > 0 ? new Vector3(.32f, 0, 0) : new Vector3(-.32f, 0, 0);
                        }
                    }
                    else
                    {
                        _dragObject.transform.position = transform.position - _dragOffset;
                    }
                }
                else
                {
                    _dragObject = null;
                }

                // Dust particles from running
                if (Mathf.Abs(rigidbody2D.velocity.x) > 0)
                    SpawnDust();
                else
                    _dustCounter = 0;
            }
        }

        private bool CanMoveUp(GameObject middle)
        {
            if (Input.GetAxisRaw("Vertical") > 0) return true;

            return _ropeSegments.Count > 2;
        }

        private bool _centerPlayer;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "KillZone")
            {
                _isDead = true;
                rigidbody2D.gravityScale = 0;
                rigidbody2D.velocity = Vector2.zero;
                _ropeSegments.Clear();
            }
            else if (col.tag == "Rope")
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
                else if(_ropeSegments.Count == 1)
                    _moveTowardsCenter = _ropeSegments[0].gameObject;
            }
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (_isOnRope || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && col.tag == "Rope")
            {
                if (!_isOnRope) _centerPlayer = true;
                if(!_ropeSegments.Contains(col.gameObject))
                    _ropeSegments.Add(col.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if(!_isOnRope) transform.rotation = new Quaternion();
            if(col.gameObject.tag == "Bouncy") rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 7.5f);
            if(col.gameObject.tag == "Floor") SpawnDust(true);
            if(col.gameObject.tag == "MovingPlatform") transform.SetParent(col.transform);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            if (col.gameObject.tag == "MovingPlatform") transform.SetParent(null);
        }

        private void UpdateJumping()
        {
            if ((IsGrounded() || _isOnRope) && Input.GetKeyDown(KeyCode.Space))
            {
                transform.SetParent(null);
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpForce);
                rigidbody2D.gravityScale = 1;
                _ropeSegments.Clear();
            }
        }

        private bool IsGrounded()
        {
            return GetRaycastHits().Any();
        }

        private IEnumerable<RaycastHit2D> GetRaycastHits()
        {
            // Check if right side of player or left side of player is touching.
            // This will account for being slightly over a ledge or something.
            RaycastHit2D[] hits =
            {
                Physics2D.Raycast(transform.position - new Vector3(-.15f, .18f), -Vector2.up, .05f),
                Physics2D.Raycast(transform.position - new Vector3(0, .18f), -Vector2.up, .05f),
                Physics2D.Raycast(transform.position - new Vector3(.15f, .18f), -Vector2.up, .05f)
            };
            return hits.Where(x => x.collider != null && x.collider.tag != "Player");
        }

        private void SpawnDust(bool isOverride = false)
        {
            if (isOverride)
                _dustCounter = 0;
            else
            {
                var hit = Physics2D.Raycast(transform.position - new Vector3(0, .18f), -Vector2.up, .05f);
                if (hit.collider == null /*|| hit.collider.tag != "Floor"*/) return;
            }

            _dustCounter--;
            if (_dustCounter <= 0)
            {
                var d = (GameObject) Instantiate(Dust);
                d.transform.position = transform.position - new Vector3(0, .08f, 0);
                _dustCounter = 15;
                AudioSource.PlayClipAtPoint(Footsteps.Random(), transform.position);
            }
        }
    }
}