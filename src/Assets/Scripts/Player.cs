using System;
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

        public GrapplingHook Hook;
        public Climbing Climbing;

        public GameObject Dust;

        private int _dustCounter;
        private bool _isDead;

        public List<AudioClip> Footsteps;

        private readonly Dictionary<PlayerState, Action> _stateActions = new Dictionary<PlayerState, Action>();

        void Start()
        {
            _stateActions.Add(PlayerState.Default, DefaultAction);
            _stateActions.Add(PlayerState.Dead, DeadAction);
            _stateActions.Add(PlayerState.Nothing, NothingAction);

            Hook = GetComponent<GrapplingHook>();
            Climbing = GetComponent<Climbing>();

            if (SpawnPoint.Point.HasValue) transform.position = SpawnPoint.Point.Value;
        }

        void Update()
        {
            _stateActions[DeterminePlayerState()]();
        }

        private PlayerState DeterminePlayerState()
        {
            if (_isDead) return PlayerState.Dead;
            if (Hook.IsEnabled && !IsGrounded()) return PlayerState.Nothing;
            if (Climbing.IsOnRope) return PlayerState.Nothing;

            return PlayerState.Default;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "KillZone")
            {
                _isDead = true;
                rigidbody2D.gravityScale = 0;
                rigidbody2D.velocity = Vector2.zero;
                Climbing.Clear();
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!Climbing.IsOnRope) transform.rotation = new Quaternion();
            if (col.gameObject.tag == "Bouncy") rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 7.5f);
            if (col.gameObject.tag == "Floor" && IsGrounded()) SpawnDust(true);
            if (col.gameObject.tag == "MovingPlatform") transform.SetParent(col.transform);
        }

        private void OnCollisionStay2D(Collision2D col)
        {
            foreach (ContactPoint2D contact in col.contacts)
                Debug.DrawLine(contact.point, contact.point + contact.normal, Color.green, .1f);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            if (col.gameObject.tag == "MovingPlatform") transform.SetParent(null);
        }

        private void DeadAction()
        {
            if(Input.anyKeyDown) Application.LoadLevel(Application.loadedLevel);
        }

        private void DefaultAction()
        {
            rigidbody2D.gravityScale = 1;

            // Horizontal movement
            rigidbody2D.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, rigidbody2D.velocity.y);

            // Jumping
            if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
                Jump();

            // Dust particles from running
            if (Mathf.Abs(rigidbody2D.velocity.x) > 0)
                SpawnDust();
            else
                _dustCounter = 0;
        }

        private void NothingAction() { }

        private void Jump()
        {
            transform.SetParent(null);
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpForce);
            rigidbody2D.gravityScale = 1;
            Climbing.Clear();
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
                Physics2D.Raycast(transform.position - new Vector3(-.12f, .18f), -Vector2.up, .05f),
                Physics2D.Raycast(transform.position - new Vector3(0, .18f), -Vector2.up, .05f),
                Physics2D.Raycast(transform.position - new Vector3(.12f, .18f), -Vector2.up, .05f)
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
                var d = (GameObject)Instantiate(Dust);
                d.transform.position = transform.position - new Vector3(0, .08f, 0);
                _dustCounter = 15;
                AudioSource.PlayClipAtPoint(Footsteps.Random(), transform.position);
            }
        }
    }

    public enum PlayerState
    {
        Default,
        Dead,
        Nothing
    }
}