using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class mvController : MonoBehaviour
{
    //设置VideoPlayer、RawImage和当前播放视频索引参数
    private VideoPlayer videoPlayer;
    private RawImage rawImage;

    /*private int currentClipIndex;
    //设置相关文本和按钮参数以及视频列表
    public Text text_PlayOrPause;
    public Button button_PlayOrPause;
    public Button button_Pre;
    public Button button_Next;
    public VideoClip[] videoClips;*/

    void Start()
    {
        //获取VideoPlayer和RawImage组件，以及初始化当前视频索引
        videoPlayer = this.GetComponent<VideoPlayer>();
        rawImage = this.GetComponent<RawImage>();
        // currentClipIndex = 0;
        /*//设置相关按钮监听事件
        button_PlayOrPause.onClick.AddListener(OnPlayOrPauseVideo);
        button_Pre.onClick.AddListener(OnPreVideo);
        button_Next.onClick.AddListener(OnNextVideo);*/
    }

    // Update is called once per frame
    void Update()
    {
        //没有视频则返回，不播放
        if (videoPlayer.texture == null)
        {
            return;
        }
        //渲染视频到UGUI上
        rawImage.texture = videoPlayer.texture;
    }

    /// <summary>
    /// 播放和暂停当前视频
    /// </summary>
    /*private void OnPlayOrPauseVideo()
    {
        //判断视频播放情况，播放则暂停，暂停就播放，并更新相关文本
        if (videoPlayer.isPlaying == true)
        {
            videoPlayer.Pause();
            text_PlayOrPause.text = "播放";
        }
        else
        {
            videoPlayer.Play();
            text_PlayOrPause.text = "暂停";
        }
    }*/

    /// <summary>
    /// 切换上一个视频
    /// </summary>
/*    private void OnPreVideo()
    {
        //视频列表减一播放上一个视频，并进行避免越界操作
        currentClipIndex -= 1;
        if (currentClipIndex < 0)
        {
            currentClipIndex = videoClips.Length - 1;
        }
        videoPlayer.clip = videoClips[currentClipIndex];
        text_PlayOrPause.text = "暂停";
    }*/

    /// <summary>
    /// 切换下一个视频
    /// </summary>
/*    private void OnNextVideo()
    {
        //视频列表加一播放下一个视频，并进行避免越界操作
        currentClipIndex += 1;
        currentClipIndex = currentClipIndex % videoClips.Length;
        videoPlayer.clip = videoClips[currentClipIndex];
        text_PlayOrPause.text = "暂停";
    }
}*/
}
