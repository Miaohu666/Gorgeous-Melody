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
    /* ConvetBeatmapOSUMania�� ��ȡһ��OSUMania��׼��osz�ļ���ѹ·����
     * �����������ļ�����Track��Koreo
     * [�㼶]��TS
     * [���ĺ���]������StartConvet()�Կ�ʼת����
     * [��Ҫ����]��sourceFilePath��sourceBeatmapName��sourceAudioName
     * ������ת��ǰָ������֤��ʽ��ȷ��
     */
    public class ConvetBeatmapOSUMania : MonoBehaviour
    {
        #region Fields
        // Ҫת������Koreo��Track����
        public Koreography targetKoreo;
        public KoreographyTrack targetTrack;
        // Ĭ��eventID
        public static string DEFAULT_EventID = "audio";
        // �洢�����ļ�����Ƶ���ļ���·����Windows��׼·����
        public string sourceFilePath;
        // �����ļ������ƣ�������׺��osu��
        public string sourceBeatmapName;
        // ��Ƶ�ļ������ƣ�����׺����Ĭ��Ϊaudio.mp3��
        public string sourceAudioName;
        // ��������Ĺ����
        public int columnCount = 4;

        // �������ݳ�Ա���ļ��ж�ȡ��
        // �洢���ص���Ƶ�ļ�
        private AudioClip sourceAudio;
        // ��Ƶ�ļ������ƣ�������׺������������eventID��
        private string sourceAudioFileName;
        // ��Ƶ����ǰ��׼��ʱ��
        private int audioLeadInTime;
        // ��Ƶ������
        private int SampleRate = 44100;

        #endregion

        #region Methods

        /*
         GeneEvent: ����һ��Track�ϵ��¼�
         @sp : �¼���ʼʱ��(ms)
         @ep ���¼�����ʱ��(ms)
         @ps : �¼��������ַ���
        */
        KoreographyEvent GeneEvent(int sp, int ep, string ps)
        {
            KoreographyEvent evt = new KoreographyEvent();

            // ���������λΪ���룬����ת��Ϊ������
            evt.StartSample = (int)(0.001f * sp * SampleRate);  // ���ÿ�ʼ����ʱ��
            evt.EndSample = (int)(0.001f * ep * SampleRate); // ���ý�������ʱ��
            TextPayload payload = new TextPayload(); // newһ���ַ���paylod
            payload.TextVal = ps; // ����payload�ַ���
            evt.Payload = payload;
            return evt;
        }

        /*
         AddEvents: ������¼�д�뵽һ��Track��
         @track : Ҫд���track
         @events ��Ҫд����¼��б�
        */
        void AddEvents(KoreographyTrack track, List<KoreographyEvent> events)
        {
            foreach (KoreographyEvent evt in events)
            {
                bool isAdded = track.AddEvent(evt);
                // ���ͬһʱ���ж���¼�д�룬Ϊ��ֹ��ͻ���󵽵��¼���ʼ�ͽ�������+1
                // ������Ӱ����Ϸ����
                while (!isAdded)
                {
                    evt.StartSample += 1;
                    evt.EndSample += 1;
                    isAdded = track.AddEvent(evt);
                }
            }
        }

        /*
         ModifyTargetKoreo: ����Ŀ��Koreo��Track
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

        /* IEnumerator GetAudioClip��ʹ��UnityWebRequest������Ƶ��Դ�����أ� 
         * @path : �����ļ����ڵ�·��
         */
        IEnumerator GetAudioClip(string path)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
            {
                // ��Ŀ�ĵ�ַ�������󣬴˴�Ϊ�������󣬲��ȴ��ظ�
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // ��www�л����Դ
                    sourceAudio = DownloadHandlerAudioClip.GetContent(www);
                    sourceAudio.name = sourceAudioFileName;
                    // �����Ƶ�Ĳ�����
                    SampleRate = sourceAudio.frequency;

                    // ����Ŀ��Koreo����Ƶ
                    targetKoreo.SourceClip = sourceAudio;
                    targetKoreo.SampleRate = SampleRate;

                    //Debug.Log(targetKoreo.SourceClipName);
                }
            }
        }


        /*
         * [������]��ͳResources.Load��ֻ��Ƕ��ʹ�ã����Ѵ����ز��ļ��е���Ƶ���ü��ص�Koreo��
        void loadAudioSource()
        {
            sourceAudio = Resources.Load<AudioClip>(sourceAudioFileName);
            // �����Ƶ�Ĳ�����
            SampleRate =  sourceAudio.frequency;
        }
        */
        /*
         ConvertBeatmap: ��OSU��ʽ������ת��Ϊ�����¼���KoreoTrack
         @beatmapRes : ���ջ��зָ���OSU��ʽnote�����ַ���
        */
        void ConvertBeatmap(List<string> beatmapRes)
        {
            // �������ʱtrack�������¼�
            targetTrack.RemoveAllEvents();

            targetTrack.EventID = DEFAULT_EventID;

            List<KoreographyEvent> events = new List<KoreographyEvent>();
            foreach (string note in beatmapRes)
            {
                /* ��ʽ������
                 0 x��λ�ã�1 y��λ�ã�2��ʼʱ�䣬3����(128�ǳ���)��4�����Ч��5����ʱ��:��������
                 192,192,19688,5,2,0:0:0:20:D4S.wav
                 448,192,19688,128,2,20913:0:0:0:20:LR_NBell_F#5.wav
                */

                // ��ø���λ�õ�����
                var data = note.Split(',');
                int posX = int.Parse(data[0]);
                int startTime = int.Parse(data[2]);
                int endTime = startTime;

                bool isHold = false;
                string payload = "";

                // �ж��Ƿ�Ϊ��������
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
                // �ж�note���ڵĹ��
                int trackNo = Mathf.FloorToInt(posX * columnCount / 512) + 1;
                // �����¼������ַ���
                payload += "," + trackNo.ToString();

                // �������������¼����������¼��б�
                events.Add(GeneEvent(startTime, endTime, payload));
            }
            // �����¼��б�����Track
            AddEvents(targetTrack, events);
        }

        /*
         BeatmapRead_local: ��ȡOSU��ʽ�����沢��ȡ��������
        */
        List<string> BeatmapRead_local()
        {
            // ��������ļ�·��
            string beatmapFilePath = Path.Combine(sourceFilePath, sourceBeatmapName );//ɾ��+ ".osu"
            Debug.Log("beatmapFilePath: " + beatmapFilePath);

            string line = "";
            List<string> result = new List<string>();

            // ���ļ���
            FileStream fs = new FileStream(beatmapFilePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);

            // ��ȡ"[HitObjects]"���ݿ�ǰ������Ƶ����Ϣ
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
            // ��ȡ"[HitObjects]"���ݿ��е�ÿһ�У�����ָ�����ַ����б�
            while ((line = sr.ReadLine()) != null)
            {
                result.Add(line);
            }
            return result;
        }

        /*
         IEnumerator LoadBeatmap: Э����������ת��������
        */
        IEnumerator LoadBeatmap()
        {
            // ��������ļ�·����URI
            string cwd = System.Environment.CurrentDirectory;
            Debug.Log("[Path]: " + Path.Combine(cwd, sourceFilePath, sourceAudioName));
            string path = new Uri(Path.Combine(cwd, sourceFilePath, sourceAudioName)).AbsoluteUri;
            Debug.Log("[LoadAudioFilePath]: " + path);

            // ��ȡosu maina�����ļ�����
            List<string> beatmaplines = BeatmapRead_local();

            // �ȴ���Ƶ�������
            yield return StartCoroutine(GetAudioClip(path));
            // Debug.Log("[Audio Load Compeleted]");

            // ������ת����Track��
            ConvertBeatmap(beatmaplines);

            // ��Track���뵽Koreo��
            ModifyTargetKoreo();
            // Debug.Log("[Koreo Load Compeleted]");
        }

        /*
         StartConvet: ������ں���������ת��
        */
        public void StartConvet()
        {
            if (sourceFilePath == "" || sourceBeatmapName == "" || sourceAudioName == "")
            {
                Debug.Log("ERROR��There is INCOMPLETE path or name. Please Check Input!");
                return;
            }
            print(sourceFilePath+ sourceBeatmapName+ sourceAudioName);
            StartCoroutine(LoadBeatmap());

            // ��ת������������ô���onLoadObject
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
