using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoardController : MonoBehaviour
{

    public void backClicked()
    {
        SceneManager.LoadScene("LoadMusicFromPlayerScene");
    }

    public void restartClicked()
    {
        SceneManager.LoadScene("PlayingScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        Text totalScore = GameObject.Find("Canvas/TotalScore").GetComponent<Text>();
        Text maxCombo = GameObject.Find("Canvas/maxCombo").GetComponent<Text>();
        Text _1stJudge = GameObject.Find("Canvas/1stJudge").GetComponent<Text>();
        Text _2ndJudge = GameObject.Find("Canvas/2ndJudge").GetComponent<Text>();
        Text _3rdJudge = GameObject.Find("Canvas/3rdJudge").GetComponent<Text>();
        Text _4thJudge = GameObject.Find("Canvas/4thJudge").GetComponent<Text>();

        totalScore.text = Score.Instance.TotalScore.ToString();
        maxCombo.text = Score.Instance.maxCombo.ToString();
        _1stJudge.text = Score.Instance._1stJudge.ToString();
        _2ndJudge.text = Score.Instance._2ndJudge.ToString();
        _3rdJudge.text = Score.Instance._3rdJudge.ToString();
        _4thJudge.text = Score.Instance._4thJudge.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
