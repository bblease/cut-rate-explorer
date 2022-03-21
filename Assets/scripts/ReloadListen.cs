using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ReloadListen : MonoBehaviour {
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
