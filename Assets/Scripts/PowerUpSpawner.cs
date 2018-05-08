using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour {

    float spawnTime;
    public GameObject[] powerUps;

    float yBounds;
    float xBounds;
    float yBoundsDown;
    float xBoundsLeft;
    PowerUP lastPowerUp;

    // Use this for initialization
    void Start()
    {
        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -50));
        xBoundsLeft = bounds.x - 2;
        xBounds = -bounds.x + 2;
        yBoundsDown = bounds.y - 2;
        yBounds = -bounds.y + 2;

        switch (GameManager.instance.itemSpawn)
        {
            case (GameManager.SpawnTime.p10):
                spawnTime = 10;
                break;

            case (GameManager.SpawnTime.p15):
                spawnTime = 15;
                break;

            case (GameManager.SpawnTime.p20):
                spawnTime = 20;
                break;

            case (GameManager.SpawnTime.p25):
                spawnTime = 25;
                break;

            case (GameManager.SpawnTime.pNone):
                gameObject.SetActive(false);
                break;
        }

        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);

        int random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
                float x = Random.Range(xBoundsLeft, xBounds);
                GameObject powerUp = Instantiate(powerUps[Random.Range(0, powerUps.Length)], new Vector3(x, yBoundsDown), Quaternion.Euler(0, 0, Random.Range(-45f, 45f)));
                //lastPowerUp = powerUp.GetComponent<PowerUP>();
                break;

            case 1:
                x = Random.Range(xBoundsLeft, xBounds);
                powerUp = Instantiate(powerUps[Random.Range(0, powerUps.Length)], new Vector3(x, yBounds), Quaternion.Euler(0, 0, Random.Range(135f, 225f)));
                //lastPowerUp = powerUp.GetComponent<PowerUP>();
                break;

            case 2:
                float y = Random.Range(yBoundsDown, yBounds);
                powerUp = Instantiate(powerUps[Random.Range(0, powerUps.Length)], new Vector3(xBoundsLeft, y), Quaternion.Euler(0, 0, Random.Range(225f, 315f)));
                //lastPowerUp = powerUp.GetComponent<PowerUP>();
                break;

            case 3:
                y = Random.Range(yBoundsDown, yBounds);
                powerUp = Instantiate(powerUps[Random.Range(0, powerUps.Length)], new Vector3(xBounds, y), Quaternion.Euler(0, 0, Random.Range(45f, 135f)));
                //lastPowerUp = powerUp.GetComponent<PowerUP>();
                break;
        }
        StartCoroutine(Spawn());
    }
}