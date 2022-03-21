using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    //handler for moving each column forward
    public GameObject fhandler;

    //UI elements
    public GameObject instruction;
    public GameObject deadUI;
    public GameObject meter;
    public GameObject counter;
    public GameObject levelLoader;

    //finished position
    public GameObject finish;

    //positional
    public Vector3 p;
    
    //character flags
    public bool alive;
    public bool calc;
    public bool movez;
    public bool movex;
    public bool falling;
    public bool heavy;
    public bool lastLevel;

    public int step = -1;
    public int max_steps;
    public int current_steps;
    
    
    void Start()
    {
        alive = true;
        movez = false;
        movex = false;
        falling = false;
        current_steps = max_steps;
        counter.gameObject.GetComponent<Text>().text = "" + max_steps;
    }

    //you died
    void die()
    {
        alive = false;
        instruction.gameObject.SetActive(true);
        instruction.gameObject.GetComponent<Text>().text = "press space to restart";
        deadUI.gameObject.SetActive(true);

        GetComponent<Animation>().Stop("ArmatureAction");
    }

    //you've made it
    void congrats()
    {
        instruction.gameObject.SetActive(true);

        if (lastLevel)
            instruction.gameObject.GetComponent<Text>().text = "thank you come again";

        else
            instruction.gameObject.GetComponent<Text>().text = "press space to continue";
    }

    void setMeter(float i)
    {
        meter.gameObject.GetComponent<Image>().fillAmount = i;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "end")
        {
            p.x = finish.gameObject.transform.position.x;
            congrats();
            levelLoader.GetComponent<LevelLoader>().finished = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 down = this.gameObject.transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        Debug.DrawRay(this.transform.position, down, Color.green, 20);

        if (movex || movez)
        {
            //check if the character is over the abyss
            //check if the character has stepped on a special
            if (Physics.Raycast(this.transform.position, down, out hit))
            {
                if (hit.transform.tag == "fall_zone")
                {
                    die();
                    falling = true;
                }
            }
        }

        else
        {
            if (Physics.Raycast(this.transform.position, down, out hit))
            {                
                if (hit.collider.transform.tag == "emerald")
                {
                    hit.collider.transform.gameObject.tag = "already_stepped";
                    //tell the handler which number the gem is
                    fhandler.gameObject.GetComponent<ForwardHandler>().skip(hit.collider.transform.gameObject.GetComponent<StepNum>().count);
                }

                if (hit.collider.transform.tag == "ruby")
                {
                    hit.collider.transform.gameObject.tag = "already_stepped";
                    fhandler.gameObject.GetComponent<ForwardHandler>().back(hit.collider.transform.gameObject.GetComponent<StepNum>().count);
                }
            }
        }
        
        //first move on the z axis, then move on x axis
        if (movez && alive)
        {

            GetComponent<Animation>()["ArmatureAction"].speed = 1;
            GetComponent<Animation>().Play("ArmatureAction");
            float diff = p.z - transform.position.z;

            //calculate the number of steps needed for the action
            //TODO steps are occasionally miscalculated
            if (calc)
            {
                if (diff <= -0.05 || diff >= 0.05)
                {
                    current_steps -= Mathf.CeilToInt(Mathf.Abs(diff) / 0.24f) + 1;
                } else
                {
                    if (heavy)
                        current_steps -= 2;
                    else
                        current_steps -= 1;
                }

                counter.gameObject.GetComponent<Text>().text = "" + current_steps;
                setMeter((float)current_steps / max_steps);

                if (current_steps < 0)
                {
                    die();
                }
                else
                {
                    calc = false;
                }
            }

            //figure out which way the character needs to face
            if (diff < 0) 
                transform.localRotation = Quaternion.Euler(0, 270, 0);

            else if (diff > 0)
                transform.localRotation = Quaternion.Euler(0, 90, 0);

            float remaining = Mathf.Abs(diff);
            transform.position = Vector3.Lerp(transform.position, 
                                              new Vector3(transform.position.x, transform.position.y, p.z), 
                                              0.05f);
            if (remaining <= 0.03)
            {
                
                movez = false;
                movex = true;
            }
        }

        else if (movex && alive)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            float remaining = Mathf.Abs(p.x - transform.position.x);
            transform.position = Vector3.Lerp(transform.position,
                                              new Vector3(p.x, transform.position.y, transform.position.z),
                                              0.05f);
            if (remaining <= 0.03)
            {
                
                GetComponent<Animation>()["ArmatureAction"].speed = 0;
                GetComponent<Animation>()["ArmatureAction"].time = 0f;
                movex = false;
            }
        }

        else if (falling)
        {
            float remaining = Mathf.Abs(this.transform.position.y - (-1.0f));
            transform.position = Vector3.Lerp(transform.position,
                                              new Vector3(transform.position.x, -1.0f, transform.position.z),
                                              0.01f);
            if (remaining <= 0.1)
                falling = false;
        }
    }
}
