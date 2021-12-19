using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeController : MonoBehaviour
{
    // 存储需要播放的音效
    public AudioClip strikeAudio;
    // 调用AudioClip的必须组件
    private AudioSource strikeSource;

    // Start is called before the first frame update
    void Start()
    {
        strikeSource = GetComponent<AudioSource>();
    }

    void playStrikeAudio()
    {
        // 播放打击音效
        strikeSource.PlayOneShot(strikeAudio, 1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
