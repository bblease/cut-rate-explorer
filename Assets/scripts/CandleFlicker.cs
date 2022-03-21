using UnityEngine;
using System.Collections;

public class CandleFlicker : MonoBehaviour {

    private int i;
    //is the flicker done?
    private bool a;

    void Start ()
    {
        i = 0;
        a = false;
    }

	// Update is called once per frame
	void Update () {
        i += 1;
        if (i % 15 == 0)
        {
            float r = Random.Range(0.0f, 3.0f);
            GetComponent<Light>().intensity -= r;
            a = true;
        }
        else if (a && i % 5 == 0)
        {
            GetComponent<Light>().intensity = 8f;
            a = false;
        }
	}
}
