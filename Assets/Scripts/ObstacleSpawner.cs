using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

    float spawnTime;
    public GameObject[] obstacles;

    float yBounds;
    float xBounds;
    float yBoundsDown;
    float xBoundsLeft;

    // Use this for initialization
    void Start ()
    {
        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -50));
        xBoundsLeft = bounds.x - 2;
        xBounds = -bounds.x + 2;
        yBoundsDown = bounds.y - 2;
        yBounds = -bounds.y + 2;

        switch (GameManager.instance.hazardSpawn)
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
	void Update () {
		
	}

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);

        int random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
                float x = Random.Range(xBoundsLeft, xBounds);
                Instantiate(obstacles[Random.Range(0,obstacles.Length)], new Vector3(x, yBoundsDown),Quaternion.Euler(0,0,Random.Range(-45f,45f)));
                break;

            case 1:
                x = Random.Range(xBoundsLeft, xBounds);
                Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(x, yBounds), Quaternion.Euler(0,0, Random.Range(135f, 225f)));
                break;

            case 2:
                float y = Random.Range(yBoundsDown, yBounds);
                Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(xBoundsLeft, y), Quaternion.Euler(0, 0, Random.Range(225f, 315f)));
                break;

            case 3:
                y = Random.Range(yBoundsDown, yBounds);
                Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(xBounds, y), Quaternion.Euler(0, 0, Random.Range(45f, 135f)));
                break;
        }

        StartCoroutine(Spawn());
    }
}
