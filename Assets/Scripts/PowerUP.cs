using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PicaVoxel;

public class PowerUP : MonoBehaviour {

    public int speed;
    public Volume[] crates;

    public int type;
    //0=Missle,1=Bomb,2=Shield,3=Hyper Drive
    Rigidbody myBody;
    float yBounds;
    float xBounds;
    float yBoundsDown;
    float xBoundsLeft;
    Volume current;
    // Use this for initialization
    void Start ()
    {
        //Set(3);
        myBody = GetComponent<Rigidbody>();
        myBody.AddForce(transform.up * speed);

        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -50));
        xBoundsLeft = bounds.x - 10;
        xBounds = -bounds.x + 10;
        yBoundsDown = bounds.y - 10;
        yBounds = -bounds.y + 10;

        Set(Random.Range(0, 4));
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

    public void Set(int targetType)
    {
        type = targetType;

        switch (type)
        {
            case 0:
                crates[type].gameObject.SetActive(true);
                current = crates[type];
                break;

            case 1:
                crates[type].gameObject.SetActive(true);
                current = crates[type];
                break;

            case 2:
                crates[type].gameObject.SetActive(true);
                current = crates[type];
                break;

            case 3:
                crates[type].gameObject.SetActive(true);
                current = crates[type];
                break;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            PlayerController player = col.GetComponent<PlayerController>();
            
            switch (type)
            {
                case 0:
                    if (player.bulletSlot1Ammo <= 0 || player.bulletSlot2Ammo <= 0)
                    {
                        player.SetAmmo(PlayerController.BulletType.Missile, 3);
                        current.Destruct(3, true);
                        Destroy(gameObject);
                    }
                    break;


                case 1:
                    if (player.bulletSlot1Ammo <= 0 || player.bulletSlot2Ammo <= 0)
                    {
                        player.SetAmmo(PlayerController.BulletType.Bomb, 3);
                        current.Destruct(3, true);
                        Destroy(gameObject);
                    }
                    break;

                case 2:
                    player.StartInvincible(30, true);
                    current.Destruct(3, true);
                    Destroy(gameObject);
                    break;

                case 3:
                    //player.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 9000);
                    player.StartHyperDrive(20);
                    player.StartInvincible(30, false);
                    current.Destruct(3, true);
                    Destroy(gameObject);
                    break;
            }
        }
    }
}