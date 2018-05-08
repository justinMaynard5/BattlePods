using UnityEngine;
using System.Collections;
using PicaVoxel;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
	Rigidbody myBody;
	public float speed;
	public bool exploding;
	public float fuse;
    public int damage;
	public ExplosionPhysicsForce explosion;
    public PlayerController owner;

    float yBounds;
	float xBounds;
	float yBoundsDown;
	float xBoundsLeft;
	// Use this for initialization
	void Start ()
	{
		myBody = GetComponent<Rigidbody>();

		Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -50));
		xBoundsLeft = bounds.x;
		xBounds = -bounds.x;
		yBoundsDown = bounds.y;
		yBounds = -bounds.y;

		myBody.AddForce(transform.up * speed);

		
		StartCoroutine(Explode());
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (transform.position.x < xBoundsLeft)
		{
			Vector3 temp = transform.position;
			temp.x = xBounds;
			transform.position = temp;
		}
		else if (transform.position.x > xBounds)
		{
			Vector3 temp = transform.position;
			temp.x = xBoundsLeft;
			transform.position = temp;
		}

		if (transform.position.y < yBoundsDown)
		{
			Vector3 temp = transform.position;
			temp.y = yBounds;
			transform.position = temp;
		}
		else if (transform.position.y > yBounds)
		{
			Vector3 temp = transform.position;
			temp.y = yBoundsDown;
			transform.position = temp;
		}
		
		//float moveDistance = speed * Time.deltaTime;
		//transform.Translate(Vector3.up * moveDistance);
	}

	void OnCollisionEnter(Collision col)
	{
		if (exploding)
		{
			ExplosionPhysicsForce spawnedExplosion = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
            Volume missile = GetComponentInChildren<Volume>();
            missile.Destruct(5, true);
            spawnedExplosion.owner = owner;
		}

        if(col.collider.tag == "Player")
        {
            if (col.gameObject.GetComponent<PlayerController>() != null)
            {
                col.gameObject.GetComponent<PlayerController>().TakeDamage(damage,owner);
            }
            else
            {
                col.gameObject.GetComponent<MotherShip>().TakeDamage(damage);
            }
        }

		Destroy(gameObject);
	}

	IEnumerator Explode()
	{
		yield return new WaitForSeconds(fuse);
		if (exploding)
		{
			Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}
}
