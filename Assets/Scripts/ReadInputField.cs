using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

/* ReadInputField： 从文本输入框中提取字符串，然后传递给转换脚本
 * 层级：UI
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

    public List<string> nameOfbeatmap;
    public List<string> nameOfmusic;

    public GameObject musicMannager;

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
        //添加一个下拉选项
        Dropdown.OptionData data = new Dropdown.OptionData();
        data.text = itemText;
        //data.image = "指定一个图片做背景不指定则使用默认"；
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
            musicSelector.clearAllButton();//清空当前滑动选择列表中的按钮
            musicSelector.selectmusic(fullDirPath);//将新路径下的音乐及其谱面文件加载到滑动列表中
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
        ofn2.pszDisplayName = new string(new char[2000]); ;     // 存放目录路径缓冲区  
        ofn2.lpszTitle = "请选择谱面文件所在文件夹";// 标题  
        IntPtr pidlPtr = DllOpenFileDialog.SHBrowseForFolder(ofn2);

        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';

        DllOpenFileDialog.SHGetPathFromIDList(pidlPtr, charArray);
        fullDirPath = new String(charArray);
        fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));
        print(fullDirPath);//这个就是选择的目录路径。

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
        musicSelector.clearAllButton();//清空当前滑动选择列表中的按钮
        musicSelector.selectmusic(fullDirPath);//将新路径下的音乐及其谱面文件加载到滑动列表中
    }

    public void onStart(string scene)
    {
        SceneManager.LoadScene(scene);

        ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
        conveter.StartConvet();

        BGM.Stop();
    }
}
