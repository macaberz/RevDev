using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;


namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(PlayerPhysics))]
    public class PlayerController : MonoBehaviour
    {
        public float Gravity = 20.0f;
        public float Speed = 8.0f;
        public float Acceleration = 12.0f;

        private float _currentSpeed;
        private float _targetSpeed;
        private Vector2 _translation;

        private PlayerPhysics _playerPhysics;

        private void Awake()
        {

        }

        private void Start()
        {
            _playerPhysics = GetComponent<PlayerPhysics>();
        }

        private void Update()
        {
            _targetSpeed = Input.GetAxisRaw("Horizontal") * Speed;
            _currentSpeed = IncreaseTowards(_currentSpeed, _targetSpeed, Acceleration);

            _translation.x = _currentSpeed;
            _translation.y -= Gravity * Time.deltaTime;

            _playerPhysics.Move(_translation * Time.deltaTime);
        }

        private float IncreaseTowards(float f, float target, float rate)
        {
            if (f == target)
            {
                return f;
            }
            else
            {
                float dir = Mathf.Sign(target - f);
                f += rate * Time.deltaTime * dir;
                return (dir >= Mathf.Sign(target - f) ? f : target);
            }
        }

    }
}

