using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts;

/* LoadKoreoInfo： 从转换脚本中获得Koreo等资源引用，用于跨场景传参
 * 层级：TS
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

        // 临时使用(加载场景)
        public void StartPlayingScene(string scene)
        {
            SceneManager.LoadScene(scene);

            ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
            conveter.StartConvet();

            ReadInputField judge = beatmapMannager.GetComponent<ReadInputField>();//暂停背景音乐播放
            judge.BGM.Stop();
        }
        // 临时使用(退出游戏)
        public void Quit()
        {
            Application.Quit();
        }
    }
}
