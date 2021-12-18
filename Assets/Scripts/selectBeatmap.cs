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

    public Button mItemPrefab;//Ҫ��ӵ��б��Ԥ���尴ť���
    public Transform mContentTransform;//����Content��transform
    public Scrollbar mScrollbar;//������

    List<myButton> lists = new List<myButton>();//��Ű�ť�������
    float itemHeight;//������ť�����height
    RectTransform rect;//����content��rect
    public VerticalLayoutGroup group;//���ڼ������ݵĸ߶�

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

            //��ÿ����ť�����������¼�
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

    //ʹ�б���ת������
    void ToTopFunc()
    {

        //offsetMin ��vector2(left, bottom);

        //offsetMax ��vector2(right, top);

        rect.offsetMin = new Vector2(rect.offsetMin.x, -rect.sizeDelta.y);
        rect.offsetMax = new Vector2(rect.offsetMax.x, 0);


    }
    //ʹ�б���ת���ײ�
    void ToBottomFunc()
    {

        /*rect.offsetMin = new Vector2(rect.offsetMin.x, 0);
        rect.offsetMax = new Vector2(rect.offsetMax.x, rect.sizeDelta.y);*/
    }
    void onClickFunc(myButton btn)
    {
        Debug.Log(btn.getFileName());
    }
    //����б�
    //ɾ��������ť���
}
