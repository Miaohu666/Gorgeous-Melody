using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamingInfoDisplayUI : MonoBehaviour
{
    #region Fields
    public TMP_Text judgeTextUI;
    private CanvasGroup judgeTextCG;
    public TMP_Text comboTextUI;
    public TMP_Text comboValueUI;
    public TMP_Text scoreTextUI;
    public TMP_Text scoreValueUI;
    private Color judge_default_color;

    #endregion

    #region method 

    public void showJudgeUI(string judgement)
    {
        StopCoroutine(judgeUIAutoFade());
        if (judgeTextCG.alpha <= 0.0f)
        {
            judgeTextCG.alpha = 1.0f;

        }

        judgeTextUI.text = judgement;

        if (judgement == "Miss")
        {
            judgeTextUI.color = Color.red;
        }
        else
        {
            judgeTextUI.color = judge_default_color;
        }
        judgeTextUI.gameObject.GetComponent<UITextAnimation>().StartAnimScale();

        if (judgeTextCG.alpha >= 1.0f)
        {
            StartCoroutine(judgeUIAutoFade());
        }
    }

    IEnumerator judgeUIAutoFade()
    {
        yield return new WaitForSeconds(1.0f);
        if (judgeTextCG.alpha >= 1.0f)
        {
            judgeTextCG.alpha = 0.0f;
        }
    }

    public void showComboUI()
    {
        comboTextUI.enabled = true;
        comboValueUI.enabled = true;
    }
    public void hideComboUI()
    {
        comboTextUI.enabled = false;
        comboValueUI.enabled = false;
    }

    public void showScoreUI()
    {
        scoreTextUI.enabled = true;
        scoreValueUI.enabled = true;
    }
    public void hideScoreUI()
    {
        scoreTextUI.enabled = false;
        scoreValueUI.enabled = false;
    }

    public void setScoreValue(int score)
    {
        scoreValueUI.text = "" + score;
    }
    public void startCombo()
    {
        comboTextUI.enabled = true;
        // combo从2开始计算
        comboValueUI.text = "2";
        comboValueUI.enabled = true;
    }
    public void addComboValue()
    {
        int combo = int.Parse(comboValueUI.text);
        comboValueUI.text = "" + (combo + 1);
        comboValueUI.gameObject.GetComponent<UITextAnimation>().StartAnimScale();

    }
    public void setComboValue(int combo)
    {
        comboValueUI.text = "" + combo;
    }
    public void resetCombo()
    {
        // comboValueUI.gameObject.GetComponent<UITextAnimation>().StopAnimPlay();
        comboValueUI.text = "";

    }


    void Start()
    {
        judge_default_color = judgeTextUI.color;
        scoreValueUI.gameObject.GetComponent<UITextAnimation>().StartAnimWave();
        judgeTextCG = judgeTextUI.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal static void restart()
    {
        throw new NotImplementedException();
    }
    #endregion
}
