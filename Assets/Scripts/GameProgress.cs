using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameProgress : MonoBehaviour
{
    public GameObject Fruit;
    public GameObject Butterfly;
    public GameObject Bubble;
    public GameObject FogCloud;
    public GameObject Chaser;
    public GameObject Turret;
    public GameObject Mine;
    public GameObject Ball;

    public int depositedScore;
    public int gameStage = 0;

    public class BiomeData
    {
        public int Cost { get; set; }
        public GameObject Land { get; set; }
        public int[] SpawnCounts { get; set; }
        //public int[] ForestSpawnCounts { get; set; }

        public BiomeData(int cost, string landName)
        {
            Cost = cost;
            Land = GameObject.Find(landName);
            //SpawnCounts = spawnCounts;
            //ForestSpawnCounts = forestSpawnCounts;
        }
    }

    enum Unit
    {
        Fruit,
        Mine,
        FogCloud,
        RocketTurret,
        LaserTurret,
        Chaser
    }

    /*public class UnitsToMake
    {
        public Unit unit;
        public int count;
    }*/

    public List<BiomeData> biomeList;


    public void Deposit(int points)
    {
        if(points > 0)
        {
            depositedScore += points;
            
        }
    }

    void Start()
    {
        //Debug.Log("Game Start");

        biomeList = new List<BiomeData>();

       /*biomeList.Add(
            new BiomeData(
                cost:0,
                landName: "Forest",
                new List<UnitsToMake> {
                    { Unit.Fruit, 1000 },
                    { Unit.Mine, 500 }
                }
            )
        );*/
        biomeList.Add(new BiomeData(3, "CavernTree"));
        biomeList.Add(new BiomeData(6, "Mushrooms"));
        biomeList.Add(new BiomeData(7, "FloatingForest"));
        biomeList.Add(new BiomeData(10, "TwistedTree"));

        gameStage--;
        AdvanceGameStage();
    }

    /*void Populate(int[] spawnCounts)
    {
        for (int i = 0; i < spawnCounts[0]; i++)
        {
            Instantiate(Fruit, placePos(150, 175, 50), Quaternion.Euler(-90, 0, 0));
        }
    }*/

    Vector3 placePos(float placeWidth, float placeTop, float placeBottom)
    {
        return new Vector3(UnityEngine.Random.Range(-1 * placeWidth, placeWidth), UnityEngine.Random.Range(placeTop, placeBottom), UnityEngine.Random.Range(-1 * placeWidth, placeWidth));
    }

    void AdvanceGameStage()
    {
        gameStage++;
        //Debug.Log(gameStage);
        //Debug.Log(biomeList.Count);

        for (int i = 0; i < biomeList.Count; i++)
        {
            GameObject Vase = GameObject.Find("Vase" + i).transform.Find("Vase").gameObject;
            
            if(i < gameStage)
            {
                Vase.transform.Find("VaseDeposits").gameObject.SetActive(false);
            }
            else if (i == gameStage)
            {
                Vase.SetActive(true);
            }
            else if (i > gameStage)
            {
                Vase.SetActive(false);
            }
        }
    }
     
    void Update()
    {
        if (depositedScore >= biomeList[gameStage + 1].Cost)
        {
            {
                AdvanceGameStage();
            }
        }

        BiomeData newBiome = biomeList[gameStage];
        if (Mathf.Abs(newBiome.Land.transform.position.y) > 3)
        {
            newBiome.Land.transform.Find("AppearParticles").GetComponent<ParticleSystem>().Play();
            newBiome.Land.transform.Translate(Vector3.up * ((newBiome.Land.transform.position.y * -0.3f + (0.1f * Math.Sign(newBiome.Land.transform.position.y))) * Time.deltaTime));
            
            if (Mathf.Abs(newBiome.Land.transform.position.y) <= 3)
            {
                newBiome.Land.transform.position = new Vector3(newBiome.Land.transform.position.x, 0, newBiome.Land.transform.position.z);
                newBiome.Land.transform.Find("AppearParticles").GetComponent<ParticleSystem>().Stop();
                //Populate(newBiome.SpawnCounts);
            }   
        }
    }
}