using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerPhysics : MonoBehaviour
    {
        public LayerMask CollisionMask;

        private BoxCollider2D _collider;
        private Vector2 _colliderSize;
        private Vector2 _colliderCenter;

        private Ray2D _ray;
        private RaycastHit2D _hit;
        private const float Skin = 0.005f;
        private bool _grounded;


        void Start()
        {
            _collider = GetComponent<BoxCollider2D>();
            _colliderSize = _collider.size;
            _colliderCenter = _collider.center;
        }

        public void Move(Vector2 tranlsation)
        {
            float deltaY = tranlsation.y;
            float deltaX = tranlsation.x;

            Vector2 playerPosition = transform.position;

            for (int i = 0; i < 3; i++)
            {
                float dir = Mathf.Sign(deltaY);
                float x = (playerPosition.x + _colliderCenter.x - _colliderSize.x/2) + _colliderSize.x / 2 * i;
                float y = playerPosition.y + _colliderCenter.y + _colliderSize.y / 2 * dir;

                _ray = new Ray2D(new Vector2(x,y), new Vector2(0, dir));
                _hit = Physics2D.Raycast(_ray.origin, _ray.direction, Mathf.Abs(deltaY), CollisionMask);

                if (_hit.collider != null)
                {
                    float distanceToGround = Vector2.Distance(_ray.origin, _hit.point);
                    if (distanceToGround > Skin)
                    {
                        deltaY -= distanceToGround * dir + Skin;
                    }
                    else
                    {
                        deltaY = 0;
                    }
                    _grounded = true;
                    break;
                }
            }

            Vector2 finalTranslation = new Vector2(deltaX, deltaY);
            transform.Translate(finalTranslation);
        }

        public bool IsGrounded
        {
            get { return _grounded; }
        }
    }
}