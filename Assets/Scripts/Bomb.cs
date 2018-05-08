using UnityEngine;
using System.Collections;
using PicaVoxel;

public class Bomb : MonoBehaviour {

    public ExplosionPhysicsForce explosion;
    public float fuse;
    public int speed;
    public PlayerController owner;

    Rigidbody myBody;
    float yBounds;
    float xBounds;
    float yBoundsDown;
    float xBoundsLeft;

    void Start()
    {
        StartCoroutine(Explode());

        myBody = GetComponent<Rigidbody>();
        myBody.AddForce(-transform.forward * speed);

        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -50));
        xBoundsLeft = bounds.x;
        xBounds = -bounds.x;
        yBoundsDown = bounds.y;
        yBounds = -bounds.y;
    }

    void Update()
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
    }

    void OnCollisionEnter(Collision col)
    {
        ExplosionPhysicsForce spawnedExplosion = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        spawnedExplosion.owner = owner;
        GetComponentInChildren<Volume>().Destruct(5, false);
        Destroy(gameObject);
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(fuse);
        ExplosionPhysicsForce spawnedExplosion = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        spawnedExplosion.owner = owner;
        Destroy(gameObject);
    }
}
