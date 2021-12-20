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
            //item = Instantiate(mItemPrefab_);

            //item.transform.parent = mContentTransform_;

            item = Instantiate(mItemPrefab_, mContentTransform_);
            item.GetComponentInChildren<TMP_Text>().text = filename_;

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
            // ��Ŀ�ĵ�ַ�������󣬴˴�Ϊ�������󣬲��ȴ��ظ�
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                print("1");
                Debug.Log(www.error);
            }
            else
            {
                ReadInputField judge = beatmapMannager.GetComponent<ReadInputField>();
                // ��www�л����Դ
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

        int count = 0;
        foreach (DirectoryInfo f in root.GetDirectories())
        {
            myButton btn = new myButton(f.Name, mItemPrefab, mContentTransform);
            lists.Add(btn);
            count++;
            //��ÿ����ť�����������¼�
            btn.item.onClick.AddListener(
                () =>
                {
                    ConvetBeatmapOSUMania conveter = beatmapMannager.GetComponent<ConvetBeatmapOSUMania>();//�趨�����ļ������ڵ�·��
                    string temp = path + "/" + btn.getFileName();
                    conveter.sourceFilePath = temp;

                    ReadInputField judge = beatmapMannager.GetComponent<ReadInputField>();
                    judge.isSelected = false;
                    judge.isReady();


                    foreach (FileInfo f in new DirectoryInfo(temp).GetFiles())
                    {
                        // wht������jpg��pngͼƬ
                        if(f.Name.EndsWith(".jpg") || f.Name.EndsWith(".png"))
                        {
                            Texture2D img = null;
                            WWW www = new WWW("file://" + f.FullName);

                            print("file://" + f.FullName);

                            img = www.texture;
                            Sprite sprite = Sprite.Create(img, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f));
                            judge.image.sprite = sprite;

                        }

                    }

                    selectBeatmap beatmapselector = beatmapSelector.GetComponent<selectBeatmap>();
                    beatmapselector.clearAllButton();//���ԭ�еİ�ť
                    
                    string audio_filename =  beatmapselector.selectBeatMap(temp);//��ȡ�����ѡ��������µ����������ļ�
                    
                    judge.audioIF.text = btn.getFileName();

                    Debug.Log("AudioFileName: " + audio_filename);
                    StartCoroutine(GetAudioClip("file://" + temp + "/" + audio_filename));//����ѡ�е�����
                }
            );
        }
        mContentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(500, count * 100);
    }
    void Update()
    {
    }

}
