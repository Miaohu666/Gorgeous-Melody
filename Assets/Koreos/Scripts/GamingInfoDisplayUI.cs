using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamingInfoDisplayUI : MonoBehaviour
{
    #region Fields
    public TMP_Text judgeTextUI;
    public TMP_Text comboTextUI;
    public TMP_Text comboValueUI;
    public TMP_Text scoreTextUI;
    public TMP_Text scoreValueUI;

    #endregion

    #region method 

    public void showJudgeUI(string judgement)
    {
        // TODO:显示一个判定结果，并在固定时间后消失 或在下一个判定结果到来时被覆盖
        judgeTextUI.text = judgement;
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
    }
    public void setComboValue(int combo)
    {
        comboValueUI.text = "" + combo;
    }
    public void resetCombo()
    {
        comboValueUI.text = "";
    }
    
    void Start()
    {
        
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
