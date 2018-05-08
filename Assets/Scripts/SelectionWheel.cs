using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWheel : MonoBehaviour {

    public string[] selections;
    public string[] discriptions;
    public GameObject[] objects;
    public Text txt;
    public Image btn;
    public Text discription;
    public int currentSelection;
    public delegate void Change();
    public event Change onChanged;
    public enum Type { Text,Num,Object};
    public Type type;

	// Use this for initialization
	void Start ()
    {
        txt = GetComponentInChildren<Text>();	
        if (type == Type.Object)
        {
            objects[currentSelection].SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Next()
    {
        switch (type)
        {
            case Type.Text:
                currentSelection++;
                if (currentSelection >= selections.Length)
                {
                    currentSelection = 0;
                }
                if (selections[currentSelection] != null)
                {
                    txt.text = selections[currentSelection];
                }
                if (discription != null)
                {
                    discription.text = discriptions[currentSelection];
                }
                break;

            case Type.Num:
                currentSelection++;
                if (currentSelection >= 100)
                {
                    currentSelection = 0;
                }
                if (currentSelection == 0)
                {
                    txt.text = "Unlimited";
                }
                else
                {
                    txt.text = currentSelection.ToString("D2");
                }
                break;

            case Type.Object:
                objects[currentSelection].SetActive(false);
                currentSelection++;
                if (currentSelection >= objects.Length)
                {
                    currentSelection = 0;
                }
                objects[currentSelection].SetActive(true);
                break;
        }
        
        if (onChanged != null)
        {
            onChanged();
        }

        AudioManager.instance.PlaySfx("btn");
    }

    public void Previous()
    {
        switch (type)
        {
            case Type.Text:
                currentSelection--;
                if (currentSelection < 0)
                {
                    currentSelection = selections.Length - 1;
                }
                if (selections[currentSelection] != null)
                {
                    txt.text = selections[currentSelection];
                }
                if (discription != null)
                {
                    discription.text = discriptions[currentSelection];
                }
                break;

            case Type.Num:
                currentSelection--;
                if (currentSelection < -0)
                {
                    currentSelection = 99;
                }
                if (currentSelection == 0)
                {
                    txt.text = "Unlimited";
                }
                else
                {
                    txt.text = currentSelection.ToString("D2");
                }
                break;

            case Type.Object:
                objects[currentSelection].SetActive(false);
                currentSelection--;
                if (currentSelection < 0)
                {
                    currentSelection = objects.Length -1;
                }
                objects[currentSelection].SetActive(true);
                break;
        }

        if (onChanged != null)
        {
            onChanged();
        }

        AudioManager.instance.PlaySfx("btn");
    }
    public void ChangeColor(Color color)
    {
        if (txt != null)
        {
            txt.color = color;
        }

        if (btn != null)
        {
            btn.color = color;
        }
    }
}