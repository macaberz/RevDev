using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerControllerScript : MonoBehaviour
{
    public float MaxSpeed = 10.0f;
    public float JumpForce = 700.0f;
    private bool _facingRight = true;

    private bool _grounded = false;
    public Transform GroundCheck;
    private float _groundRadius = 0.2f;
    public LayerMask GroundMask;

    private bool _hasDoubleJumped;

    private Animator _animator;

	// Use this for initialization
	void Start ()
	{
	    _animator = GetComponent<Animator>();
	}

    void FixedUpdate()
    {
        _grounded = Physics2D.OverlapCircle(GroundCheck.position, _groundRadius, GroundMask);
        _animator.SetBool("ground", _grounded);

        if (_grounded)
            _hasDoubleJumped = false;

        _animator.SetFloat("verticalSpeed", rigidbody2D.velocity.y);

        float translationX = Input.GetAxis("Horizontal");

        _animator.SetFloat("speed", Mathf.Abs(translationX));

        // Time.deltaTime not needed as fixedUpdate() is in sync with deltaTime
        rigidbody2D.velocity = new Vector2(translationX * MaxSpeed, rigidbody2D.velocity.y);

        if (translationX > 0 && !_facingRight)
            Flip();
        else if (translationX < 0 && _facingRight)
            Flip();
    }

    void Update()
    {
        if ((_grounded || !_hasDoubleJumped) && Input.GetButtonDown("Jump"))
        {
            _animator.SetBool("ground", false);
            rigidbody2D.AddForce(new Vector2(0, JumpForce));
            if (!_hasDoubleJumped && !_grounded)
                _hasDoubleJumped = true;
        }
    }

    void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
