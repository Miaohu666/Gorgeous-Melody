using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts;
using UnityEngine.UI;

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

        public GameObject beatmapManager;

        public bool is_background_video;
        public Sprite bgPicSprite;


        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }

        // Obsoleted
        public void StartPlayingScene(string scene)
        {

            /* ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
            conveter.StartConvet();

            ReadInputField judge = beatmapManager.GetComponent<ReadInputField>();//��ͣ�������ֲ���
            judge.BGM.Stop();

            SceneManager.LoadScene(scene);*/

        }
        // ��ʱʹ��(�˳���Ϸ)
        public void Quit()
        {
            Application.Quit();
        }
    }
}
