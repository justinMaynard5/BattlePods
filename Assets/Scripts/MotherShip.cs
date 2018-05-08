using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class MotherShip : MonoBehaviour {

    Rigidbody myBody;
    public float speed = 10.0f;
    public float turnSpeed = 10.0f;
    public float maxVelocity;
    public float bulletPush;
    public float fireDelay;
    public GameObject bulletSpawn;
    public GameObject bombSpawn;
    public GameObject bullet;
    public GameObject explodingBullet;
    public GameObject bomb;
    public GameObject shield;
    public GameObject gun;
    public Image healthBar;
    public int playerID;
    public int bulletSlot1Ammo;
    public int bulletSlot2Ammo;
    public Text txtBulletSlot1;
    public Text txtBulletSlot2;
    public enum BulletType { Empty, Missile, Bomb };

    Player player;

    float sqrMaxVelocity;
    float yBounds;
    float xBounds;
    float yBoundsDown;
    float xBoundsLeft;
    float health;
    [SerializeField]
    float maxHealth;
    bool invincible = false;
    bool hyperDrive = false;
    bool fire = true;
    BulletType bulletSlot1;
    BulletType bulletSlot2;

    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        sqrMaxVelocity = maxVelocity * maxVelocity;

        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -50));
        xBoundsLeft = bounds.x;
        xBounds = -bounds.x;
        yBoundsDown = bounds.y;
        yBounds = -bounds.y;

        bulletSlot1 = BulletType.Missile;
        bulletSlot1Ammo = 3;
        bulletSlot2 = BulletType.Bomb;
        bulletSlot2Ammo = 3;

        player = ReInput.players.GetPlayer(playerID);
        if (player.id == 0)
        {
            foreach (Joystick joy in ReInput.controllers.Joysticks)
            {
                Debug.Log(ReInput.controllers.IsControllerAssigned(ControllerType.Joystick, joy));
                if (!ReInput.controllers.IsControllerAssigned(ControllerType.Joystick, joy))
                {
                    player.controllers.AddController(joy, false);
                    player.controllers.maps.SetAllMapsEnabled(true);
                    break;
                }
            }
        }

        Debug.Log(player.id);
        Debug.Log(player.controllers.joystickCount);

        health = maxHealth;
        //txtBulletSlot1.text = bulletSlot1Ammo.ToString();
        //txtBulletSlot2.text = bulletSlot2Ammo.ToString();
    }

    // Update is called once per frame
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
            //temp.x = temp.x * -1;
            transform.position = temp;
        }
        else if (transform.position.y > yBounds)
        {
            Vector3 temp = transform.position;
            temp.y = yBoundsDown;
            //temp.x = temp.x * -1;
            transform.position = temp;
        }

        if (player.GetButtonDown("Fire 1"))
        {
            if (fire)
            {
                GameObject spawnedBullet = Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
                spawnedBullet.transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
                myBody.AddForce(-gun.transform.forward * bulletPush);
                fire = false;
                StartCoroutine(FireDelay());
            }
        }

        if (player.GetButtonDown("Fire 2"))
        {
            if (bulletSlot1 != BulletType.Empty && fire)
            {
                FireSpecial(bulletSlot1);
                bulletSlot1Ammo--;
                txtBulletSlot1.text = bulletSlot1Ammo.ToString();
                if (bulletSlot1Ammo <= 0)
                {
                    bulletSlot1 = BulletType.Empty;
                }
            }
        }

        if (player.GetButtonDown("Fire 3"))
        {
            if (bulletSlot2 != BulletType.Empty && fire)
            {
                FireSpecial(bulletSlot2);
                bulletSlot2Ammo--;
                txtBulletSlot2.text = bulletSlot2Ammo.ToString();
                if (bulletSlot2Ammo <= 0)
                {
                    bulletSlot2 = BulletType.Empty;
                }
            }
        }

        if (player.GetButton("GunLeft"))
        {
            gun.transform.Rotate(new Vector3(0, -1, 0));
        }

        if (player.GetButton("GunRight"))
        {
            gun.transform.Rotate(new Vector3(0, 1, 0));
        }
    }

    void FixedUpdate()
    {
        if (hyperDrive)
        {
            myBody.AddRelativeForce(Vector3.forward * speed * 5);
        }

        if (myBody.velocity.sqrMagnitude < sqrMaxVelocity)
        {
            myBody.AddRelativeForce(Vector3.forward * speed * player.GetAxisRaw("Forward"));
            myBody.AddRelativeForce(Vector3.back * speed * player.GetAxisRaw("Back"));
        }

        if (player.GetButton("Right"))
        {
            myBody.AddRelativeTorque(Vector3.up * turnSpeed);
        }
        else if (player.GetButton("Left"))
        {
            myBody.AddRelativeTorque(Vector3.down * turnSpeed);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerController>().TakeDamage((int)myBody.velocity.magnitude,col.gameObject.GetComponent<PlayerController>());
        }
    }

    void FireSpecial(BulletType slot)
    {
        switch (slot)
        {
            case BulletType.Empty:
                Debug.Log("Misfire");
                break;

            case BulletType.Missile:
                Instantiate(explodingBullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
                myBody.AddRelativeForce(Vector3.back * bulletPush);
                break;

            case BulletType.Bomb:
                Instantiate(bomb, bombSpawn.transform.position, bombSpawn.transform.rotation);
                myBody.AddRelativeForce(Vector3.forward * bulletPush / 2);
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!invincible)
        {
            health -= damage;
            healthBar.fillAmount = health / maxHealth;

            if (health <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void SetAmmo(BulletType type, int amount)
    {
        if (bulletSlot1 == BulletType.Empty)
        {
            bulletSlot1 = type;
            bulletSlot1Ammo = amount;
            txtBulletSlot1.text = bulletSlot1Ammo.ToString();
        }
        else if (bulletSlot2 == BulletType.Empty)
        {
            bulletSlot2 = type;
            bulletSlot2Ammo = amount;
            txtBulletSlot2.text = bulletSlot2Ammo.ToString();
        }
    }

    public void StartInvincible(float time, bool shieldOn)
    {
        if (shieldOn)
        {
            shield.SetActive(true);
        }

        invincible = true;
        StartCoroutine(InvinsibleTime(time));
    }

    public void StartHyperDrive(float time)
    {
        hyperDrive = true;
        StartCoroutine(HyperDriveTime(time));
    }

    IEnumerator InvinsibleTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (shield.activeInHierarchy)
        {
            shield.SetActive(false);
        }

        invincible = false;
    }

    IEnumerator HyperDriveTime(float time)
    {
        yield return new WaitForSeconds(time);

        hyperDrive = false;
    }

    IEnumerator FireDelay()
    {
        yield return new WaitForSeconds(fireDelay);
        fire = true;
    }
}