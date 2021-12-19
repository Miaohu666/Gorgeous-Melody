using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// �������˵��ϵ� Dropdown����Ƴ����滻Ϊ�ýű�
/// </summary>
public class ChinarDropdown : Dropdown
{
    public bool AlwaysCallback = false;//�Ƿ��� ���ѡ�ť���ǻص�


    public void Show()
    {
        base.Show();
        Transform toggleRoot = transform.Find("Dropdown List/Viewport/Content");
        Toggle[] toggleList = toggleRoot.GetComponentsInChildren<Toggle>(false);
        for (int i = 0; i < toggleList.Length; i++)
        {
            Toggle temp = toggleList[i];
            temp.onValueChanged.RemoveAllListeners();
            temp.isOn = false;
            temp.onValueChanged.AddListener(x => OnSelectItemEx(temp));
        }
    }


    public override void OnPointerClick(PointerEventData eventData)
    {
        Show();
    }


    public void OnSelectItemEx(Toggle toggle)
    {
        if (!toggle.isOn)
        {
            toggle.isOn = true;
            return;
        }

        int selectedIndex = -1;
        Transform tr = toggle.transform;
        Transform parent = tr.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i) == tr)
            {
                selectedIndex = i - 1;
                break;
            }
        }

        if (selectedIndex < 0)
            return;
        if (value == selectedIndex && AlwaysCallback)
            onValueChanged.Invoke(value);
        else
            value = selectedIndex;
        Hide();
    }
}