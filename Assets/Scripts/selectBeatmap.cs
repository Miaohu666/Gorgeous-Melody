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

    // wht���޸ģ����ڻ�ȡ����ͬһ��Ŀ�µ�����ʱ�᷵����Ƶ�ļ������ơ�
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

            //��ÿ����ť�����������¼�
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
        mContentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(400, count * 120);//��̬�����б����ݸ߶�
        return source_audio_filename;
    }
    void Update()
    {
    }
}
