//----------------------------------------------
//     代码由Koreographer插件官方脚本改写  
// RhythmGameController：控制一局游戏的主要流程
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using SonicBloom.Koreo.Players;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace SonicBloom.Koreo.Demos
{
	//将该脚本的内容加入Unity编辑器的菜单界面中
	[AddComponentMenu("Koreographer/Demos/Rhythm Game/Rhythm Game Controller")]
	public class RhythmGameController : MonoBehaviour
	{
		#region Fields 

		[Tooltip("标记该播放是否在设置菜单模式下，默认否")]
		public bool isSettingMode = false;

		[Tooltip("标记该播放是否在自动模式下，默认否")]
		public bool isAutoMode = false;

		[Tooltip("音乐是否结束，此标记用于场景切换（切换至计分板）")]
		public bool isEnd = false;

		[Tooltip("要生成音符的事件对应的Event ID")]
		[EventID]
		public string eventID;

		[Tooltip("判定Hit击中的延时范围（包括提前和延后）单位ms")]
		[Range(8f, 150f)]
		public float hitWindowRangeInMS = 80;

		[Tooltip("各判定等级的标准")]
		[Range(0f, 1f)]
		public float firstClassFactor = 0.2f;
		[Range(0f, 1f)]
		public float secondClassFactor = 0.5f;
		[Range(0f, 1f)]
		public float thirdClassFactor = 0.8f;

		[Tooltip("音符下落速度")]
		public float noteSpeed = 8f;

		[Tooltip("生成音符物体的原型件脚本（可以是prefab）")]
		public NoteObject noteObjectArchetype;

		[Tooltip("收纳所有轨道的父物体")]
		public GameObject LaneParent;

		[Tooltip("轨道列表（传入脚本引用）")]
		public List<LaneController> noteLanes = new List<LaneController>();

		[Tooltip("设置音乐播放前的准备时间（设置为负数可以跳过音乐的开头），单位s")]
		// 准备时间：设定好的在音乐开始前的延迟时间
		public float leadInTime;

		[Tooltip("音乐源，注意关闭其'Auto Play On Awake'选项")]
		public AudioSource audioCom;

		[Tooltip("背景图片组件")]
		public SpriteRenderer bgPic;

		[Tooltip("背景视频组件")]
		public GameObject bgVideoQuad;

		// The amount of leadInTime left before the audio is audible.
		// 剩余准备时间
		float leadInTimeLeft;

		// The amount of time left before we should play the audio (handles Event Delay).
		// 音乐播放倒计时 = 剩余准备时间 - 轨道时间延迟的触发时间
		float timeLeftToPlay;

		//Koreographer组件的本地引用.
		Koreography playingKoreo;

		// 使用采样作为单位的判定范围窗口值
		int hitWindowRangeInSamples;    // 此为私有成员，使用getset实现公共访问

		// 音符物件池（用于存储和记录音符物件的生成和销毁，使用栈实现）
		Stack<NoteObject> noteObjectPool = new Stack<NoteObject>();

		int combo;
		int totalScore;
		int count_maxCombo;
		int count_1stJudge;
		int count_2ndJudge;
		int count_3rdJudge;
		int count_4thJudge;
		int count_missedJudge;

		#endregion
		#region Properties

		// 判定窗口信息
		public float FirstLevelWindowSampleWidth {
			get
			{
				return hitWindowRangeInSamples * firstClassFactor;
			}
		}
		public float SecondLevelWindowSampleWidth
		{
			get
			{
				return hitWindowRangeInSamples * secondClassFactor;
			}
		}
		public float ThridLevelWindowSampleWidth
		{
			get
			{
				return hitWindowRangeInSamples * thirdClassFactor;
			}
		}

		// Public access to the hit window.
		public int HitWindowSampleWidth
		{
			get
			{
				return hitWindowRangeInSamples;
			}
		}

		// 获得判定窗口的宽度，以时间秒为单位
		public float WindowSizeInUnits
		{
			get
			{
				return noteSpeed * (hitWindowRangeInMS * 0.001f);
			}
		}

		// 获得当前音轨的采样率
		public int SampleRate
		{
			get
			{
				return playingKoreo.SampleRate;
			}
		}

		// 获得以采样为单位的当前时间，包括可能存在的延迟时间
		public int DelayedSampleTime
		{
			get
			{
				// Offset the time reported by Koreographer by a possible leadInTime amount.
				return playingKoreo.GetLatestSampleTime() - (int)(audioCom.pitch * leadInTimeLeft * SampleRate);
			}
		}

		// 得分信息的获取
		public int Combo { get => combo; set => combo = value; }
		public int TotalScore { get => totalScore; set => totalScore = value; }
		public int Count_maxCombo { get => count_maxCombo; set => count_maxCombo = value; }
		public int Count_1stJudge { get => count_1stJudge; set => count_1stJudge = value; }
		public int Count_2ndJudge { get => count_2ndJudge; set => count_2ndJudge = value; }
		public int Count_3rdJudge { get => count_3rdJudge; set => count_3rdJudge = value; }
		public int Count_4thJudge { get => count_4thJudge; set => count_4thJudge = value; }
		public int Count_missedJudge { get => count_missedJudge; set => count_missedJudge = value; }


		#endregion
		#region Methods
		void Awake()
		{
			if (!isSettingMode)
			{
				// 通过上一场景传入的koreo参数，初始化游戏场景的koreo配置
				LoadKoreoInfo onLoadObject = GameObject.FindObjectOfType<LoadKoreoInfo>();
				SimpleMusicPlayer musicPlayer = audioCom.GetComponent<SimpleMusicPlayer>();
				eventID = onLoadObject.eventID;
				leadInTime = onLoadObject.leadInTime + 4.0f;
				musicPlayer.LoadSong(onLoadObject.onLoadKoreo, autoPlay: false);

				// 判断背景播放视频还是图片
				if (!onLoadObject.is_background_video)
                {
					bgPic.sprite = onLoadObject.bgPicSprite;
					bgVideoQuad.SetActive(false);
				}
                else
                {
					bgVideoQuad.SetActive(true);
				}

				// 是否开启自动模式
				isAutoMode = onLoadObject.is_auto_mode;
				
			}
			

		}
		void Start()
		{

			// 从持久数据中获取延迟设置值、速度值
			// 【PS】：持久数据不能在移动平台使用
			Koreographer.Instance.EventDelayInSeconds = PlayerPrefs.GetInt("DelayInMS", 0) * 0.001f;
			if(!isSettingMode)
				noteSpeed = PlayerPrefs.GetFloat("NoteSpeed", 8.0f) * 2f;

			// 初始化准备时间
			InitializeLeadIn();


			// 初始化所有轨道
			for (int i = 0; i < noteLanes.Count; ++i)
			{
				noteLanes[i].Initialize(this);
			}

			// 初始化事件0
			playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);

			// 按照Koreo音轨（按照eventID确定一个音轨）上定义的音符事件顺序，获取所有事件
			KoreographyTrack rhythmTrack = playingKoreo.GetTrackByID(eventID);
			List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents();

			for (int i = 0; i < rawEvents.Count; ++i)
			{
				KoreographyEvent evt = rawEvents[i];
				// 获得事件对应音符的payload载荷值
				string payload = evt.GetTextValue();

				// 根据payload值判断音符应该下落的轨道
				for (int j = 0; j < noteLanes.Count; ++j)
				{
					LaneController lane = noteLanes[j];
					if (lane.DoesMatchPayload(payload))
					{
						// 找到对应的轨道后，将音符事件加入轨道的判定列表中
						lane.AddEventToLane(evt);

						// break会导致每个事件只会被放入到第一个满足payload标签的轨道上
						// 这样就无法实现双押操作了，因此我们注释掉break
						// break;
					}
				}
			}
		}

		// 初始化准备时间  
		// 如果准备时间为0，音频将立刻开始播放
		void InitializeLeadIn()
		{
			// Initialize the lead-in-time only if one is specified.
			if (leadInTime > 0f)
			{
				// 如果准备时间设置大于0，直接写入私有变量
				leadInTimeLeft = leadInTime;
				timeLeftToPlay = leadInTime - Koreographer.Instance.EventDelayInSeconds;
			}
			else
			{
				// 如果准备时间小于等于0，取其相反数并让音乐立刻从此处开始播放（跳过update里的倒计时操作）
				audioCom.time = -leadInTime;
				audioCom.Play();
			}
		}

		void Update()
		{
			// This should be done in Start().  We do it here to allow for testing with Inspector modifications.
			// 此函数只需在start中调用一次，这里调用是为了游戏过程中可以随时修改面板参数应用设置更改
			UpdateInternalValues();

			// 准备时间减少（准备时间 -= 渲染本帧已使用的时间）
			if (leadInTimeLeft > 0f)
			{
				leadInTimeLeft = Mathf.Max(leadInTimeLeft - Time.unscaledDeltaTime, 0f);
			}

			// 音乐播放倒计时减少
			if (timeLeftToPlay > 0f)
			{
				timeLeftToPlay -= Time.unscaledDeltaTime;

				// 如果音乐播放时间到，开始播放音频
				if (timeLeftToPlay <= 0f)
				{
					audioCom.time = -timeLeftToPlay;
					audioCom.Play();

					timeLeftToPlay = 0f;
				}
			}

			float rate = playingKoreo.GetLatestSampleTime() /
				(float)(playingKoreo.SourceClip.length * playingKoreo.SampleRate);

            if (!isSettingMode)
            {
				// 右下角展示当前音乐播放的百分比
				GamingInfoDisplayUI gUI = GameObject.Find("Canvas").GetComponent<GamingInfoDisplayUI>();
				gUI.updateGamingRate(rate);

			}

			if (1 - rate <= 0.0001f && !isSettingMode)
			{
				isEnd = !isEnd;
				if (!isEnd)
				{
					showScoreBoardScene();
				}
			}
			
		}

		// Update any internal values that depend on externally accessible fields (public or Inspector-driven).
		// 将所有需要从 外部获取值 并更新内部变量 的操作写在这里
		void UpdateInternalValues()
		{
			//根据ms单位的设定值，更新音符判定窗口的宽度（采样单位）
			hitWindowRangeInSamples = (int)(0.001f * hitWindowRangeInMS * SampleRate);
		}

		// 从音符池中读取一个note
		public NoteObject GetFreshNoteObject()
		{
			NoteObject retObj;

			if (noteObjectPool.Count > 0)
			{
				// 从音符栈中弹出首位note
				retObj = noteObjectPool.Pop();
			}
			else
			{
				// 实例化note对象
				retObj = GameObject.Instantiate<NoteObject>(noteObjectArchetype, LaneParent.transform);
			}

			// 将生成的note的状态设置为激活
			retObj.gameObject.SetActive(true);
			retObj.enabled = true;

			return retObj;
		}

		// 禁用note并将其放回音符栈
		public void ReturnNoteObjectToPool(NoteObject obj)
		{
			if (obj != null)
			{
				obj.enabled = false;
				obj.gameObject.SetActive(false);

				noteObjectPool.Push(obj);
			}
		}

		// 重新开始该铺面的游戏，重置所有轨道和数据
		public void Restart()
		{
			// 重置音频
			audioCom.Stop();
			audioCom.time = 0f;

            // 重置视频
            if (bgVideoQuad.activeSelf)
            {
				bgVideoQuad.GetComponent<VideoPlayer>().Stop();
				bgVideoQuad.GetComponent<VideoPlayer>().time = 0f;


			}
			

			// 重置所有还在延时状态的事件  
			// This effectively resets the Koreography and ensures that
			// delayed events that haven't been sent yet do not continue to be sent.
			Koreographer.Instance.FlushDelayQueue(playingKoreo);

			// Reset the Koreography time.  This is usually handled by loading the Koreography. 
			// 重置Koreo的时间
			playingKoreo.ResetTimings();

			// 重置所有轨道，使其从头开始运行Lane脚本的生命周期
			for (int i = 0; i < noteLanes.Count; ++i)
			{
				noteLanes[i].Restart();

			}

			if (!isSettingMode)
			{
				// 重置所有UI和游戏统计信息
				Combo = 0;
				TotalScore = 0;
				Count_maxCombo = 0;
				Count_4thJudge = 0;
				Count_3rdJudge = 0;
				Count_2ndJudge = 0;
				Count_1stJudge = 0;
				Count_missedJudge = 0;


				GamingInfoDisplayUI gUI = GameObject.Find("Canvas").GetComponent<GamingInfoDisplayUI>();
				gUI.showJudgeUI("");
				gUI.resetCombo();
				gUI.setScoreValue(0);
			}

			// 重新开始初始化准备时间
			InitializeLeadIn();

		}
		public void BackToScene(string scene)
		{
			Destroy(FindObjectOfType<LoadKoreoInfo>().gameObject);
			audioCom.Stop();
			audioCom.time = 0f;
			SceneManager.LoadScene(scene);
		}

		public void showScoreBoardScene()
        {
			Score.Instance.TotalScore = TotalScore;
			Score.Instance.maxCombo = Count_maxCombo;
			Score.Instance._1stJudge = Count_1stJudge;
			Score.Instance._2ndJudge = Count_2ndJudge;
			Score.Instance._3rdJudge = Count_3rdJudge;
			Score.Instance._4thJudge = Count_4thJudge;
			Score.Instance.missedJudge = Count_missedJudge;
			SceneManager.LoadScene("ScoreBoardScene");
		}

		#endregion
	}
}
