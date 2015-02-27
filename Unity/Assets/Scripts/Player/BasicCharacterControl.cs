using UnityEngine;
using System.Collections;

public class BasicCharacterControl : MonoBehaviour
{
    public float moveForce = 3f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	if(Input.GetKey(KeyCode.A))
    {
        rigidbody.velocity = new Vector2(moveForce,rigidbody.velocity.y);
    }
    else if(Input.GetKey(KeyCode.D))
    {
        rigidbody.velocity = new Vector2(-moveForce,rigidbody.velocity.y);
    }
    else
    {
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

	}

    void OnCollisionEnter()
    {

        Debug.Log(collider);
    }

}
