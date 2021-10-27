using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace SonicBloom.Koreo.Demos
{
    public class LoadKoreoInfo : MonoBehaviour
    {
        public Koreography onLoadKoreo;
        public KoreographyTrack onLoadTrack;
        public string eventID;
        public float leadInTime;

        public TMP_Text beatmapNameUI;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }

        // ¡Ÿ ± π”√
        public void StartPlayingScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
