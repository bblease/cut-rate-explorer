using UnityEngine;
using System.Collections;

public class MaskRotation : MonoBehaviour {
    
    //rotate the mask
	void Update () {
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 
                                             this.transform.eulerAngles.y + Mathf.Sin(Time.time * 1.5f), 
                                             this.transform.eulerAngles.z);
	}
}
