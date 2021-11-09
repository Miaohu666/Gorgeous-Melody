using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



namespace SonicBloom.Koreo.Demos
{
    public class ConvetBeatmapOSUMania : MonoBehaviour
    {
        #region Fields
        public Koreography targetKoreo;
        public KoreographyTrack targetTrack;
        public string sourceBeatmapName;
        private AudioClip sourceAudio;
        private string sourceAudioFileName;
        private int audioLeadInTime;
        public int columnCount = 4;
        private int SampleRate = 44100;

        #endregion

        #region Methods
        KoreographyEvent geneEvent(int sp, int ep, string ps)
        {
            
            KoreographyEvent evt = new KoreographyEvent();

            // TODO：传入参数单位为毫秒，将其转换为采样率
            evt.StartSample = (int)(0.001f * sp * SampleRate);  // 设置开始采样时间
            evt.EndSample = (int)(0.001f * ep * SampleRate); // 设置结束采样时间
            TextPayload payload  = new TextPayload(); // new一个字符串paylod
            payload.TextVal = ps; // 设置payload字符串
            evt.Payload = payload;
            return evt;
        }

        void addEvents(KoreographyTrack track, List<KoreographyEvent> events)
        {
            foreach(KoreographyEvent evt in events)
            {
                bool isAdded = track.AddEvent(evt);
                while (!isAdded)
                {
                    evt.StartSample += 1;
                    evt.EndSample += 1;
                    isAdded = track.AddEvent(evt);
                }
            }
        }
        void modifyTargetKoreo()
        {
            targetKoreo.SourceClip = sourceAudio;
            targetKoreo.SampleRate = SampleRate;
            if (targetKoreo.GetNumTracks() > 0)
            {
                int numOfTracks = targetKoreo.GetNumTracks();
                for(int i = 0; i< numOfTracks; i++)
                {
                    targetKoreo.RemoveTrack(targetKoreo.GetTrackAtIndex(i));
                }
                
            }
            targetKoreo.AddTrack(targetTrack);
        }

        /* 服务器包或本地包使用，可异步加载，但缓存在内存中，加载的音频资源引用不能保存到Koreo中
         * 若要使用 AssetBundle，必须保证其在游戏主场景加载前完成对Koreo的设置
        void loadAudioSource1()
        {
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "AssetBundles/OSUmania"));
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            var audio = myLoadedAssetBundle.LoadAsset<AudioClip>(sourceAudioFileName);
            sourceAudio = audio;
        }
        */

        // 传统Resources.Load，只能本地使用，但只需运行一次，即可将已存在的音频引用加载到Koreo中
        void loadAudioSource()
        {
            sourceAudio = Resources.Load<AudioClip>(sourceAudioFileName);
            // 获得音频的采样率
            SampleRate =  sourceAudio.frequency;
        }

        void convertBeatmap(List<string> beatmapRes)
        {
            // 先清除临时track的所有事件
            targetTrack.RemoveAllEvents();
            targetTrack.EventID = sourceAudioFileName;

            List<KoreographyEvent> events = new List<KoreographyEvent>();
            foreach (string note in beatmapRes)
            {
                // 格式举例：
                // 0x轴位置，1y轴位置，2起始时间，3类型(128是长条)，4打击音效，5结束时间:其他属性
                // 192,192,19688,5,2,0:0:0:20:D4S.wav
                // 448,192,19688,128,2,20913:0:0:0:20:LR_NBell_F#5.wav
                var data = note.Split(',');
                int posX = int.Parse(data[0]);
                int startTime = int.Parse(data[2]);
                int endTime = startTime;
                bool isHold = false;
                string payload = "";
                if (data[3].Equals("128")){
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
                int trackNo = Mathf.FloorToInt(posX * columnCount / 512) + 1;
                payload += ("," + trackNo.ToString());

                // Debug.Log("s: "+startTime+ " e: "+endTime + " payload: "+payload);
                events.Add(geneEvent(startTime, endTime, payload));
            }
            addEvents(targetTrack, events);
        }

        List<string> beatmapRead_local()
        {
            string dir = Directory.GetCurrentDirectory();
            string beatmapFilePath = dir + "\\Assets\\OSUmaps\\" + sourceBeatmapName + ".osu";
            Debug.Log(beatmapFilePath);
            string line = "";
            List<string> result = new List<string>();
            FileStream fs = new FileStream(beatmapFilePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            while ( !(line = sr.ReadLine()).Equals("[HitObjects]") )
            {
                if (line.Split(':')[0].Equals("AudioFilename"))
                {
                    sourceAudioFileName = line.Split(':')[1].Substring(1).Split('.')[0];
                    Debug.Log("sourceAudioFileName: " + sourceAudioFileName);
                }
                if (line.Split(':')[0].Equals("AudioLeadIn"))
                {
                    int audioLeadInTimeInMS = int.Parse(line.Split(':')[1].Substring(1));
                    audioLeadInTime = (int)(audioLeadInTimeInMS * 0.001f);
                    Debug.Log("audioLeadInTime: " + audioLeadInTime);
                }
            }
            while ( (line = sr.ReadLine()) != null)
            {
                result.Add(line); 
            }
            return result;
        }
        void Start()
        {

            List<string> beatmaplines = beatmapRead_local();
            // 因为加载音频时获取了采样率，所以必须先加载音频
            loadAudioSource();
            
            convertBeatmap(beatmaplines);

            Debug.Log(targetKoreo.SourceClipName);

            
            modifyTargetKoreo();

            LoadKoreoInfo onLoadObject = GameObject.FindObjectOfType<LoadKoreoInfo>();

            onLoadObject.onLoadKoreo = targetKoreo;
            onLoadObject.onLoadTrack = targetTrack;
            onLoadObject.eventID = targetTrack.EventID;
            onLoadObject.leadInTime = audioLeadInTime;
            onLoadObject.beatmapNameUI.text = sourceBeatmapName;

        }


        void Update()
        {

        }
        #endregion
    }
}
