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


    public void selectBeatMap(string path)
    {
        DirectoryInfo root = new DirectoryInfo(path);
        foreach (FileInfo f in root.GetFiles("*.osu"))
        {
            myButton btn = new myButton(f.Name, mItemPrefab, mContentTransform);
            lists.Add(btn);

            //给每个按钮组件监听点击事件
            btn.item.onClick.AddListener(
                () =>
                {
                    onClickFunc(btn);
                    ConvetBeatmapOSUMania conveter = beatmapMannager.GetComponent<ConvetBeatmapOSUMania>();
                    ReadInputField judge = beatmapMannager.GetComponent<ReadInputField>();

                    conveter.sourceBeatmapName = btn.getFileName();
                    conveter.sourceAudioName = "audio.mp3";
                    judge.beatmapIF.text = btn.getFileName();
                    judge.isSelected = true;
                    judge.isReady();
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
