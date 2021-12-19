using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts;

/* LoadKoreoInfo�� ��ת���ű��л��Koreo����Դ���ã����ڿ糡������
 * �㼶��TS
 */
namespace SonicBloom.Koreo.Demos
{
    public class LoadKoreoInfo : MonoBehaviour
    {
        public Koreography onLoadKoreo;
        public KoreographyTrack onLoadTrack;
        public string eventID;
        public float leadInTime;

        public TMP_Text beatmapNameUI;

        public GameObject beatmapMannager;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }

        // ��ʱʹ��(���س���)
        public void StartPlayingScene(string scene)
        {
            SceneManager.LoadScene(scene);

            ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
            conveter.StartConvet();

            ReadInputField judge = beatmapMannager.GetComponent<ReadInputField>();//��ͣ�������ֲ���
            judge.BGM.Stop();
        }
        // ��ʱʹ��(�˳���Ϸ)
        public void Quit()
        {
            Application.Quit();
        }
    }
}
