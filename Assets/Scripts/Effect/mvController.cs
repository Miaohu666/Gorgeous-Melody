using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class mvController : MonoBehaviour
{
    //����VideoPlayer��RawImage�͵�ǰ������Ƶ��������
    private VideoPlayer videoPlayer;
    private RawImage rawImage;

    /*private int currentClipIndex;
    //��������ı��Ͱ�ť�����Լ���Ƶ�б�
    public Text text_PlayOrPause;
    public Button button_PlayOrPause;
    public Button button_Pre;
    public Button button_Next;
    public VideoClip[] videoClips;*/

    void Start()
    {
        //��ȡVideoPlayer��RawImage������Լ���ʼ����ǰ��Ƶ����
        videoPlayer = this.GetComponent<VideoPlayer>();
        rawImage = this.GetComponent<RawImage>();
        // currentClipIndex = 0;
        /*//������ذ�ť�����¼�
        button_PlayOrPause.onClick.AddListener(OnPlayOrPauseVideo);
        button_Pre.onClick.AddListener(OnPreVideo);
        button_Next.onClick.AddListener(OnNextVideo);*/
    }

    // Update is called once per frame
    void Update()
    {
        //û����Ƶ�򷵻أ�������
        if (videoPlayer.texture == null)
        {
            return;
        }
        //��Ⱦ��Ƶ��UGUI��
        rawImage.texture = videoPlayer.texture;
    }

    /// <summary>
    /// ���ź���ͣ��ǰ��Ƶ
    /// </summary>
    /*private void OnPlayOrPauseVideo()
    {
        //�ж���Ƶ�����������������ͣ����ͣ�Ͳ��ţ�����������ı�
        if (videoPlayer.isPlaying == true)
        {
            videoPlayer.Pause();
            text_PlayOrPause.text = "����";
        }
        else
        {
            videoPlayer.Play();
            text_PlayOrPause.text = "��ͣ";
        }
    }*/

    /// <summary>
    /// �л���һ����Ƶ
    /// </summary>
/*    private void OnPreVideo()
    {
        //��Ƶ�б��һ������һ����Ƶ�������б���Խ�����
        currentClipIndex -= 1;
        if (currentClipIndex < 0)
        {
            currentClipIndex = videoClips.Length - 1;
        }
        videoPlayer.clip = videoClips[currentClipIndex];
        text_PlayOrPause.text = "��ͣ";
    }*/

    /// <summary>
    /// �л���һ����Ƶ
    /// </summary>
/*    private void OnNextVideo()
    {
        //��Ƶ�б��һ������һ����Ƶ�������б���Խ�����
        currentClipIndex += 1;
        currentClipIndex = currentClipIndex % videoClips.Length;
        videoPlayer.clip = videoClips[currentClipIndex];
        text_PlayOrPause.text = "��ͣ";
    }
}*/
}
