using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using SonicBloom.Koreo.Demos;

/* ReadInputField�� ���ı����������ȡ�ַ�����Ȼ�󴫵ݸ�ת���ű�
 * �㼶��UI
 */
public class ReadInputField : MonoBehaviour
{
 
    public bool isSelected = false;

    public TMP_Text beatmapIF;
    public TMP_Text audioIF;

    public TMP_InputField pathIF;
   
    public Slider slider;
    public TMP_Text readoutSpeedValue;
    public Button startButton;
    public Toggle bgVideoToggle;
    public Toggle autoModeToggle;


    public List<string> nameOfbeatmap;
    public List<string> nameOfmusic;

    public GameObject musicMannager;
    public LoadKoreoInfo koreoLoadManager;

    public AudioSource BGM;

    string fullDirPath;

    public Image image;
    public void isReady()
    {
        if(isSelected)
        {
            startButton.interactable = true;
            print("Ready");
            
        }
        else
            startButton.interactable = false;

    }

    public void OnLoadCliked()
    {
        
    }

    public void OnNoteSpeedValueChanged()
    {
        readoutSpeedValue.text = ((int)slider.value).ToString();
        PlayerPrefs.SetFloat("NoteSpeed", (int)slider.value);
    }


    void AddDropDownOptionsData(Dropdown dropDown,string itemText)
    {
        //����һ������ѡ��
        Dropdown.OptionData data = new Dropdown.OptionData();
        data.text = itemText;
        //data.image = "ָ��һ��ͼƬ��������ָ����ʹ��Ĭ��"��
        dropDown.options.Add(data);
    }

    // Start is called before the first frame update
    void Start()
    {
        fullDirPath = PlayerPrefs.GetString("dirPath", "");
        if(fullDirPath != "")
        {
            ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
            conveter.sourceFilePath = fullDirPath;

            isSelected = false;
            isReady();

            SelectMusic musicSelector = musicMannager.GetComponent<SelectMusic>();
            musicSelector.clearAllButton();//��յ�ǰ����ѡ���б��еİ�ť
            musicSelector.selectmusic(fullDirPath);//����·���µ����ּ��������ļ����ص������б���
        }
        slider.value = PlayerPrefs.GetFloat("NoteSpeed", 8.0f);
        readoutSpeedValue.text = slider.value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getDIRPath()
    {
        OpenDialogDir ofn2 = new OpenDialogDir();
        ofn2.pszDisplayName = new string(new char[2000]); ;     // ���Ŀ¼·��������  
        ofn2.lpszTitle = "��ѡ�������ļ������ļ���";// ����  
        IntPtr pidlPtr = DllOpenFileDialog.SHBrowseForFolder(ofn2);

        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';

        DllOpenFileDialog.SHGetPathFromIDList(pidlPtr, charArray);
        fullDirPath = new String(charArray);
        fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));
        print(fullDirPath);//�������ѡ���Ŀ¼·����

        if(fullDirPath == "")
        {
            fullDirPath = PlayerPrefs.GetString("dirPath", "");
        }

        ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
        conveter.sourceFilePath = fullDirPath;
        PlayerPrefs.SetString("dirPath",fullDirPath);

        isSelected = false;
        isReady();

        SelectMusic musicSelector = musicMannager.GetComponent<SelectMusic>();
        musicSelector.clearAllButton();//��յ�ǰ����ѡ���б��еİ�ť
        musicSelector.selectmusic(fullDirPath);//����·���µ����ּ��������ļ����ص������б���
    }

    public void onStart(string scene)
    {
        koreoLoadManager.bgPicSprite = image.sprite;

        koreoLoadManager.is_background_video = bgVideoToggle.isOn;

        koreoLoadManager.is_auto_mode = autoModeToggle.isOn;

        ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
        conveter.StartConvet();

        BGM.Stop();

        SceneManager.LoadScene(scene);
    }
}
