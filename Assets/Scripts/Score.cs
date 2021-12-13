using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }

    public int TotalScore { get; set; } = 0;

    public int maxCombo { get; set; } = 0;

    public int missedJudge { get; set; } = 0;

    public int _4thJudge { get; set; } = 0;

    public int _3rdJudge { get; set; } = 0;

    public int _2ndJudge { get; set; } = 0;

    public int _1stJudge { get; set; } = 0;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}