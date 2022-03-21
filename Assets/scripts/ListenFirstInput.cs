using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ListenFirstInput : MonoBehaviour {

    public Text levelName;
    public Text inputInstructions;

    private bool fading;

    void Start()
    {
        fading = false;
    }

	//fade out the instructions when space is hit
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
        {
            fading = true;
        }

        if (fading)
        {
            Color l = levelName.GetComponent<Text>().color;
            levelName.GetComponent<Text>().color = new Color(l.r, l.b, l.g, l.a -= 0.01f);
            inputInstructions.GetComponent<Text>().color = new Color(l.r, l.b, l.g, l.a -= 0.02f);

            if (l.a == 0)
            {
                inputInstructions.enabled = false;
                levelName.enabled = false;
                fading = false;
            }
        }

        

    }
}
