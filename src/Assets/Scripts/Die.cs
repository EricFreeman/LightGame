using System;
using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Die : MonoBehaviour
    {
        public GameObject Blood;
        private DeathType _deathType;
        private bool _isDead;
        private int _remainingBlood = 10;

        private Dictionary<DeathType, Action> _deathActions;

        void Start()
        {
            _deathActions = new Dictionary<DeathType, Action>
            {
                { DeathType.Impale, ImpaleDeath },
                { DeathType.Drown, DrownDeath },
                { DeathType.Default, DefaultDeath },
            };
        }

        void Update()
        {
            if (!_isDead) return;

            _deathActions[_deathType]();
        }

        void ImpaleDeath()
        {
            rigidbody2D.gravityScale = 0;
            Bleed();
        }

        void DrownDeath()
        {
            rigidbody2D.gravityScale = .01f;
        }

        void DefaultDeath()
        {
            Bleed();
        }

        void Bleed()
        {
            if (_remainingBlood-- > 0)
            {
                for (var i = 0; i < 5; i++)
                {
                    var b = (GameObject)Instantiate(Blood);
                    b.transform.position = transform.position;
                    b.rigidbody2D.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 2f)) * 50);
                }
            }
        }

        public void SetDeath(DeathType type)
        {
            if (_isDead) return;

            _isDead = true;
            _deathType = type;
        }
    }
}