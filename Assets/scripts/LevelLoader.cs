using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    public string previous;
    public string next;
    public bool finished;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        else if ((Input.GetKeyDown(KeyCode.RightArrow) && next != "") || (finished && Input.GetKeyDown(KeyCode.Space) && next != ""))
            SceneManager.LoadScene(next);

        else if (finished && Input.GetKeyDown(KeyCode.Space) && next == "")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        else if (Input.GetKeyDown(KeyCode.LeftArrow) && previous != "")
            SceneManager.LoadScene(previous);

    }
}
