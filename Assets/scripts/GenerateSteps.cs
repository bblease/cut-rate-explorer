using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateSteps: MonoBehaviour {

    public static float speed = 0.4f;
    public bool chosen = false;
    public GameObject step;
    public GameObject emStep;
    public GameObject rubStep;
    public Material enorm;
    public Material rnorm;
    public Material efaded;
    public Material rfaded;
    public Material faded;
    public Material norm;
    //y position for final path
    public float accept_pos;
    //will there be special blocks?
    public bool specials;

    private bool accept = false;
    private bool stopped = false;
    private bool spawn = false;
    
 
    //number of steps, ensure the player isn't left with no where to go
    private int num = 0;
    //count the position
    private int counter = 0;

	// Use this for initialization
	void Start () {
        List<Transform> steps = new List<Transform>();
        foreach (Transform i in this.transform)
        {
            steps.Add(i.transform);
        }
        foreach (Transform j in steps)
        {
            float r = Random.Range(0.0f, 1.0f);

            if (r > 0.68)
            {
                GameObject new_step = step;
                if (specials)
                {
                    float new_r = Random.Range(0.0f, 1.0f);
                    if (new_r >= 0.75f)
                    {
                        float re = Random.Range(0.0f, 1.0f);
                        if (re >= 0.5f)
                        {
                            if (this.transform.parent.tag != "end")
                                new_step = emStep;
                        }
                        else
                        {
                            if (this.transform.parent.tag != "first" &&
                                this.transform.parent.tag != "end")
                                new_step = rubStep;
                        }
                    }
                }
                GameObject inst = Instantiate(new_step, new Vector3(j.transform.position.x, 
                                              j.transform.position.y, 
                                              j.transform.position.z), 
                                              Quaternion.identity, 
                                              j.transform) as GameObject;


                inst.transform.GetComponent<StepNum>().Set(counter);
                num++;
            }
            counter++;
        }

        if (num == 0)
        {
            
            int k = Random.Range(0, steps.Count);
            Instantiate(step, new Vector3(steps[k].transform.position.x, 
                                          steps[k].transform.position.y, 
                                          steps[k].transform.position.z), 
                                          Quaternion.identity, 
                                          steps[k].transform);

        }
	}

    //assign material mat to each step
    //assigns different materials depending if emerald or ruby
    void assignMat(Material step, Material em, Material ru)
    {
        foreach(Transform i in this.transform)
        {
            if (i.childCount != 0)
            {
                Transform child = i.transform.GetChild(0);
                if (child.gameObject.tag == "steps")
                    child.GetComponent<Renderer>().material = step;

                else if (child.gameObject.tag == "emerald")
                    child.GetComponent<Renderer>().material = em;

                else if (child.gameObject.tag == "ruby")
                    child.GetComponent<Renderer>().material = ru;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {   
        //create the new row
        //row may be sitting in collider
        //TODO remove colliders when done
        if (other.gameObject.tag == "create" && !accept)
        {
            //generate the next row
            if (!spawn) {
                this.GetComponentInParent<HandleCols>().gen();
            }
            spawn = true;
            assignMat(norm, enorm, rnorm);
            accept = true;
            this.GetComponentInParent<HandleCols>().first_accept = true;
        }

        else if (other.gameObject.tag == "faded")
        {
            if (!stopped)
            {
                assignMat(faded, efaded, rfaded);
                accept = false;
            }
        }

        //destroy the old row
        else if (other.gameObject.tag == "destroy")
        {
            Destroy(this.gameObject);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && accept && this.GetComponentInParent<HandleCols>().first_accept)
        {
            
            transform.position = new Vector3(transform.position.x, accept_pos, transform.position.z);
            chosen = true;
            stopped = true;
            this.GetComponentInParent<HandleCols>().clean();
        }

        else if (!stopped)
        {
            transform.localPosition += Vector3.down * Time.deltaTime * speed;
        }
    }
}
