//----------------------------------------------
//      代码由Koreographer插件官方脚本改写                
//   NoteObject：定义和控制note音符的生命周期
//----------------------------------------------

using System;
using UnityEngine;

namespace SonicBloom.Koreo.Demos
{
	[AddComponentMenu("Koreographer/Demos/Rhythm Game/Note Object")]
	public class NoteObject : MonoBehaviour
	{
		#region Fields

		[Tooltip("音符物体的贴图")]
		public SpriteRenderer headVisuals;
		// body和end只在长按中使用，默认组件enable状态为false
		public SpriteRenderer bodyVisuals;
		public SpriteRenderer endVisuals;

		[Tooltip("UI更新脚本引用")]
		public GamingInfoDisplayUI gamingInfoDisplay;

		[Tooltip("长条初始化缩放系数，由使用的贴图原型的比例决定")]
		public float tranSizeDelta = 1.5f;


		// 该音符的类型，由一个字符串表示
		// 'C': 点按
		// 'F': 接收
		// 'H': 长按
		string noteType = "";

		// 该音符是否为正在被长按的状态，默认false
		bool isOnHolding = false;
		float bodyVisualLength = 0.0f;

		// If active, the KoreographyEvent that this Note Object wraps.  Contains the relevant timing information.
		// 该音符绑定的Koreo事件
		KoreographyEvent trackedEvent;

		// If active, the Lane Controller that this Note Object is contained by.
		// 该音符对应的轨道控制器
		LaneController laneController;

		// If active, the Rhythm Game Controller that controls the game this Note Object is found within.
		// 游戏整体流程控制器 RhythmGameController
		RhythmGameController gameController;

		public bool IsOnHolding { 
			get => isOnHolding; 
			set => isOnHolding = value; 
		}

		#endregion
		#region Static Methods
		// 静态方法：重写Lerp插值函数
		// Unclamped Lerp.  Same as Vector3.Lerp without the [0.0-1.0] clamping.
		// 和Unity的Lerp一样，但两边都是开区间（不包括两边的值0.0和1.0）
		static Vector3 Lerp(Vector3 from, Vector3 to, float t)
		{
			return new Vector3 (from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
		}

		#endregion
		#region Methods

		// 初始化note物件
		public void Initialize(KoreographyEvent evt, Color color, LaneController laneCont, RhythmGameController gameCont)
		{
			trackedEvent = evt; // 初始化事件
			// 初始化颜色
			headVisuals.color = color;
			bodyVisuals.color = color;
			endVisuals.color = color;

			laneController = laneCont; // 初始化轨道
			gameController = gameCont; // 初始化游戏控制器

			// 初始化note位置
			UpdatePosition();

			// 初始化note种类
			UpdateNoteType(evt.GetTextValue());

			headVisuals.enabled = true;
			if(noteType == "H")
			{
				bodyVisuals.enabled = true;
				endVisuals.enabled = true;
				SetBodyVisualsLength();
			}
			else
			{
				bodyVisuals.enabled = false;
				endVisuals.enabled = false;
			}

			IsOnHolding = false;
		}

		private void SetBodyVisualsLength()
		{
			// 获得单位(世界坐标长度)内经过的采样数（根据音频采样率决定），简称spu
			float samplesPerUnit = gameController.SampleRate / gameController.noteSpeed;
			float samplesHoldLength = trackedEvent.EndSample - trackedEvent.StartSample;

			// 根据音符长度更改长按条的大小
			Vector3 bodyScale = bodyVisuals.transform.localScale;
			bodyScale.y = (samplesHoldLength / samplesPerUnit) * tranSizeDelta ;
			bodyVisuals.transform.localScale = bodyScale;
			
			// Debug.Log(bodyVisuals.size.y);

			// 更新长度到全局变量
			bodyVisualLength = bodyVisuals.transform.localScale.y;

			
			Vector3 headPos = headVisuals.transform.position;
			headPos.y += (samplesHoldLength / samplesPerUnit);
			// 将note尾部的标志也初始化到正确位置
			endVisuals.transform.position = headPos;
		}

		// 将note物体的属性重置为默认值
		void Reset()
		{
			trackedEvent = null;
			laneController = null;
			gameController = null;
		}

		void Update()
		{
			// 游戏过程中，更新note的可视长度（可不进行自适应修改)
			// UpdateHeight();
			// 游戏过程中，更新note的位置
			UpdatePosition();

			// 如果note位置低于销毁线，将该note放回音符池，并重置note
			if (transform.position.y <= laneController.DespawnY)
			{
				ReturnToPool();
			}

		}

		// 【弃用】更新note的可视长度，长度缩放由判定窗口的大小决定，判定窗口越大，note的可视长度越长 
		void UpdateHeight()
		{
			// 定义note的原本长度，由贴图高度决定
			float baseUnitHeight = headVisuals.sprite.rect.height / headVisuals.sprite.pixelsPerUnit;
			// 定义判定区的长度
			float targetUnitHeight = gameController.WindowSizeInUnits * 2f;	// Double it for before/after.

			// 按照比例对note进行缩放
			Vector3 scale = transform.localScale;
			scale.y = targetUnitHeight / baseUnitHeight;	
			transform.localScale = scale;
		}

		// 根据payload更新note的种类
		void UpdateNoteType(string payload)
		{
			string type_flag = "";

			try
			{
				type_flag = payload.Split(',')[0];
			}
			catch(System.Exception ex)
			{
				Debug.LogError(ex.Message);
			}

			
			if (!(type_flag == "C" | type_flag == "F" | type_flag == "H")){
				Debug.LogError("Event start at : " + trackedEvent.StartSample + 
					" has wrong payload format as " + payload);
			}
			else
			{
				noteType = type_flag;
			}
		}

		// 更新note的位置
		void UpdatePosition()
		{
			if (IsOnHolding)
			{
				// 需要在holding过程中进行的元素位移操作
				UpdateHoldingViusals();

				// 如果长按按钮结束，不论其有没有判定成功，将note可视化效果隐藏
				// 注意，此时不能将长按note放回音符池，这会导致音符栈的进出顺序错乱造成bug
				// TODO：处理尾判miss和update代码堆栈调用优化
				if (IsNoteMissed(false))
				{
					headVisuals.enabled = false;
					bodyVisuals.enabled = false;
					endVisuals.enabled = false;
				}
			}
			else
			{
				// 获得单位(unity距离)内经过的采样数（根据音频采样率决定），简称spu
				// Get the number of samples we traverse given the current speed in Units-Per-Second.
				float samplesPerUnit = gameController.SampleRate / gameController.noteSpeed;

				// Our position is offset by the distance from the target in world coordinates.  This depends on
				//  the distance from "perfect time" in samples (the time of the Koreography Event!).
				Vector3 pos = laneController.TargetPosition;
				// 位置相对于判定线的移动量 = （当前帧的音频采样数 - 本note应当被击中时的音频采样数）/ spu
				pos.y -= (gameController.DelayedSampleTime - trackedEvent.StartSample) / samplesPerUnit;
				transform.position = pos;
				
			}
		}

		internal string getNoteType()
		{
			return noteType;
		}



		// 判断本note是否在判定窗口中
		// Checks to see if the Note Object is currently hittable or not based on current audio sample
		//  position and the configured hit window width in samples (this window used during checks for both
		//  before/after the specific sample time of the Note Object).
		public bool IsNoteHittable()
		{
			int noteTime = trackedEvent.StartSample; // 本note应当被击中时的音频采样数
			int curTime = gameController.DelayedSampleTime; // 当前帧的音频采样数
			int hitWindow = gameController.HitWindowSampleWidth; // 判断窗口以采样为单位的宽度

			return (Mathf.Abs(noteTime - curTime) <= hitWindow);
		}

		// 判断本note(长按)是否在应该松开的判定窗口中
		public bool IsNoteReleseable()
		{
			if(noteType == "H")
			{
				int noteEndTime = trackedEvent.EndSample; // 本note结束时的音频采样数
				int curTime = gameController.DelayedSampleTime; // 当前帧的音频采样数
				int hitWindow = gameController.HitWindowSampleWidth; // 判断窗口以采样为单位的宽度

				return (Mathf.Abs(noteEndTime - curTime) <= hitWindow);
			}
			else
			{
				return false;
			}
		}

		// 判断note是否已经离开判定窗口，默认判定开始时间，传入参数false来判定结束时间
		// Checks to see if the note is no longer hittable based on the configured hit window width in samples.
		public bool IsNoteMissed(bool atStart=true)
		{

			bool bMissed = true;

			if (enabled)
			{
				int noteTime = 0;
				if(atStart == true)
				{
					noteTime = trackedEvent.StartSample;
				}
				else
				{
					noteTime = trackedEvent.EndSample;
				}
				int curTime = gameController.DelayedSampleTime;
				int hitWindow = gameController.HitWindowSampleWidth;

				bMissed = (curTime - noteTime > hitWindow);
			}
			
			return bMissed;
		}


		// 音符被击中或miss后，将note返回音符池，减少渲染管线的负担
		void ReturnToPool()
		{
			gameController.ReturnNoteObjectToPool(this);
			Reset();
		}

		// 当音符被击中时，调用以下操作
		// TODO:传入hit时延来获得不同等级的判定
		public void OnHit()
		{
			gamingInfoDisplay.showJudgeUI("Good");
			if (noteType == "C" | noteType == "F")
			{
				// TODO：处理UI、动画和得分情况
				ReturnToPool();
			}
			else
			{
				// 该变量用来处理Update中的位移函数，使得其处理不同的note的情况不同
				IsOnHolding = true;
				// 将长条note的头部对准判定线，以防止玩家操作延迟造成的视觉误差
				transform.position = new Vector3(transform.position.x,
					laneController.transform.position.y ,transform.position.z);
				

				// TODO：处理UI、动画和得分情况
			}
		}

		public void OnRelese()
		{

			// TODO：处理长按完成UI、动画和得分情况
			gamingInfoDisplay.showJudgeUI("Relesed");

			Debug.Log("relesed.");
			// ReturnToPool();
			headVisuals.enabled = false;
			bodyVisuals.enabled = false;
			endVisuals.enabled = false;
		}

		public void OnHoldBreak()
		{
			// TODO：处理长按中断UI、动画和得分情况
			gamingInfoDisplay.showJudgeUI("Breaked");

			Debug.Log("breaked.");
			expireHoldNote();
		}

		public void OnHoldLateRelesed()
		{
			gamingInfoDisplay.showJudgeUI("Miss");
			// 如果有尾判，在这里处理尾判事项
			// 如果没有尾判，略过该函数
			Debug.Log("尾判miss了");
		}

		void expireHoldNote()
		{
			//将note颜色变半透明
			headVisuals.color = expireVisualColor(headVisuals.color);
			bodyVisuals.color = expireVisualColor(headVisuals.color);
			endVisuals.color = expireVisualColor(headVisuals.color);
		}
		Color expireVisualColor(Color color)
		{
			return new Color(color.r, color.g, color.b, color.a / 3);
		}

		// 更新长按note条的视觉效果（变短）
		void UpdateHoldingViusals()
		{
			// 获得当前长按note的完成度，并据此缩放body条的长度
			float scaleDelta = 1.0f - trackedEvent.GetEventDeltaAtSampleTime(gameController.DelayedSampleTime);
			bodyVisuals.transform.localScale = new Vector3(bodyVisuals.transform.localScale.x, 
				bodyVisualLength * scaleDelta, bodyVisuals.transform.localScale.z);


			// 接下来移动尾部note标志，使其跟随相同的速度移动
			float samplesPerUnit = gameController.SampleRate / gameController.noteSpeed;

			Vector3 pos = laneController.transform.position;
			// 位置相对判定线的移动量 = （当前帧的音频采样数 - 本note应当松开时的音频采样数）/ spu
			pos.y -= (gameController.DelayedSampleTime - trackedEvent.EndSample) / samplesPerUnit;
			endVisuals.transform.position = pos;

		}

		// 当重启游戏需要重置音符时，调用以下操作
		public void OnClear()
		{
			ReturnToPool();
		}

		#endregion
	}
}
