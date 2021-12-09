using SonicBloom.Koreo;
using SonicBloom.Koreo.Demos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    /* ConvetBeatmapOSUMania： 读取一个OSUMania标准的osz文件解压路径，
     * 并根据其中文件生成Track和Koreo
     * [层级]：TS
     * [核心函数]：调用StartConvet()以开始转换。
     * [必要参数]：sourceFilePath、sourceBeatmapName、sourceAudioName
     * 必须在转换前指定并保证格式正确。
     */
    public class ConvetBeatmapOSUMania : MonoBehaviour
    {
        #region Fields
        // 要转换到的Koreo和Track容器
        public Koreography targetKoreo;
        public KoreographyTrack targetTrack;
        // 默认eventID
        public static string DEFAULT_EventID = "audio";
        // 存储铺面文件和音频的文件夹路径（Windows标准路径）
        public string sourceFilePath;
        // 铺面文件的名称（不带后缀名osu）
        public string sourceBeatmapName;
        // 音频文件的名称（带后缀名，默认为audio.mp3）
        public string sourceAudioName;
        // 加载铺面的轨道数
        public int columnCount = 4;

        // 以下数据成员从文件中读取：
        // 存储加载的音频文件
        private AudioClip sourceAudio;
        // 音频文件的名称（不带后缀名，用于生成eventID）
        private string sourceAudioFileName;
        // 音频播放前的准备时间
        private int audioLeadInTime;
        // 音频采样率
        private int SampleRate = 44100;

        #endregion

        #region Methods

        /*
         GeneEvent: 生成一个Track上的事件
         @sp : 事件开始时间(ms)
         @ep ：事件结束时间(ms)
         @ps : 事件的属性字符串
        */
        KoreographyEvent GeneEvent(int sp, int ep, string ps)
        {
            KoreographyEvent evt = new KoreographyEvent();

            // 传入参数单位为毫秒，将其转换为采样率
            evt.StartSample = (int)(0.001f * sp * SampleRate);  // 设置开始采样时间
            evt.EndSample = (int)(0.001f * ep * SampleRate); // 设置结束采样时间
            TextPayload payload = new TextPayload(); // new一个字符串paylod
            payload.TextVal = ps; // 设置payload字符串
            evt.Payload = payload;
            return evt;
        }

        /*
         AddEvents: 将多个事件写入到一个Track中
         @track : 要写入的track
         @events ：要写入的事件列表
        */
        void AddEvents(KoreographyTrack track, List<KoreographyEvent> events)
        {
            foreach (KoreographyEvent evt in events)
            {
                bool isAdded = track.AddEvent(evt);
                // 如果同一时间有多个事件写入，为防止冲突，后到的事件起始和结束采样+1
                // 几乎不影响游戏体验
                while (!isAdded)
                {
                    evt.StartSample += 1;
                    evt.EndSample += 1;
                    isAdded = track.AddEvent(evt);
                }
            }
        }

        /*
         ModifyTargetKoreo: 设置目标Koreo的Track
        */
        void ModifyTargetKoreo()
        {
            if (targetKoreo.GetNumTracks() > 0)
            {
                int numOfTracks = targetKoreo.GetNumTracks();
                for (int i = 0; i < numOfTracks; i++)
                {
                    targetKoreo.RemoveTrack(targetKoreo.GetTrackAtIndex(i));
                }

            }
            targetKoreo.AddTrack(targetTrack);
        }

        /* IEnumerator GetAudioClip：使用UnityWebRequest导入音频资源（本地） 
         * @path : 导入文件所在的路径
         */
        IEnumerator GetAudioClip(string path)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
            {
                // 向目的地址发送请求，此处为本地请求，并等待回复
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // 从www中获得资源
                    sourceAudio = DownloadHandlerAudioClip.GetContent(www);
                    sourceAudio.name = sourceAudioFileName;
                    // 获得音频的采样率
                    SampleRate = sourceAudio.frequency;

                    // 设置目标Koreo的音频
                    targetKoreo.SourceClip = sourceAudio;
                    targetKoreo.SampleRate = SampleRate;

                    //Debug.Log(targetKoreo.SourceClipName);
                }
            }
        }


        /*
         * [已弃用]传统Resources.Load，只能嵌入使用，将已存在素材文件夹的音频引用加载到Koreo中
        void loadAudioSource()
        {
            sourceAudio = Resources.Load<AudioClip>(sourceAudioFileName);
            // 获得音频的采样率
            SampleRate =  sourceAudio.frequency;
        }
        */
        /*
         ConvertBeatmap: 将OSU格式的铺面转化为含义事件的KoreoTrack
         @beatmapRes : 按照换行分隔的OSU格式note定义字符串
        */
        void ConvertBeatmap(List<string> beatmapRes)
        {
            // 先清除临时track的所有事件
            targetTrack.RemoveAllEvents();

            targetTrack.EventID = DEFAULT_EventID;

            List<KoreographyEvent> events = new List<KoreographyEvent>();
            foreach (string note in beatmapRes)
            {
                /* 格式举例：
                 0 x轴位置，1 y轴位置，2起始时间，3类型(128是长条)，4打击音效，5结束时间:其他属性
                 192,192,19688,5,2,0:0:0:20:D4S.wav
                 448,192,19688,128,2,20913:0:0:0:20:LR_NBell_F#5.wav
                */

                // 获得各个位置的数据
                var data = note.Split(',');
                int posX = int.Parse(data[0]);
                int startTime = int.Parse(data[2]);
                int endTime = startTime;

                bool isHold = false;
                string payload = "";

                // 判断是否为长按类型
                if (data[3].Equals("128"))
                {
                    isHold = true;
                }
                if (isHold)
                {
                    endTime = int.Parse(data[5].Split(':')[0]);
                    payload += "H";
                }
                else
                {
                    payload += "C";
                }
                // 判断note所在的轨道
                int trackNo = Mathf.FloorToInt(posX * columnCount / 512) + 1;
                // 生成事件属性字符串
                payload += "," + trackNo.ToString();

                // 根据数据生成事件，并加入事件列表
                events.Add(GeneEvent(startTime, endTime, payload));
            }
            // 根据事件列表生成Track
            AddEvents(targetTrack, events);
        }

        /*
         BeatmapRead_local: 读取OSU格式的铺面并提取核心数据
        */
        List<string> BeatmapRead_local()
        {
            // 获得铺面文件路径
            string beatmapFilePath = Path.Combine(sourceFilePath, sourceBeatmapName + ".osu");
            // Debug.Log("beatmapFilePath: " + beatmapFilePath);

            string line = "";
            List<string> result = new List<string>();

            // 打开文件流
            FileStream fs = new FileStream(beatmapFilePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);

            // 读取"[HitObjects]"数据块前关于音频的信息
            while (!(line = sr.ReadLine()).Equals("[HitObjects]"))
            {
                if (line.Split(':')[0].Equals("AudioFilename"))
                {
                    sourceAudioFileName = line.Split(':')[1].Substring(1).Split('.')[0];
                    // Debug.Log("sourceAudioFileName: " + sourceAudioFileName);
                }
                if (line.Split(':')[0].Equals("AudioLeadIn"))
                {
                    int audioLeadInTimeInMS = int.Parse(line.Split(':')[1].Substring(1));
                    audioLeadInTime = (int)(audioLeadInTimeInMS * 0.001f);
                    // Debug.Log("audioLeadInTime: " + audioLeadInTime);
                }
            }
            // 读取"[HitObjects]"数据块中的每一行，将其分割加入字符串列表
            while ((line = sr.ReadLine()) != null)
            {
                result.Add(line);
            }
            return result;
        }

        /*
         IEnumerator LoadBeatmap: 协调整个铺面转换的流程
        */
        IEnumerator LoadBeatmap()
        {
            // 获得输入文件路径的URI
            string cwd = System.Environment.CurrentDirectory;
            Debug.Log("[Path]: " + Path.Combine(cwd, sourceFilePath, sourceAudioName));
            string path = new Uri(Path.Combine(cwd, sourceFilePath, sourceAudioName)).AbsoluteUri;
            Debug.Log("[LoadAudioFilePath]: " + path);

            // 读取osu maina铺面文件数据
            List<string> beatmaplines = BeatmapRead_local();

            // 等待音频加载完毕
            yield return StartCoroutine(GetAudioClip(path));
            // Debug.Log("[Audio Load Compeleted]");

            // 将铺面转换到Track中
            ConvertBeatmap(beatmaplines);

            // 将Track载入到Koreo中
            ModifyTargetKoreo();
            // Debug.Log("[Koreo Load Compeleted]");
        }

        /*
         StartConvet: 功能入口函数，开启转换
        */
        public void StartConvet()
        {
            if (sourceFilePath == "" || sourceBeatmapName == "" || sourceAudioName == "")
            {
                Debug.Log("ERROR：There is INCOMPLETE path or name. Please Check Input!");
                return;
            }

            StartCoroutine(LoadBeatmap());

            // 将转换后的物体引用传给onLoadObject
            LoadKoreoInfo onLoadObject = FindObjectOfType<LoadKoreoInfo>();

            onLoadObject.onLoadKoreo = targetKoreo;
            onLoadObject.onLoadTrack = targetTrack;
            onLoadObject.eventID = targetTrack.EventID;
            onLoadObject.leadInTime = audioLeadInTime;
            onLoadObject.beatmapNameUI.text = sourceBeatmapName;
        }

        void Start()
        {

        }

        void Update()
        {

        }
        #endregion
    }
}
