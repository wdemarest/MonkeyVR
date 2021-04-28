using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameProgress : MonoBehaviour
{
    public TMP_Text VaseDeposits;

    public int depositedScore;

    public void Deposit(int points)
    {
        FindObjectOfType<AudioManager>().Play("Hit");
        depositedScore += points;
        VaseDeposits.text = ""+depositedScore;
    }

    void Start()
    {
        Debug.Log("Game Start");
    }
}
