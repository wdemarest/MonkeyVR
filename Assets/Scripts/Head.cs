using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Head : MonoBehaviour
{
    public Rigidbody PlayerRB;
    public bool gravGrace = false;
    public bool levitation = false;
    public bool updrafting = false;
    public TMP_Text HeldScore;
    public Vector3 lastStablePos;
    public float thrustRemaining;
    public float thrustMax;
    public bool thrustRanOut = false;
    public bool usingThrust;
    public bool inCloud = false;
    public float abyssY = 40f;
    public float updraftSpeed = 3f;
    public float health;
    public int heldScore = 0;
    public float healthMax = 50;
    public TMP_Text Health;
    public float moveMaxSpeed = 8.5f;

    public AudioSource takeDamageSound;
    public AudioSource headBonk;
    public AudioSource velWind;
    float velWindVol;

    SphereCollider SC;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
        SC = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerRB.velocity.magnitude > 13) { PlayerRB.velocity *= 13 / PlayerRB.velocity.magnitude; }

        PlayerRB.useGravity = !(gravGrace || levitation);

        if(transform.position.y < abyssY)
        {
            takeDamage(15);
            JumpSave();
        }

        if (updrafting)
        {
            Impulse(new Vector3(0, updraftSpeed * Time.deltaTime, 0));
            updrafting = false;
        }

        Health.text = ""+health;

        if(health <= 0)
        {
            Debug.Log("DeathReset");
            Reset();
        }

        HeldScore.text = "" + heldScore;

        if (usingThrust)
        {
            if (thrustRemaining > 0)
            {
                thrustRemaining -= Time.deltaTime;
            }
            else
            {
                thrustRemaining = 0;
                thrustRanOut = true;
            }
        }
        else
        {
            thrustRanOut = false;
            if (thrustRemaining < thrustMax)
            {
                thrustRemaining += Time.deltaTime;
            }
            else
            {
                thrustRemaining = thrustMax;
            }
        }

        usingThrust = false;

        

        //FogLightness
        float fogLightness = (GetComponent<Transform>().position.y - 60) * (160f / 500f);
        fogLightness += 60;
        fogLightness /= 220;

        Color fogColor = new Color(fogLightness, fogLightness, fogLightness, 1);
        RenderSettings.fogColor = fogColor;
        if (inCloud)
        {
            RenderSettings.fogColor = Color.red;
            inCloud = false;
        }
        else
        {
            //RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f, 1);
        }


        float volMin = 0.1f;
        float volMax = 0.30f;
        velWindVol = volMin + (volMax-volMin) * ((PlayerRB.velocity.magnitude / moveMaxSpeed));

        velWind.volume = velWindVol;
        velWind.pitch = 1 + (velWindVol / 2);
    }

    public void GetPoint(int points)
    {
        heldScore += points;
    }

    public void takeDamage(float damageTaken)
    {
        health -= damageTaken;
        takeDamageSound.Play();
    }
    
    public void Reset()
    {
        Debug.Log("Reset");
        lastStablePos = new Vector3(0, 125, -3);
        JumpSave();
        health = healthMax;
        heldScore = 0;
    }

    public void Impulse(Vector3 impulse)
    {
        PlayerRB.velocity += impulse;
    }

    public void Deposit()
    {
        GameObject.Find("GameProgress").GetComponent<GameProgress>().Deposit(heldScore);
        heldScore = 0;
        health = healthMax;
    }

    public void JumpSave()
    {
        PlayerRB.velocity = new Vector3(0, 0, 0);
        PlayerRB.position = lastStablePos;
        gravGrace = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("headbonk");
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            headBonk.Play();
            
        }
    }
}