using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;

public class Hand : MonoBehaviour
{
    List<InputDevice> devices;
    InputDevice targetDevice;

    //PUBLIC
    public GameObject OtherHand;
    public GameObject Head;
    public Head HeadScript;
    public int handNum;
    public bool grabOverridden = false;
    public bool flight;
    public Rigidbody PlayerRB;
    public Transform PlayerT;
    public Material defaultMat;
    public Material grabbedMat;
    public GameObject Gun;
    public TMP_Text LogText;
    public GameObject Projectile;

    Vector3 lastPosition;
    Vector3 lastPlayerPosition;
    //float gripAxis = 0;
    float triggerAxis = 0;
    float gripAxis = 0;

    float moveSpeed = 20f;
    int velHistLength = 15;
    float swingMaxSpeed = 0.4f;
    float thrusterSpeed = 1.5f;
    float vibDurationRemaining = 0f;
    const float vibOneFrame = 0.01f;

    int touchingBranches = 0;
    bool grabbed = false;
    bool interior = false;
    bool jumpSaving = false;
    bool lastTriggerDown = false;
    bool alreadyCol = false;
    bool gunMode = false;

    Vector3[] velHist;

    Vector3 grabPos;
    Vector3 grabMove;
    

    // Start is called before the first frame update
    void Start()
    {
        HeadScript = Head.GetComponent<Head>();
        velHist = new Vector3[velHistLength];
        devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        PlayerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Vibrate(float strength, float duration)
    {
        OVRInput.SetControllerVibration(1f, strength, (handNum == 1 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
        vibDurationRemaining = duration;

    }

    // Update is called once per frame
    void Update()
    {
        alreadyCol = false;

        
        
        if (devices.Count <= 0)
        {
            LogText.text = "NODEVS"; 
            return;
        }
        targetDevice = devices[handNum];

        //targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripAxis);
        targetDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerAxis);
        targetDevice.TryGetFeatureValue(CommonUsages.grip, out gripAxis);
        targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 myPos);
        targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion myRot);
        targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool restartButton);
        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton);
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton);

        if (restartButton)
        {
            HeadScript.Reset();
        }

        if (handNum == 1) {
            HeadScript.levitation = secondaryButton;
        }

        if (handNum == 2)
        {
            if (secondaryButton && !jumpSaving)
            {
                HeadScript.JumpSave();
            }
            jumpSaving = secondaryButton;
        }

        if (primaryButton)
        {
            if (!HeadScript.thrustRanOut)
            {
                Vector3 impulseDirection = transform.rotation * Vector3.forward;

                HeadScript.Impulse(impulseDirection * thrusterSpeed * Time.deltaTime);

                Vibrate(0.1f, vibOneFrame);
            }

            HeadScript.usingThrust = true;
        }

        if (flight)
        {
            interior = true;
        }

        transform.localPosition = myPos;
        transform.localRotation = myRot;

        

        if (grabbed)
        {
            grabMove = grabPos - transform.position;
            PlayerT.position += grabMove;
        }

        
        interior = CheckInterior();

        gunMode = GripDown();
        Gun.SetActive(gunMode);
        GetComponent<Renderer>().enabled = !gunMode;

        if (TriggerDown() && !lastTriggerDown && gunMode)
        {
            Instantiate(Projectile, transform.position, transform.rotation);
            FindObjectOfType<AudioManager>().Play("Shoot");
            Vibrate(0.25f, 0.1f);
        }

        //GRABBING
        if (TriggerDown() && !gunMode && (touchingBranches > 0 || interior) && !grabOverridden)
        {
            if (!grabbed) {

                LogText.text = ""+PlayerRB.velocity.magnitude;


                Vibrate(PlayerRB.velocity.magnitude/20, 0.05f);

                GetComponent<Renderer>().material = grabbedMat;

                //GetComponent<AudioSource>().Play();
                
                if (grabPos == new Vector3(0, 0, 0))
                {
                    grabPos = transform.position;
                }
                
                OtherHand.GetComponent<Hand>().grabOverridden = true;
            }


            grabbed = true;

            PlayerRB.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            grabPos = new Vector3(0, 0, 0);
            
            PlayerRB.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);

            if (grabbed)
            {
                GetComponent<Renderer>().material = defaultMat;

                PlayerRB.velocity = SwingVel()*moveSpeed;
            }

            grabbed = false;

             
        }

        if (TriggerDown())
        {
            HeadScript.gravGrace = false;
        }

        //if (!(TriggerDown() && (touchingBranches > 0 || interior)))
        if(!TriggerDown())
        {
            grabOverridden = false;
        }

        /*if (vibRemaining > 0f)
        {
            vibRemaining -= Time.deltaTime;
            if (vibRemaining <= 0f)
            {
                OVRInput.SetControllerVibration(0, 0, handNum == 0 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
            }
        }*/

        if (vibDurationRemaining <= 0f)
        {
            OVRInput.SetControllerVibration(0, 0, (handNum == 1 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
            vibDurationRemaining = 0f;
        }

        if (vibDurationRemaining > 0f)
        {
            vibDurationRemaining -= Time.deltaTime;
        }

        lastPosition = transform.position;
        lastPlayerPosition = PlayerT.position;
        lastTriggerDown = TriggerDown();
    }

    /*void StartVib(float duration)
    {
        vibRemaining = duration;
        OVRInput.SetControllerVibration(1, 1, handNum == 0 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
    }*/

    bool TriggerDown()
    {
        return triggerAxis > 0.75;
    }

    bool GripDown()
    {
        return gripAxis > 0.75;
    }

    bool CheckInterior()
    {
        Vector3 castDirection = GetComponent<Transform>().position - Head.GetComponent<Transform>().position;

        float length = castDirection.magnitude;

        int layerMask = 1 << 9;

        return Physics.Raycast(Head.GetComponent<Transform>().position, castDirection.normalized, length, layerMask);

        //return Physics.Raycast(new Vector3(-1, 10, 0), new Vector3(1, 10, 0), 100);
    }
    
    Vector3 SwingVel()
    {
        Vector3 AvgVel = new Vector3(0, 0, 0);

        for (int i = 0; i < velHistLength; i++)
        {
            AvgVel += velHist[i] / velHistLength;
        }

        if (AvgVel.magnitude > swingMaxSpeed)
        {
            AvgVel = AvgVel.normalized * swingMaxSpeed;
        }

        return AvgVel;
    }

    void setLastStable(GameObject terrain)
    {
        if (terrain.layer == LayerMask.NameToLayer("GrabbableTerrain") && terrain.tag != "NoJumpSave" && grabbed && PlayerT.position.y > HeadScript.abyssY+10)
        {
            HeadScript.lastStablePos = PlayerT.position;
        }
    }

    void FixedUpdate()
    {
        if (grabbed)
        {
            for (int i = velHistLength - 2; i > 0; i -= 1)
            {
                velHist[i + 1] = velHist[i];

            }

            velHist[0] = grabMove / Time.deltaTime;
        }
        else
        {
            for (int i = 0; i < velHistLength; i++)
            {
                velHist[i] = new Vector3(0, 0, 0);
            }
        }
    }
        
    void OnCollisionEnter(Collision collision)
    {
        setLastStable(collision.collider.gameObject);
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            if(TriggerDown() && !alreadyCol)
            {
                grabPos = collision.contacts[0].point;
                alreadyCol = true;
                //touched = true;

                //PlayerRB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                //PlayerRB.velocity = new Vector3(0, 0, 0);
            }
            if(touchingBranches == 0 && !interior)
            {
                Vibrate(0.01f, vibOneFrame);
            }
            touchingBranches += 1;
        }

        if (collision.collider.gameObject.tag == "Vase")
        {
            HeadScript.Deposit();
        }

        Item item = collision.collider.gameObject.GetComponent<Item>();
        if (item != null)
        {
            if (HeadScript.heldScore >= 30) { return; }
            HeadScript.GetPoint(item.points);
            Debug.Log("item");
            item.OnHandCollide();
            Vibrate(item.vibDur, 0.5f);
            
        }
    }

    void OnCollisionExit(Collision collision)
    {
        setLastStable(collision.collider.gameObject);
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            touchingBranches -= 1;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        setLastStable(collision.collider.gameObject);
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.GetComponent<Updraft>())
        {
            HeadScript.updrafting = true;
        }
    }
}
