using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PicaVoxel;

public class Astroid : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        Color color;
        color = new Color(1, 0, 0, 0.95f);
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            random = Random.Range(0, 3);

            switch (random)
            {
                case 0:
                    color = new Color(1, 0, 0, 0.95f);
                    break;

                case 2:
                    color = new Color(0, 1, 0, 0.95f);
                    break;

                case 3:
                    color = new Color(0, 0, 1, 0.95f);
                    break;
            }

            for(int i=0; i < GetComponent<Volume>().Frames[0].Voxels.Length; i++)
            {
                random = Random.Range(0, 101);

                if (random <= 5)
                {
                    if (GetComponent<Volume>().Frames[0].Voxels[i].Value == 128)
                    {
                        GetComponent<Volume>().Frames[0].Voxels[i].Color = color;
                    }
                }
            }
            GetComponent<Volume>().Frames[0].UpdateAllChunks();
        }
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
