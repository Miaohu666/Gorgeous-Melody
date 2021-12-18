
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DropdownController : MonoBehaviour
{

    public Dropdown dropDown;

    // �Ƿ��Ǵ����������� Item ֵ
    private bool isCodeSetItemValue = false;

    void Start()
    {
        // ���ü���
        SetDropDownAddListener(OnValueChange);
        SetDropDownItemValue(1);
    }

    /// <summary>
    /// �������ֵ�ı��Ǵ��� (�л�����ѡ��)
    /// </summary>
    /// <param name="v">�ǵ����ѡ����OptionData�µ�����ֵ</param>
    void OnValueChange(int v)
    {
        //�л�ѡ�� ʱ�����������߼�...
        Debug.Log("��������ؼ���������..." + v);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            List<Dropdown.OptionData> listOptions = new List<Dropdown.OptionData>();
            listOptions.Add(new Dropdown.OptionData("Option 0"));
            listOptions.Add(new Dropdown.OptionData("Option 1"));

            AddDropDownOptionsData(listOptions);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {

            AddDropDownOptionsData("Option " + dropDown.options.Count);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveAtDropDownOptionsData(dropDown.options.Count - 1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearDropDownOptionsData();
        }
    }

    /// <summary>
    /// ����ѡ�������Item
    /// </summary>
    /// <param name="ItemIndex"></param>
    void SetDropDownItemValue(int ItemIndex)
    {
        // �������õ�ֵ
        isCodeSetItemValue = true;

        if (dropDown.options == null)
        {

            Debug.Log(GetType() + "/SetDropDownItemValue()/�����б�Ϊ�գ�����");
            return;
        }
        if (ItemIndex >= dropDown.options.Count)
        {
            ItemIndex = dropDown.options.Count - 1;
        }

        if (ItemIndex < 0)
        {
            ItemIndex = 0;
        }

        dropDown.value = ItemIndex;
    }


    /// <summary>
    /// �Ƿ���Ե��
    /// </summary>
    void SetDropDownInteractable()
    {
        //�Ƿ���Ե��
        dropDown.interactable = true;
    }

    /// <summary>
    /// ������ʾ�����С
    /// </summary>
    /// <param name="fontSize"></param>
    void SetDropDownCaptionTextFontSize(int fontSize)
    {
        //������ʾ�����С
        dropDown.captionText.fontSize = fontSize;
    }

    /// <summary>
    /// ��������Item��ʾ�����С
    /// </summary>
    /// <param name="fontSize"></param>
    void SetDropDownItemTextFontSize(int fontSize)
    {
        //��������Item��ʾ�����С
        dropDown.itemText.fontSize = fontSize;
    }

    /// <summary>
    /// ���һ���б���������
    /// </summary>
    /// <param name="listOptions"></param>
    void AddDropDownOptionsData(List<Dropdown.OptionData> listOptions)
    {
        dropDown.AddOptions(listOptions);
    }

    /// <summary>
    /// ���һ����������
    /// </summary>
    /// <param name="itemText"></param>
    void AddDropDownOptionsData(string itemText)
    {
        //���һ������ѡ��
        Dropdown.OptionData data = new Dropdown.OptionData();
        data.text = itemText;
        //data.image = "ָ��һ��ͼƬ��������ָ����ʹ��Ĭ��"��
        dropDown.options.Add(data);
    }


    /// <summary>
    /// �Ƴ�ָ��λ��   ����:����
    /// </summary>
    /// <param name="index"></param>
    void RemoveAtDropDownOptionsData(int index)
    {

        // ��ȫУ��
        if (index >= dropDown.options.Count || index < 0)
        {
            return;
        }

        //�Ƴ�ָ��λ��   ����:����
        dropDown.options.RemoveAt(index);
    }


    /// <summary>
    /// ֱ����������е�����ѡ��
    /// </summary>
    void ClearDropDownOptionsData()
    {
        //ֱ����������е�����ѡ�
        dropDown.ClearOptions();
    }

    /// <summary>
    /// �������ֵ�ı��Ǵ��� (�л�����ѡ��)
    /// </summary>
    void SetDropDownAddListener(UnityAction<int> OnValueChangeListener)
    {


        //�������ֵ�ı��Ǵ��� (�л�����ѡ��)
        dropDown.onValueChanged.AddListener((value) => {
            // �ֶ��������õ�ֵ�������¼���������Ҫ���Ա�������ȥ����
            if (isCodeSetItemValue == true)
            {

                isCodeSetItemValue = false;

                return;
            }

            OnValueChangeListener(value);
        });
    }



}
