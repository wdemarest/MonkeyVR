using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    public TMP_Text Score;

    public int score;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        Score.text = "" + score;
        if (score < 0)
        {
            score = 0;
        }
        if (score > 30)
        {
            score = 30;
        }
    }

    public void GetPoint()
    {
        score += 5;
    }
    public void Reset()
    {
        score = 0;
    }
}
