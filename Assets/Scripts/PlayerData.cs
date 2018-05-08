using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {

    public int playerID = -1;
    public enum Colors {Red,Green,Blue,Yellow};
    public Colors color;
    public string team = null;
    public int ship = -1;
    public enum Type { Player, AI };
    public Type type;

	// Use this for initialization
	void Start ()
    {
        	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reset()
    {
        playerID = -1;
        team = null;
        ship = -1;
    }
}
