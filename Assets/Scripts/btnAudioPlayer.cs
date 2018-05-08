using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btnAudioPlayer : MonoBehaviour {

	public void btnPlaySFX(string name)
    {
        AudioManager.instance.PlaySfx(name);
    }
}
