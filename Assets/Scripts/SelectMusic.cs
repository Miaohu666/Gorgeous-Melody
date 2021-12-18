using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;
using UnityEngine.Networking;

public class SelectMusic : MonoBehaviour
{
    public GameObject beatmapMannager;
    public GameObject beatmapSelector;

    public Button mItemPrefab;//要添加到列表的预设体按钮组件
    public Transform mContentTransform;//容器Content的transform

    public Scrollbar mScrollbar;//滑动条
    List<myButton> lists = new List<myButton>();//存放按钮对象组件
    float itemHeight;//单个按钮组件的height
    RectTransform rect;//容器content的rect
    public VerticalLayoutGroup group;//用于计算内容的高度

    

    class myButton
    {
        public Button item;
        string filename;

        public myButton(string filename_,Button mItemPrefab_, Transform mContentTransform_)
        {
            filename = filename_;
            item = Instantiate(mItemPrefab_);
            item.GetComponentInChildren<TMP_Text>().text = filename_;
            item.transform.parent = mContentTransform_;
        }

        public string getFileName()
        {
            return filename;
        }
    }

    public void clearAllButton()
    {
        for (int i = 0; i < lists.Count; i++)
        {
            Destroy(lists[i].item.gameObject);
        }
        lists = new List<myButton>();
        //rect.sizeDelta = new Vector2(rect.sizeDelta.x, lists.Count * itemHeight);
    }

    void Start()
    {
        rect = mContentTransform.GetComponent<RectTransform>();
        itemHeight = mItemPrefab.GetComponent<RectTransform>().rect.height;
        //mScrollbar.value = 1.0f;
    }

    IEnumerator GetAudioClip(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
        {
            // 向目的地址发送请求，此处为本地请求，并等待回复
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                print("1");
                Debug.Log(www.error);
            }
            else
            {
                ReadInputField judge = beatmapMannager.GetComponent<ReadInputField>();
                // 从www中获得资源
                judge.BGM.clip = DownloadHandlerAudioClip.GetContent(www);
                judge.BGM.Play();
            }
        }
    }


    [System.Obsolete]
    public void selectmusic(string path)
    {
        string cwd = System.Environment.CurrentDirectory;
        //string path = @"F:\Gorgeous-Melody-develop\Assets\OSUmaps\581729 jioyi - cyanine";
        DirectoryInfo root = new DirectoryInfo(path);

            foreach (DirectoryInfo f in root.GetDirectories())
            {
                myButton btn = new myButton(f.Name, mItemPrefab, mContentTransform);
                lists.Add(btn);
                //给每个按钮组件监听点击事件
                btn.item.onClick.AddListener(
                    () =>
                    {
                        ConvetBeatmapOSUMania conveter = beatmapMannager.GetComponent<ConvetBeatmapOSUMania>();//设定谱面文件夹所在的路径
                        string temp = path + "/" + btn.getFileName();
                        conveter.sourceFilePath = temp;

                        ReadInputField judge = beatmapMannager.GetComponent<ReadInputField>();
                        judge.isSelected = false;
                        judge.isReady();

                         
                        foreach (FileInfo f in new DirectoryInfo(temp).GetFiles("*.jpg"))
                        {
                            Texture2D img = null;
                            WWW www = new WWW("file://"+f.FullName);
                            
                            print("file://" + f.FullName);
                        
                                img = www.texture;
                                Sprite sprite = Sprite.Create(img, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f));
                                judge.image.sprite = sprite;
                            
                        }

                       // foreach (FileInfo f in new DirectoryInfo(temp).GetFiles("*.mp3"))
                        //{
                            StartCoroutine(GetAudioClip("file://" + temp + "/audio.mp3"));//播放选中的音乐
                            
                        //}

                        
                        selectBeatmap beatmapselector = beatmapSelector.GetComponent<selectBeatmap>();
                        beatmapselector.clearAllButton();//清空原有的按钮
                        beatmapselector.selectBeatMap(temp);//读取玩家所选择的音乐下的所有谱面文件
                        judge.audioIF.text = btn.getFileName();
                    }
                );
            }
        
    }
    void Update()
    {
    }

    //使列表跳转到顶部
    void ToTopFunc()
    {

        //offsetMin 是vector2(left, bottom);

        //offsetMax 是vector2(right, top);

        rect.offsetMin = new Vector2(rect.offsetMin.x, -rect.sizeDelta.y);
        rect.offsetMax = new Vector2(rect.offsetMax.x, 0);


    }
    //使列表跳转到底部
    void ToBottomFunc()
    {

        /*rect.offsetMin = new Vector2(rect.offsetMin.x, 0);
        rect.offsetMax = new Vector2(rect.offsetMax.x, rect.sizeDelta.y);*/
    }
    void onClickFunc(myButton btn)
    {
        Debug.Log(btn.getFileName());
    }
    //清空列表
    //删除单个按钮组件
}
