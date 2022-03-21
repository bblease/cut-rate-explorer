using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandleCols : MonoBehaviour {

    public GameObject fall;
    public GameObject row;
    public GameObject player;
    public float speed = 1.0f;
    public bool first_accept = false;

    private List<Transform> cols = new List<Transform>();
    private List<GameObject> rows = new List<GameObject>();
    private Transform prev = null;
    

    //clean up after ourselves and make sure the rows are empty
    public void deleteFirst()
    {
        if (rows.Count > 0)
        {
            rows.RemoveAt(0);
        }
    }

    //clear any rows spawned (used for backtracking)
    public void clear()
    {
        foreach (Transform t in this.transform)
        {
            if (t.transform.gameObject.tag == "row")
                Destroy(t.gameObject);
        }
    }

    public void gen()
    {
        Transform j = cols[0];
        GameObject n = (GameObject) Instantiate(row, new Vector3(j.transform.position.x, 
                                     j.transform.position.y, 
                                     j.transform.position.z), 
                                     Quaternion.identity, 
                                     this.transform);
        rows.Add(n);

        if (rows.Count > 4)
        {
            deleteFirst();
        }
    }

    public void go(Transform pre) {
        prev = pre;
        foreach (Transform i in this.transform)
        {
            cols.Add(i.transform);
        }
        gen();
    }

    //print the list l
    //helpful for debugging
    void printList(List<Transform> l)
    {
        string o = "";
        foreach(Transform t in l)
        {
            if (t != null)
            {
                o += Mathf.Abs(t.transform.position.z - player.transform.position.z) + " ";
            }
          
        }
        Debug.Log(o);
    }

    List<Transform> sortChildren(GameObject r)
    {
        List<Transform> o = new List<Transform>();
        List<Transform> start = new List<Transform>();

        //generate start
        foreach (Transform t in r.transform)
        {
            start.Add(t);
        }

        //simple insertion sort
        while(o.Count <= 6)
        {
            int j = 0;

            //transform to beat
            Transform toAdd = null;
            //minimum distance to beat
            float d = new float();

            foreach (Transform t in start)
            {
                //the step could be a step, emerald, or ruby
                if (t.gameObject.transform.childCount != 0 &&
                   (t.gameObject.transform.GetChild(0).tag == "steps" ||
                    t.gameObject.transform.GetChild(0).tag == "ruby" ||
                    t.gameObject.transform.GetChild(0).tag == "emerald"))
                {
                    //start off the search for min value
                    if (j == 0)
                    {
                        d = Mathf.Abs(t.transform.position.z - player.transform.position.z);
                        toAdd = t;
                        j++;
                    }
                    else if (Mathf.Abs(t.transform.position.z - player.transform.position.z) < d)
                    {
                        d = Mathf.Abs(t.transform.position.z - player.transform.position.z);
                        toAdd = t;
                        j++;
                    }
                }
            }

            o.Add(toAdd);
            if (start.Contains(toAdd))
                start.Remove(toAdd);
            
        }

        printList(o);
        return o;

    }

    int checkPath(int pnum, int num)
    {
        int o = 1;
        if (prev != null && prev.GetChild(5) != null)
        {
            Debug.Log(prev.gameObject.name);
            
            if (pnum == num)
            {
                o = 1;
            }

            else if (pnum > num)
            {
                for (int i = pnum; i >= num; i--)
                {
                    if (prev.GetChild(5).transform.GetChild(i).childCount == 0)
                    {
                        o = -1;
                    }
                }
            }
            else if (pnum < num)
            {
                for (int i = pnum; i <= num; i++)
                {
                    if (prev.GetChild(5).transform.GetChild(i).childCount == 0)
                    {
                        o = -1;
                    }
                }
            }
        }
        return o;
    }

    //move the player to the destination
    //first check which step is closest to the player
    public void movePlayer(GameObject r)
    {
        if (r != null)
        {

            List<Transform> sorted = sortChildren(r);

            float x = new float();
            float z = new float();
            //the number of the closest step
            int num = -1;

            if (sorted[0] != null)
            {
                num = sorted[0].gameObject.transform.GetChild(0).gameObject.GetComponent<StepNum>().count;
            }

            int pnum = player.gameObject.GetComponent<Player>().step;

            //if there's no path, aim for the closest and die
            Transform original = sorted[0];

            //the player must be on the first row
            if (prev != null)
            { 
                while (checkPath(pnum, num) == -1 && sorted.Count > 0)
                {
                    sorted.RemoveAt(0);
                    if (sorted.Count > 0 && sorted[0] != null)
                    {
                        num = sorted[0].gameObject.transform.GetChild(0).gameObject.GetComponent<StepNum>().count;
                    }
                }
            }

            if (sorted.Count > 0 && sorted[0] != null)
            {
                x = sorted[0].transform.position.x;
                z = sorted[0].transform.position.z;
            } else if (original != null)
            {
                x = original.transform.position.x;
                z = original.transform.position.z;
            }

            player.GetComponent<Player>().p = new Vector3(x,
                                                          player.gameObject.transform.position.y,
                                                          z);
            player.GetComponent<Player>().movez = true;
            player.GetComponent<Player>().calc = true;
            player.GetComponent<Player>().step = num;
        }
    }

    public void clean()
    {
        foreach(GameObject i in rows)
        {
            if (i != null && !i.GetComponent<GenerateSteps>().chosen)    
                Destroy(i);


            else if (i != null && i.GetComponent<GenerateSteps>().chosen)
            {
                movePlayer(i);
            }
                
        }
  
    }

    public void endCol()
    {
        if (rows.Count != 0)
        {
            rows[0].gameObject.GetComponent<GenerateSteps>().enabled = false;
        }
    }
}
