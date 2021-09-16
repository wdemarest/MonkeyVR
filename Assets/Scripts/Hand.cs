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
    public int handNum = -1;
    public bool grabOverridden = false;
    public Rigidbody PlayerRB;
    public Transform PlayerT;
    public Material defaultMat;
    public Material grabbedMat;
    public GameObject Gun;
    public TMP_Text LogText;
    public GameObject Projectile;
    public GameObject Indicator;

    public AudioSource PistolShoot;
    public AudioSource grab;

    Vector3 lastPosition;
    Vector3 lastPlayerPosition;
    float triggerAxis = 0;
    float gripAxis = 0;

    float moveSpeed = 1.5f;
    float moveMaxSpeed;
    int histLength = 0;
    int maxFruitCarry = 6;
    float thrusterSpeed = 1.5f;
    float vibDurationRemaining = 0f;
    const float vibOneFrame = 0.01f;

    int touchingBranches = 0;
    bool grabbed = false;
    int grabbedSampleCount = 0;
    bool interior = false;
    bool jumpSaving = false;
    public bool lastTriggerDown = false;
    bool alreadyCol = false;
    bool gunMode = false;

    Vector3[] velHist;

    Vector3 grabPos;
    Vector3 grabMove;

    static bool[] handAssigned = new bool[3] { false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(handNum == 2 || handNum == 1);
        Debug.Assert(handAssigned[handNum] == false);
        handAssigned[handNum] = true;
        //Debug.Assert(histLength == 0);
        histLength = (int)(1 / Time.fixedDeltaTime);
        //Debug.Assert(histLength == 50);
        HeadScript = Head.GetComponent<Head>();
        velHist = new Vector3[histLength];
        devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        PlayerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        moveMaxSpeed = HeadScript.moveMaxSpeed;
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
            //Debug.Log("NODEVS");
            return;
        }
        else if (devices.Count <= 1)
        {
            LogText.text = "OneDev";
            //Debug.Log("OneDev");
            return;
        }

        Debug.Assert(handNum == 2 || handNum == 1);
        targetDevice = devices[handNum];

        LogText.text = ""+devices;
        Debug.Log("" + devices);

        /*if (targetDevice)
        {
            LogText.text = devices;
        }*/

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
            Debug.Log("ManualReset");
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
            PistolShoot.Play();
            Vibrate(0.25f, 0.1f);
        }

        //GRABBING
        if (TriggerDown() && !gunMode && (touchingBranches > 0 || interior) && !grabOverridden)
        {
            if (!grabbed) {

                //LogText.text = ""+PlayerRB.velocity.magnitude;

                grab.Play();

                Vibrate(PlayerRB.velocity.magnitude/20, 0.05f);

                GetComponent<Renderer>().material = grabbedMat;
                
                if (grabPos == new Vector3(0, 0, 0))
                {
                    grabPos = transform.position;
                    //Debug.Log("GrabPos " + grabPos);
                }
                
                OtherHand.GetComponent<Hand>().grabOverridden = true;
            }


            grabbed = true;

            PlayerRB.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            grabPos = new Vector3(0, 0, 0);
            //Debug.Log("GrabPos " + grabPos);

            PlayerRB.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);

            if (grabbed)
            {
                GetComponent<Renderer>().material = defaultMat;

                PlayerRB.velocity = SwingVel();
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
        Debug.Assert(histLength != 0);

        //collects dotproducts of hand movement vectors

        //OHHHH SHIT DAWG! Higher dotproducts mean straighter angles between two vectors. This makes an array that start only once the dotproducts are above
        //the validThreshold (straight enough) and ends only once they're below the validThreshold.

        bool straightEnough = false;
        float validThreshold = 0.8f; //~35 degrees
        List <Vector3> validHist = new List<Vector3>();
        for (int i = 0; i < grabbedSampleCount-1; i++)
        {
            float dotProduct = Vector3.Dot(velHist[i].normalized, velHist[i + 1].normalized);
            if (dotProduct > validThreshold) { straightEnough = true; }
            if (straightEnough)
            {
                validHist.Add(velHist[i]);
                if (dotProduct <= validThreshold ) { break; }
            }
        }

        Vector3 AvgVel = new Vector3(0, 0, 0);

        if (validHist.Count == 0)
        {
            return AvgVel;
        }


        //Use angle of whole swing but only velocity of last few because we want the release velocity.
        for (int i = 0; i < validHist.Count; i++)
        {
            AvgVel += validHist[i] / validHist.Count;
        }

        Vector3 angle = AvgVel.normalized;


        float velocity = 0.0f;
        int velSamplesTaken = Mathf.Min(validHist.Count, 3);
        for (int i = 0; i < velSamplesTaken; i++)
        {
            velocity += validHist[i].magnitude / velSamplesTaken;
        }

        AvgVel = angle * Mathf.Min(velocity * moveSpeed, moveMaxSpeed);


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
        for (int i = histLength - 2; i >= 0; i -= 1)
        {
            velHist[i + 1] = velHist[i];
                
        }

        if (grabbed)
        {
            velHist[0] = grabMove / Time.deltaTime;
            grabbedSampleCount = Mathf.Min(grabbedSampleCount+1,histLength);
            //Debug.Log("velHist[0]="+ velHist[0]);
        }
        else {
            velHist[0] = new Vector3(0, 0, 0);
            grabbedSampleCount = 0;
            //Debug.Log("clearing velHist[0]");
        }
    }
        
    //HAND IS GRABBING WHILE ALREADY GRABBED CREATING SKITTER
    void OnCollisionEnter(Collision collision)
    {
        setLastStable(collision.collider.gameObject);
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            //Debug.Log("ColEnt");
            if(TriggerDown() && !alreadyCol)
            {
                if (!grabbed) { grabPos = collision.contacts[0].point; }
                //Instantiate(Indicator, collision.contacts[0].point, Quaternion.Euler(0, 0, 0));

                alreadyCol = true;
                //touched = true;

                //PlayerRB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                PlayerRB.velocity = new Vector3(0, 0, 0);
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
            if (HeadScript.heldScore >= maxFruitCarry) { return; }
            HeadScript.GetPoint(item.points);
            //Debug.Log("item");
            item.OnHandCollide();
            Vibrate(item.vibDur, 0.5f);
            
        }
    }

    void OnCollisionExit(Collision collision)
    {
        setLastStable(collision.collider.gameObject);
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            //Debug.Log("ColEx");
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