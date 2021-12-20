using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using SonicBloom.Koreo.Demos;

public class ScoreBoardController : MonoBehaviour
{
    public TMP_Text totalScore;
    public TMP_Text maxCombo;
    public TMP_Text _1stJudge;
    public TMP_Text _2ndJudge;
    public TMP_Text _3rdJudge;
    public TMP_Text _4thJudge;
    public TMP_Text missedJudge;
    public TMP_Text _rank;


    public void backClicked()
    {
        Destroy(FindObjectOfType<LoadKoreoInfo>().gameObject);
        SceneManager.LoadScene("LoadMusicFromPlayerScene");

    }

    public void restartClicked()
    {
        SceneManager.LoadScene("PlayingScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        /*totalScore = GameObject.Find("Canvas/TotalScore").GetComponent<TMP_Text>();
        maxCombo = GameObject.Find("Canvas/maxCombo").GetComponent<TMP_Text>();
        _1stJudge = GameObject.Find("Canvas/1stJudge").GetComponent<TMP_Text>();
        _2ndJudge = GameObject.Find("Canvas/2ndJudge").GetComponent<TMP_Text>();
        _3rdJudge = GameObject.Find("Canvas/3rdJudge").GetComponent<TMP_Text>();
        _4thJudge = GameObject.Find("Canvas/4thJudge").GetComponent<TMP_Text>();
        missedJudge = GameObject.Find("Canvas/missedJudge").GetComponent<TMP_Text>();
        _rank = GameObject.Find("Canvas/Rank").GetComponent<TMP_Text>();*/

        totalScore.text = Score.Instance.TotalScore.ToString();
        maxCombo.text = Score.Instance.maxCombo.ToString();
        _1stJudge.text = Score.Instance._1stJudge.ToString();
        _2ndJudge.text = Score.Instance._2ndJudge.ToString();
        _3rdJudge.text = Score.Instance._3rdJudge.ToString();
        _4thJudge.text = Score.Instance._4thJudge.ToString();
        missedJudge.text = Score.Instance.missedJudge.ToString();

        string rank = calc_ranking();
        _rank.text = rank;

    }

    private string calc_ranking()
    {
        int[] judges = { Score.Instance._1stJudge, Score.Instance._2ndJudge, Score.Instance._3rdJudge
                        ,Score.Instance._4thJudge, Score.Instance.missedJudge};
        double[] standers = { 0.9f, 0.75f, 0.65f, 0.45f };
        int all_notes = 0;
        foreach(int i in judges){
            all_notes += i;
        }

        double accu = (judges[0] * 1 + judges[1] * 0.7 + judges[2] * 0.5 + judges[3] * 0.3) / all_notes;
        bool allcombo_flag = (all_notes == Score.Instance.maxCombo);

        if (allcombo_flag)
        {
            if (accu > standers[0])
                return "S";
            else
                return "A";
        }

        if (accu > standers[0])
        {
            return "A";
        }
        else if (accu > standers[1])
        {
            return "B";
        }
        else if (accu > standers[2])
        {
            return "C";
        }
        else if (accu > standers[3])
        {
            return "D";
        }
        else
        {
            return "ERR";
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
