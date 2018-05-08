using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : PlayerController {

    public override event NoLives onNoLives;
    public GameObject[] players;

    GameObject target;
    bool turnLeft = false;
    bool turnRight = false;
    bool moveForward = false;

    // Update is called once per frame
    public override void Start()
    {
        base.Start();

        Destroy(GetComponent<PlayerController>());
    }

    override public void Update ()
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
        }

        if (ClosestPlayer() != null)
        {
            target = ClosestPlayer();
        }

        //if (ClosestPowerUP() != null)
        //{
        //    if (target != null)
        //    {
        //        if (Vector3.Distance(ClosestPowerUP().transform.position, transform.position) < Vector3.Distance(target.transform.position, transform.position))
        //        {
        //            target = ClosestPowerUP();
        //            Debug.Log("power up targeted");
        //        }
        //    }
        //    else
        //    {
        //        target = ClosestPowerUP();
        //        Debug.Log("power up targeted");
        //    }
        //}

        if (ClosestPowerUP() == null && ClosestPlayer() == null)
        {
            target = null;
        }

        //move toward target
        if (target != null)
        {
            moveForward = true;

            if (Vector3.Angle((target.transform.position - transform.position), transform.forward) >= 80 && AngleDir(transform.forward, (target.transform.position - transform.position), transform.up) == -1)
            {

                turnLeft = true;
                turnRight = false;
            }
            else if (Vector3.Angle((target.transform.position - transform.position), transform.forward) >= 80 && AngleDir(transform.forward, (target.transform.position - transform.position), transform.up) == 1)
            {
                turnLeft = false;
                turnRight = true;
            }
            else if (Vector3.Angle((target.transform.position - transform.position), transform.forward) < 80)
            {
                turnLeft = false;
                turnRight = false;
            }
        }
        else
        {
            target = null;
            moveForward = false;
            turnLeft = false;
            turnRight = false;
        }

        //shooot 
        if (target != null && Vector3.Angle((target.transform.position - transform.position), transform.forward) < 10 && target.GetComponent<PlayerController>() != null)
        {
            float random = Random.Range(0.0f, 1.0f);

            if (random <= 0.125f && bulletSlot1Ammo > 0 && fire)
            {
                FireSpecial(bulletSlot1);
            }
            else if (random <= 0.25f && bulletSlot2Ammo > 0 && fire)
            {
                FireSpecial(bulletSlot2);
            }
            else if (random <= 0.5f && fire)
            {
                Fire();
            }
        }
    }

    public override void FixedUpdate()
    {
        if (alive && gamePlaying)
        {
            if (hyperDrive)
            {
                myBody.AddRelativeForce(Vector3.forward * speed * 5);
                hyperDriveParticle.Emit(7);
            }

            if (moveForward)
            {
                myBody.AddRelativeForce(Vector3.forward * speed);
                backThruster.Emit(5);
            }

            if (turnLeft)
            {
                myBody.AddRelativeTorque(Vector3.down * turnSpeed);
                leftThrusters[1].Emit(5);
                rightThrusters[1].Emit(5);
            }
            else if (turnRight)
            {
                myBody.AddRelativeTorque(Vector3.up * turnSpeed);
                leftThrusters[0].Emit(5);
                rightThrusters[0].Emit(5);
            }
        }
    }

    public override void TakeDamage(float damage, PlayerController attacker)
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

    public override void FlashBar()
    {
       
    }

    public override void Vibrate()
    {
        
    }

    //returns -1 when to the left, 1 to the right, and 0 for forward/backward
    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }

    GameObject ClosestPlayer()
    {
        int tempWinner = -1;
        bool tie = false;
        for (int i = 0; i < players.Length; i++)
        {
            for (int j = 0; j < players.Length; j++)
            {
                if (players[i] != null && players[j] != null)
                {
                    if (Vector3.Distance(players[i].transform.position, transform.position) > Vector3.Distance(players[j].transform.position, transform.position) && players[i].activeInHierarchy && players[j].activeInHierarchy)
                    {
                        if (tempWinner != -1)
                        {
                            if (Vector3.Distance(players[i].transform.position, transform.position) > Vector3.Distance(players[tempWinner].transform.position, transform.position))
                            {
                                if (players[i].GetComponent<PlayerController>().team.text != team.text && players[i].GetComponent<PlayerController>().alive)
                                {
                                    tempWinner = i;
                                    tie = false;
                                }
                            }
                            else if (Vector3.Distance(players[i].transform.position, transform.position) == Vector3.Distance(players[tempWinner].transform.position, transform.position) && i != tempWinner)
                            {
                                tie = true;
                            }
                        }
                        else if (players[i].GetComponent<PlayerController>().team.text != team.text && players[i].GetComponent<PlayerController>().alive)
                        {
                            tempWinner = i;
                            tie = false;
                        }
                    }
                    else if (Vector3.Distance(players[i].transform.position, transform.position) == Vector3.Distance(players[j].transform.position, transform.position) && players[i].activeInHierarchy)
                    {
                        if (tempWinner == -1 && players[i].GetComponent<PlayerController>().team.text != team.text && players[i].GetComponent<PlayerController>().alive)
                        {
                            tempWinner = i;
                        }
                    }
                }
            }
        }

        if (tempWinner == -1)
        {
            return null;
        }
        else
        {
            return players[tempWinner];
        }
    }

    GameObject ClosestPowerUP()
    {
        int tempWinner = -1;
        bool tie = false;
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("PowerUP");
        if (powerUps != null)
        {
            for (int i = 0; i < powerUps.Length; i++)
            {
                for (int j = 0; j < powerUps.Length; j++)
                {
                    if (Vector3.Distance(powerUps[i].transform.position, transform.position) > Vector3.Distance(powerUps[j].transform.position, transform.position) && powerUps[i].activeInHierarchy && powerUps[j].activeInHierarchy)
                    {
                        if (tempWinner != -1)
                        {
                            if (Vector3.Distance(powerUps[i].transform.position, transform.position) > Vector3.Distance(powerUps[tempWinner].transform.position, transform.position))
                            {
                                tempWinner = i;
                                tie = false;
                            }
                            else if (Vector3.Distance(powerUps[i].transform.position, transform.position) == Vector3.Distance(powerUps[tempWinner].transform.position, transform.position) && i != tempWinner)
                            {
                                tie = true;
                            }
                        }
                        else
                        {
                            tempWinner = i;
                            tie = false;
                        }
                    }
                    else if (Vector3.Distance(powerUps[i].transform.position, transform.position) == Vector3.Distance(powerUps[j].transform.position, transform.position) && powerUps[i].activeInHierarchy)
                    {
                        if (tempWinner == -1)
                        {
                            tempWinner = i;
                        }
                    }
                }
            }

            if (tempWinner == -1)
            {
                return null;
            }
            else
            {
                switch (powerUps[tempWinner].GetComponent<PowerUP>().type)
                {
                    case 0:
                        if(bulletSlot1 == BulletType.Empty || bulletSlot2 == BulletType.Empty)
                        {
                            return powerUps[tempWinner];
                        }
                        else
                        {
                            return null;
                        }

                    case 1:
                        if (bulletSlot1 == BulletType.Empty || bulletSlot2 == BulletType.Empty)
                        {
                            return powerUps[tempWinner];
                        }
                        else
                        {
                            return null;
                        }

                    case 2:
                        return powerUps[tempWinner];

                    case 3:
                        return powerUps[tempWinner];

                    default:
                        return powerUps[tempWinner];
                }
            }
        }
        return null;
    }
}
