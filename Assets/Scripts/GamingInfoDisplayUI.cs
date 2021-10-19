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

    #endregion

    #region method 

    public void showJudgeUI(string judgement)
    {
        // TODO:��ʾһ���ж���������ڹ̶�ʱ�����ʧ ������һ���ж��������ʱ������
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
    public void startCombo()
    {
        comboTextUI.enabled = true;
        // combo��2��ʼ����
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
    #endregion
}
