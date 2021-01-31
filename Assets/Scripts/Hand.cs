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
    public int handNum;
    public bool grabOverridden = false;
    public bool flight;
    public Rigidbody PlayerRB;
    public Transform PlayerT;
    public Material defaultMat;
    public Material grabbedMat;
    public TMP_Text LogText;

    Vector3 lastPosition;
    Vector3 lastPlayerPosition;
    //float gripAxis = 0;
    float triggerAxis = 0;
    
    float moveSpeed = 20f;
    int velHistLength = 15;
    float swingMaxSpeed = 0.4f;
    //float vibOnGrabDur = 0.25f;

    int touchingBranches = 0;
    bool grabbed = false;
    bool interior = false;
    bool jumpSaving = false;
    bool lastTriggerDown = false;
    bool alreadyCol = false;
    //float vibRemaining = 0;

    Vector3[] velHist;

    Vector3 grabPos;
    Vector3 grabMove;
    

    // Start is called before the first frame update
    void Start()
    {
        velHist = new Vector3[velHistLength];
        devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        PlayerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void VibrationTest()
    {
        OVRInput.SetControllerVibration(0.6f, 0.6f, OVRInput.Controller.RTouch);
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
        targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 myPos);
        targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion myRot);
        targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool restartButton);
        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton);

        //OVRInput.SetControllerVibration(1, 1, handNum == 0 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);

        if (restartButton)
        {
            Head.GetComponent<Head>().Reset();
        }

        if (handNum == 1) {
            Head.GetComponent<Head>().levitation = secondaryButton;
            if (secondaryButton)
            {
                VibrationTest();
            }
        }

        if (handNum == 2)
        {
            if (secondaryButton && !jumpSaving)
            {
                Head.GetComponent<Head>().JumpSave();
                GameObject.Find("ScoreTracker").GetComponent<ScoreTracker>().score--;
            }
            jumpSaving = secondaryButton;
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

        //if (TriggerDown() && !lastTriggerDown)
        //{
            interior = CheckInterior();
        //}
        
        //GRABBING
        if (TriggerDown() && (touchingBranches > 0 || interior) && !grabOverridden)
        {
            if (!grabbed) {

                GetComponent<Renderer>().material = grabbedMat;

                //GetComponent<AudioSource>().Play();
                
                if (grabPos == new Vector3(0, 0, 0))
                {
                    grabPos = transform.position;
                }
                
                OtherHand.GetComponent<Hand>().grabOverridden = true;
            }


            grabbed = true;


            Head.GetComponent<Head>().lastStablePos = PlayerT.position;

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
            Head.GetComponent<Head>().gravGrace = false;
        }

        LogText.text = ""+Head.GetComponent<Head>().gravGrace;

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
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            if(TriggerDown() && !alreadyCol)
            {
                grabPos = collision.contacts[0].point;
                alreadyCol = true;
                //touched = true;

                //PlayerRB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                //PlayerRB.velocity = new Vector3(0, 0, 0);
            }
            touchingBranches += 1;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            touchingBranches -= 1;
        }
    }
}
