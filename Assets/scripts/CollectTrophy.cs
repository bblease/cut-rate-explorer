using UnityEngine;
using System.Collections;

public class CollectTrophy : MonoBehaviour {

    public GameObject trophy;
    public GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(trophy.gameObject);
            player.GetComponent<Player>().heavy = true;
        }
    }
}
