using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class selectBeatmap : MonoBehaviour
{
    public GameObject beatmapMannager;

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
        public myButton(string filename_, Button mItemPrefab_, Transform mContentTransform_)
        {
            filename = filename_;
            item = Instantiate(mItemPrefab_, mContentTransform_);
            item.GetComponentInChildren<TMP_Text>().text = filename_.Split('[')[1].Split(']')[0];

            // item.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(375,200);
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

    // wht：修改：现在获取所有同一曲目下的铺面时会返回音频文件的名称。
    public string selectBeatMap(string path)
    {
        int count = 0;
        DirectoryInfo root = new DirectoryInfo(path);
        string source_audio_filename = "";

        foreach (FileInfo f in root.GetFiles("*.osu"))
        {
            

            myButton btn = new myButton(f.Name, mItemPrefab, mContentTransform);
            lists.Add(btn);
            count++;

            if (count == 1)
            {
                FileStream fs = new FileStream(Path.Combine(f.DirectoryName, f.FullName), FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string line = "";

                while (!(line = sr.ReadLine()).Equals("[Editor]"))
                {
                    if (line.Split(':')[0].Equals("AudioFilename"))
                    {
                        source_audio_filename = line.Split(':')[1].Substring(1);
                        Debug.Log("sourceAudioFileName: " + source_audio_filename);
                    }
                }
                sr.Close();
                fs.Close();
            }

            //给每个按钮组件监听点击事件
            btn.item.onClick.AddListener(
                () =>
                {
                    ConvetBeatmapOSUMania conveter = beatmapMannager.GetComponent<ConvetBeatmapOSUMania>();
                    ReadInputField judge = beatmapMannager.GetComponent<ReadInputField>();

                    conveter.sourceBeatmapName = btn.getFileName();
                    conveter.sourceAudioName = source_audio_filename;
                    judge.beatmapIF.text = btn.getFileName();
                    judge.isSelected = true;
                    judge.isReady();
                }
            );
        }
        if(count == 0)
        {
            Debug.LogError("No beatmap content found! ");
        }
        mContentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(400, count * 120);//动态更改列表内容高度
        return source_audio_filename;
    }
    void Update()
    {
    }
}
