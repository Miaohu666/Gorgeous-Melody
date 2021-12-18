using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeController : MonoBehaviour
{
    // �洢��Ҫ���ŵ���Ч
    public AudioClip strikeAudio;
    // ����AudioClip�ı������
    private AudioSource strikeSource;

    // Start is called before the first frame update
    void Start()
    {
        strikeSource = GetComponent<AudioSource>();
    }

    void playStrikeAudio()
    {
        // ���Ŵ����Ч
        strikeSource.PlayOneShot(strikeAudio, 1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
