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
    float heatlhMax = 100;
    public TMP_Text Health;

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
        PlayerRB.useGravity = !(gravGrace || levitation);

        if(transform.position.y < abyssY)
        {
            health -= 15;
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

        if (inCloud)
        {
            RenderSettings.fogColor = Color.red;
            inCloud = false;
        }
        else
        {
            RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }

    public void GetPoint(int points)
    {
        heldScore += points;
    }

    public void takeDamage(float damageTaken)
    {
        health -= damageTaken;
        FindObjectOfType<AudioManager>().Play("TakeDamage");
    }
    
    public void Reset()
    {
        health = heatlhMax;
        heldScore = 0;
        lastStablePos = new Vector3(0, 125, -3);
        JumpSave();
    }

    public void Impulse(Vector3 impulse)
    {
        PlayerRB.velocity += impulse;
    }

    public void Deposit()
    {
        GameObject.Find("GameProgress").GetComponent<GameProgress>().Deposit(heldScore);
        heldScore = 0;
        health = heatlhMax;
    }

    public void JumpSave()
    {
        PlayerRB.velocity = new Vector3(0, 0, 0);
        PlayerRB.position = lastStablePos;
        gravGrace = true;
    }
}