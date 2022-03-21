using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForwardHandler : MonoBehaviour {

    public GameObject dudRow;
    //steps instantiated by emeralds
    public GameObject step;
    public GameObject player;
    
    private List<Transform> forward = new List<Transform>();
    //which column the character is on
    private int count = 0;


    //emerald is stepped on
    //i determines which step to spawn on after jumping
    public void skip(int i)
    {
        Vector3 moveTo = new Vector3();
        for (int j = 1; j <= 2; j++)
        {
            if (count + 1 < forward.Count)
            {
                this.gameObject.transform.GetChild(count).GetComponent<HandleCols>().clear();

                Transform target = this.transform.GetChild(count).GetChild(2);
                //instantiate the new row
                //temporary
                GameObject nextRow = Instantiate(dudRow,
                                                 new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z),
                                                 Quaternion.identity,
                                                 target.parent.transform) as GameObject;

                Transform nextStepTarget = nextRow.transform.GetChild(i);
                GameObject nextStep = Instantiate(step,
                                                  new Vector3(nextStepTarget.transform.position.x, nextStepTarget.transform.position.y, nextStepTarget.transform.position.z),
                                                  Quaternion.identity,
                                                  nextStepTarget.transform) as GameObject;

                nextStep.GetComponent<StepNum>().count = i;
                moveTo = nextStep.transform.position;
                count++;
            }
        }

        //start the animation from here
        player.GetComponent<Animation>()["ArmatureAction"].speed = 1;
        player.GetComponent<Animation>().Play("ArmatureAction");
       
        //move
        player.GetComponent<Player>().movex = true;
        player.GetComponent<Player>().p = new Vector3(moveTo.x, player.transform.position.y, player.transform.position.z);
        player.GetComponent<Player>().step = i;

        //finally, start the row back up
        go(null);   
    }

    //ruby is stepped on, trace back one row
    //i determines which step to spawn on after destroying the steps
    public void back(int i)
    {
        if (count > 1 && !player.GetComponent<Player>().movex)
        {
            if (count < forward.Count)
            {
                this.gameObject.transform.GetChild(count).GetComponent<HandleCols>().clear();
            }

            this.gameObject.transform.GetChild(count-1).GetComponent<HandleCols>().clear();
            count--;
            player.transform.position = new Vector3(this.transform.GetChild(count - 1).transform.GetChild(5).transform.GetChild(i).transform.position.x,
                                                    player.transform.position.y,
                                                    this.transform.GetChild(count - 1).transform.GetChild(5).transform.GetChild(i).transform.position.z);
            go(forward[count - 1]);
        }
    }

    //run the current column
    void go(Transform prev)
    {
        if (forward.Count > 0)
        {
            forward[count].transform.gameObject.GetComponent<HandleCols>().go(prev);
        }
    }

    // Use this for initialization
    void Start()
    {
        foreach (Transform i in this.transform)
        {
            forward.Add(i.transform);
        }
        go(null);
    }

    //move forwards
    void Update()
    {
        if (count < forward.Count && forward[count].GetComponent<HandleCols>().first_accept && Input.GetKeyDown(KeyCode.Space) && forward.Count > 0)
        {
            if (count + 1 < forward.Count)
                count++;

            //send the previous row to the current row
            go(forward[count - 1]);
        }
    }
}
