using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    Rigidbody myBody;
    public float speed;

    float yBounds;
    float xBounds;
    float yBoundsDown;
    float xBoundsLeft;

    // Use this for initialization
    void Start ()
    {
        myBody = GetComponent<Rigidbody>();

        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -50));
        xBoundsLeft = bounds.x - 10;
        xBounds = -bounds.x + 10;
        yBoundsDown = bounds.y - 10;
        yBounds = -bounds.y + 10;

        myBody.AddForce(transform.up * speed);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position.x < xBoundsLeft)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x > xBounds)
        {
            Destroy(gameObject);
        }

        if (transform.position.y < yBoundsDown)
        {
            Destroy(gameObject);
        }
        else if (transform.position.y > yBounds)
        {
            Destroy(gameObject);
        }
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            if (col.gameObject.GetComponent<PlayerController>() != null)
            {
                col.gameObject.GetComponent<PlayerController>().TakeDamage((int)myBody.velocity.magnitude,null);
            }
            else
            {
                col.gameObject.GetComponent<MotherShip>().TakeDamage((int)myBody.velocity.magnitude);
            }
        }
    }
}
