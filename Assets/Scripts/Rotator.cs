using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    public float x;
    public float y;
    public float z;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //transform.Rotate(new Vector3(0.02f, 0.02f, 0.0f));
        transform.Rotate(x, y, z);
    }
}
