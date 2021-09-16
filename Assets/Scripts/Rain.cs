using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    public GameObject RainParticles;
    public GameObject Head;

    public float rainTimer = 0;
    public float rainInterval = 10;
    public float rainDuration = 15;
    public bool isRaining;
    public bool thundered = true;
    public float rainHeight = 50;

    public AudioSource thunder;

    float damageTimer = 0;
    float damageInterval = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rainTimer += Time.deltaTime;

         isRaining = rainTimer > rainInterval;
        if(isRaining != RainParticles.active)
        {
            RainParticles.SetActive(isRaining);
        }

        if(rainTimer > rainInterval - 15 && !thundered)
        {
            thundered = true;
            thunder.Play();
        }
        
        if (rainTimer > rainInterval + rainDuration)
        {
            rainTimer = 0;
            thundered = false;
        }

        if (isRaining)
        {
            RainParticles.transform.position = Head.transform.position + new Vector3(0, rainHeight, 0);
            
            bool touchingPlayer = !Physics.Raycast(Head.transform.position, new Vector3(0, 1, 0), rainHeight, LayerMask.GetMask("GrabbableTerrain"));
            if (touchingPlayer)
            {
                if(damageTimer >= damageInterval)
                {
                    Head.GetComponent<Head>().takeDamage(1);
                    damageTimer -= damageInterval;
                }
                damageTimer += Time.deltaTime;
            }
            else
            {
                damageTimer = 0;
            }
        }

    }
}
