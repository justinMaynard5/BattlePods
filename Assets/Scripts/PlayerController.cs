using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UI;
using PicaVoxel;

public class PlayerController : MonoBehaviour {

    public Rigidbody myBody;
    public float speed = 10.0f;
    public float turnSpeed = 10.0f;
    public float maxVelocity;
    public float bulletPush;
    public float fireDelay;
    public GameObject bulletSpawn;
    public GameObject bombSpawn;
    public Projectile bullet;
    public Projectile explodingBullet;
    public Bomb bomb;
    public GameObject shield;
    public Image UIBorder;
    public Image healthBar;
    public Image bullet1;
    public Image bullet2;
    public Sprite missleSprite;
    public Sprite bombSprite;
    public int playerID;
    public int bulletSlot1Ammo;
    public int bulletSlot2Ammo;
    public int points;
    public Text txtBulletSlot1;
    public Text txtBulletSlot2;
    public Text team;
    public Text pointsLabel;
    public Text txtPoints;
    public enum BulletType { Empty, Missile, Bomb };
    public Volume[] ships;
    public Volume ship;
    [HideInInspector]
    public bool gamePlaying = true;
    public delegate void ScoreIncrease(int points, string team);
    public delegate void NoLives(string team);
    public event ScoreIncrease onScoreIncrease;
    public virtual event NoLives onNoLives;
    public ParticleSystem backThruster;
    public ParticleSystem hyperDriveParticle;
    public ParticleSystem[] leftThrusters;
    public ParticleSystem[] rightThrusters;
    public ParticleSystem[] frontThrusters;

    Player player;
    public PlayerData playerData = null;

    float sqrMaxVelocity;
    public float yBounds;
    public float xBounds;
    public float yBoundsDown;
    public float xBoundsLeft;
    public float health;
    public float maxHealth;
    public int lives;
    public bool invincible = false;
    public bool hyperDrive = false;
    public bool fire = true;
    public bool pointsMatch = false;
    public bool alive = true;
    public BulletType bulletSlot1;
    public BulletType bulletSlot2;
    Color color;
    public CapsuleCollider myCollider;
    Vector3 startPos;
    Quaternion startRot;
    GameplayManager gameplayManager;
    public AudioSource thruster;

    public virtual void Start ()
    {
        gameplayManager = FindObjectOfType<GameplayManager>();
        thruster = GetComponent<AudioSource>();

        foreach(PlayerData data in GameManager.instance.playerData)
        {
            if (data.playerID == playerID)
            {
                playerData = data;
            }
        }

        if (playerData == null)
        {
            gameObject.SetActive(false);
            UIBorder.gameObject.SetActive(false);
        }

        myBody = GetComponent<Rigidbody>();
        myCollider = GetComponent<CapsuleCollider>();
        startPos = transform.position;
        startRot = transform.rotation;
        sqrMaxVelocity = maxVelocity * maxVelocity;

        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -50));
        xBoundsLeft = bounds.x;
        xBounds = -bounds.x;
        yBoundsDown = bounds.y;
        yBounds = -bounds.y;

        switch (GameManager.instance.slot1)
        {
            case GameManager.BulletType.Missile:
                bulletSlot1 = BulletType.Missile;
                bullet1.sprite = missleSprite;
                bullet1.color = new Color(1, 0, 0, 0.56f);
                bulletSlot1Ammo = 3;
                break;

            case GameManager.BulletType.Bomb:
                bulletSlot1 = BulletType.Bomb;
                bullet1.sprite = bombSprite;
                bullet1.color = new Color(1, 0.47f, 0, 0.56f);
                bulletSlot1Ammo = 3;
                break;

            case GameManager.BulletType.Empty:
                bulletSlot1 = BulletType.Empty;
                bullet1.gameObject.SetActive(false);
                bulletSlot1Ammo = 0;
                break;
        }

        switch (GameManager.instance.slot2)
        {
            case GameManager.BulletType.Missile:
                bulletSlot2 = BulletType.Missile;
                bullet2.sprite = missleSprite;
                bullet2.color = new Color(1, 0, 0, 0.56f);
                bulletSlot2Ammo = 3;
                break;

            case GameManager.BulletType.Bomb:
                bulletSlot2 = BulletType.Bomb;
                bullet2.sprite = bombSprite;
                bullet2.color = new Color(1, 0.47f, 0, 0.56f);
                bulletSlot2Ammo = 3;
                break;

            case GameManager.BulletType.Empty:
                bulletSlot2 = BulletType.Empty;
                bullet2.gameObject.SetActive(false);
                bulletSlot2Ammo = 0;
                break;
        }

        switch (playerData.color)
        {
            case PlayerData.Colors.Red:
                color = new Color(1, 0, 0, 0.98f);
                break;

            case PlayerData.Colors.Green:
                color = new Color(0, 1, 0, 0.98f);
                break;

            case PlayerData.Colors.Blue:
                color = new Color(0, 0, 1, 0.98f);
                break;

            case PlayerData.Colors.Yellow:
                color = new Color(1, 1, 0, 0.98f);
                break;
        }

        player = ReInput.players.GetPlayer(playerID);
        if (player.id == 0)
        {
            foreach(Joystick joy in ReInput.controllers.Joysticks)
            {
                if (!ReInput.controllers.IsControllerAssigned(ControllerType.Joystick,joy))
                {
                    player.controllers.AddController(joy, false);
                    player.controllers.maps.SetAllMapsEnabled(true);
                    break;
                }
            }
        }

        health = maxHealth;
        txtBulletSlot1.text = bulletSlot1Ammo.ToString();
        txtBulletSlot2.text = bulletSlot2Ammo.ToString();
        team.text = playerData.team;

        switch (GameManager.instance.scoreMethod)
        {
            case (GameManager.ScoreMethod.Points):
                pointsLabel.text = "Points:";
                txtPoints.text = "0";
                points = 0;
                pointsMatch = true;
                break;

            case (GameManager.ScoreMethod.Lives):
                pointsLabel.text = "Lives:";
                lives = GameManager.instance.maxScore;
                if (lives == 0)
                {
                    txtPoints.text = "∞";
                }
                else
                {
                    txtPoints.text = lives.ToString();
                }
                break;
        }

        UIBorder.color = new Color(color.r, color.g, color.b, 0.56f);
        team.color = new Color(color.r, color.g, color.b, 0.56f);
        healthBar.color = new Color(color.r, color.g, color.b, 0.56f);
        pointsLabel.color = new Color(color.r, color.g, color.b, 0.56f);
        txtPoints.color = new Color(color.r, color.g, color.b, 0.56f);
        txtBulletSlot1.color = new Color(color.r, color.g, color.b, 0.56f);
        txtBulletSlot2.color = new Color(color.r, color.g, color.b, 0.56f);
        shield.GetComponent<Renderer>().material.SetColor("_Color", color);

        ships[playerData.ship].gameObject.SetActive(true);
        ship = ships[playerData.ship];

        for (int i = 0; i < ship.Frames[0].Voxels.Length;i++)
        {
            if (ship.Frames[0].Voxels[i].Value == 1)
            {
                ship.Frames[0].Voxels[i].Color = color;
            }
        }
        ship.Frames[0].UpdateAllChunks();
    }
	
	// Update is called once per frame
	virtual public void Update ()
    {
        if (alive && gamePlaying)
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
                    Fire();
                }
            }

            if (player.GetButtonDown("Fire 2"))
            {
                if (bulletSlot1 != BulletType.Empty && fire)
                {
                    FireSpecial(bulletSlot1);
                }
            }

            if (player.GetButtonDown("Fire 3"))
            {
                if (bulletSlot2 != BulletType.Empty && fire)
                {
                    FireSpecial(bulletSlot2);
                }
            }

            if (player.GetButtonDown("Pause"))
            {
                gameplayManager.Pause();
            }
        }
    }

    virtual public void FixedUpdate()
    {
        if (alive && gamePlaying)
        {
            if (hyperDrive)
            {
                myBody.AddRelativeForce(Vector3.forward * speed * 5);
                hyperDriveParticle.Emit(7);
            }

            if (myBody.velocity.sqrMagnitude < sqrMaxVelocity)
            {
                myBody.AddRelativeForce(Vector3.forward * speed * player.GetAxisRaw("Forward"));
                backThruster.Emit(5 * Mathf.RoundToInt(player.GetAxisRaw("Forward")));
                myBody.AddRelativeForce(Vector3.back * speed * player.GetAxisRaw("Back"));
                frontThrusters[0].Emit(5 * Mathf.RoundToInt(player.GetAxisRaw("Back")));
                frontThrusters[1].Emit(5 * Mathf.RoundToInt(player.GetAxisRaw("Back")));
                if (player.GetAxisRaw("Forward") > 0 || player.GetAxisRaw("Back" ) > 0 || player.GetButton("Right") || player.GetButton("Left"))
                {
                    if (!thruster.isPlaying)
                    {
                        thruster.Play();
                    }
                }
                else
                {
                    thruster.Stop();
                }
            }

            if (player.GetButton("Right"))
            {
                myBody.AddRelativeTorque(Vector3.up * turnSpeed);
                leftThrusters[0].Emit(5);
                rightThrusters[0].Emit(5);
            }
            else if (player.GetButton("Left"))
            {
                myBody.AddRelativeTorque(Vector3.down * turnSpeed);
                leftThrusters[1].Emit(5);
                rightThrusters[1].Emit(5);
            }
        }
    }

    virtual public void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            if (col.gameObject.GetComponent<PlayerController>() != null)
            {
                col.gameObject.GetComponent<PlayerController>().TakeDamage((int)myBody.velocity.magnitude,this);
            }
            else
            {
                col.gameObject.GetComponent<MotherShip>().TakeDamage((int)myBody.velocity.magnitude);
            }
        }
    }

    virtual public void Fire()
    {
        Projectile spawnedBullet = Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation) as Projectile;
        spawnedBullet.owner = this;
        spawnedBullet.GetComponent<Renderer>().material.color = color;
        spawnedBullet.GetComponent<Renderer>().material.SetColor("_EmissionColor",color);
        AudioManager.instance.PlaySfx("Shoot");
        myBody.AddRelativeForce(Vector3.back * bulletPush);
        fire = false;
        StartCoroutine(FireDelay());
    }

    virtual public void FireSpecial(BulletType slot)
    {
        switch (slot)
        {
            case BulletType.Empty:
                Debug.Log("Misfire");
                break;

            case BulletType.Missile:
                Projectile missle = Instantiate(explodingBullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation) as Projectile;
                missle.owner = this;
                AudioManager.instance.PlaySfx("Shoot");
                myBody.AddRelativeForce(Vector3.back * bulletPush);
                break;

            case BulletType.Bomb:
                Bomb spawnedBomb = Instantiate(bomb, bombSpawn.transform.position, bombSpawn.transform.rotation) as Bomb;
                spawnedBomb.owner = this;
                AudioManager.instance.PlaySfx("Shoot");
                myBody.AddRelativeForce(Vector3.forward * bulletPush/2);
                break;
        }

        if (slot == bulletSlot1)
        {
            bulletSlot1Ammo--;
            txtBulletSlot1.text = bulletSlot1Ammo.ToString();
            if (bulletSlot1Ammo <= 0)
            {
                bulletSlot1 = BulletType.Empty;
            }
        }
        else if (slot == bulletSlot2)
        {
            bulletSlot2Ammo--;
            txtBulletSlot2.text = bulletSlot2Ammo.ToString();
            if (bulletSlot2Ammo <= 0)
            {
                bulletSlot2 = BulletType.Empty;
            }
        }

        fire = false;
        StartCoroutine(FireDelay());
    }

    virtual public void TakeDamage(float damage,PlayerController attacker)
    {
        if (!invincible && alive && gamePlaying)
        {
            health -= damage;
            AudioManager.instance.PlaySfx("Hit");
            healthBar.fillAmount = health / maxHealth;
            FlashBar();

            if (health <= 0)
            {
                Vibrate();
                alive = false;
                ship.Destruct(8, false);
                AudioManager.instance.PlaySfx("Explosion");
                thruster.Stop();
                ship.gameObject.SetActive(false);
                if (attacker != null)
                {
                    attacker.GainPoints();
                }
                lives--;
                onNoLives(playerData.team);
                if (!pointsMatch && GameManager.instance.maxScore != 0)
                {
                    txtPoints.text = lives.ToString("D2");
                }
                if (lives > 0 || GameManager.instance.scoreMethod != GameManager.ScoreMethod.Lives || GameManager.instance.maxScore == 0)
                {
                    StartCoroutine(Respawn());
                }
                myCollider.enabled = false;
            }
            else
            {
                ship.Destruct(5, false);
            }
        }
    }

    virtual public void SetAmmo(BulletType type,int amount)
    {
        if (bulletSlot1 == BulletType.Empty)
        {
            bulletSlot1 = type;
            bulletSlot1Ammo = amount;
            txtBulletSlot1.text = bulletSlot1Ammo.ToString();
            switch (type)
            {
                case BulletType.Missile:
                    bullet1.sprite = missleSprite;
                    bullet1.color = new Color(1, 0, 0, 0.56f);
                    break;

                case BulletType.Bomb:
                    bullet1.sprite = bombSprite;
                    bullet1.color = new Color(1, 0.47f, 0, 0.56f);
                    break;
            }
        }
        else if (bulletSlot2 == BulletType.Empty)
        {
            bulletSlot2 = type;
            bulletSlot2Ammo = amount;
            txtBulletSlot2.text = bulletSlot2Ammo.ToString();
            switch (type)
            {
                case BulletType.Missile:
                    bullet2.sprite = missleSprite;
                    bullet2.color = new Color(1, 0, 0, 0.56f);
                    break;

                case BulletType.Bomb:
                    bullet2.sprite = bombSprite;
                    bullet2.color = new Color(1, 0.47f, 0, 0.56f);
                    break;
            }
        }
    }

    virtual public void StartInvincible(float time,bool shieldOn)
    {
        if (shieldOn)
        {
            shield.SetActive(true);
        }

        invincible = true;
        StartCoroutine(InvinsibleTime(time));
    }

    virtual public void StartHyperDrive(float time)
    {
        hyperDrive = true;
        StartInvincible(time,false);
        StartCoroutine(HyperDriveTime(time));
    }

    virtual public void GainPoints()
    {
        points += 1;
        onScoreIncrease(1, team.text);
        if (pointsMatch)
        {
            txtPoints.text = points.ToString();
        }
    }

    virtual public void FlashBar()
    {
        foreach (Joystick joystick in player.controllers.Joysticks)
        {
            var ds4 = joystick.GetExtension<Rewired.ControllerExtensions.DualShock4Extension>();
            if (ds4 == null) continue; // this is not a DS4, skip it

            ds4.SetLightFlash(0.25f, 0.25f);
            StartCoroutine(StopFlash());
        }
    }

    virtual public void Vibrate()
    {
        foreach (Joystick j in player.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            if (j.vibrationMotorCount > 0) j.SetVibration(1, 1f, 0.5f);
            if (j.vibrationMotorCount > 0) j.SetVibration(0, 1f, 0.5f);
        }
    }

    virtual public IEnumerator InvinsibleTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (shield.activeInHierarchy)
        {
            shield.SetActive(false);
        }

        invincible = false;
    }

    virtual public IEnumerator HyperDriveTime(float time)
    {
        yield return new WaitForSeconds(time);

        hyperDrive = false;
    }

    virtual public IEnumerator FireDelay()
    {
        yield return new WaitForSeconds(fireDelay);
        fire = true;
    }
    virtual public IEnumerator StopFlash()
    {
        yield return new WaitForSeconds(1);
        foreach (Joystick joystick in player.controllers.Joysticks)
        {
            var ds4 = joystick.GetExtension<Rewired.ControllerExtensions.DualShock4Extension>();
            if (ds4 == null) continue; // this is not a DS4, skip it

            ds4.StopLightFlash();
            ds4.SetLightColor(color);
        }
    }

    virtual public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(10);

        transform.position = startPos;
        transform.rotation = startRot;
        myCollider.enabled = true;
        ship.gameObject.SetActive(true);
        myBody.velocity = new Vector3(0, 0, 0);
        myBody.angularVelocity = new Vector3(0, 0, 0);
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;

        alive = true;
    }
}