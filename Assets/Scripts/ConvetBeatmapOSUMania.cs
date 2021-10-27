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
        private int columnCount = 2;
        private int SampleRate = 44100;

        #endregion

        #region Methods
        KoreographyEvent geneEvent(int sp, int ep, string ps)
        {
            
            KoreographyEvent evt = new KoreographyEvent();

            // TODO�����������λΪ���룬����ת��Ϊ������
            evt.StartSample = (int)(0.001f * sp * SampleRate);  // ���ÿ�ʼ����ʱ��
            evt.EndSample = (int)(0.001f * ep * SampleRate); // ���ý�������ʱ��
            TextPayload payload  = new TextPayload(); // newһ���ַ���paylod
            payload.TextVal = ps; // ����payload�ַ���
            evt.Payload = payload;
            return evt;
        }

        void addEvents(KoreographyTrack track, List<KoreographyEvent> events)
        {
            foreach(KoreographyEvent evt in events)
            {
                track.AddEvent(evt);
            }
        }
        void modifyTargetKoreo()
        {
            targetKoreo.SourceClip = sourceAudio;
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

        /* ���������򱾵ذ�ʹ�ã����첽���أ����������ڴ��У����ص���Ƶ��Դ���ò��ܱ��浽Koreo��
         * ��Ҫʹ�� AssetBundle�����뱣֤������Ϸ����������ǰ��ɶ�Koreo������
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

        // ��ͳResources.Load��ֻ�ܱ���ʹ�ã���ֻ������һ�Σ����ɽ��Ѵ��ڵ���Ƶ���ü��ص�Koreo��
        void loadAudioSource()
        {
            sourceAudio = Resources.Load<AudioClip>(sourceAudioFileName);
        }

        void convertBeatmap(List<string> beatmapRes)
        {
            // �������ʱtrack�������¼�
            targetTrack.RemoveAllEvents();
            targetTrack.EventID = sourceAudioFileName;

            List<KoreographyEvent> events = new List<KoreographyEvent>();
            foreach (string note in beatmapRes)
            {
                // ��ʽ������
                // 0x��λ�ã�1y��λ�ã�2��ʼʱ�䣬3����(128�ǳ���)��4�����Ч��5����ʱ��:��������
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
                if (Mathf.Floor(posX * columnCount / 512) == 0)
                {
                    payload += ",1";
                }
                else
                {
                    payload += ",2";
                }
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
            convertBeatmap(beatmaplines);

            Debug.Log(targetKoreo.SourceClipName);

            loadAudioSource();
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
