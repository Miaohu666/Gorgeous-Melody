//----------------------------------------------
//      代码由Koreographer插件官方脚本改写                
//  LaneControlle：定义和控制音符轨道的生命周期 
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace SonicBloom.Koreo.Demos
{
	[AddComponentMenu("Koreographer/Demos/Rhythm Game/Lane Controller")]
	public class LaneController : MonoBehaviour
	{
		#region Fields

		[Tooltip("定义该轨道上note和判定线的颜色")]
		public Color color = Color.blue;

		[Tooltip("定义判定线的贴图组件引用")]
		public SpriteRenderer targetVisuals;

		[Tooltip("定义该轨道对应玩家需要按下的按钮")]
		public KeyCode keyboardButton;

		[Tooltip("定义该轨道在Koroe中所对应的Payload字符串列表")]
		public List<string> matchedPayloads = new List<string>();

		// 存储这条轨道上的所有事件，游戏控制器会在开始加载时为其添加事件
		List<KoreographyEvent> laneEvents = new List<KoreographyEvent>();

		// 存储当前所有在这条轨道上激活（即在屏幕范围内显示）的note物体
		//Input and lifetime validity checks are tracked with operations on this Queue.
		Queue<NoteObject> trackedNotes = new Queue<NoteObject>();

		// 引用游戏控制器
		RhythmGameController gameController;

		// 轨道的起点和终点对应的Y轴位置  This game goes from the top of the screen to the bottom.
		float spawnY = 0f;
		float despawnY = 0f;

		// 记录下一个要生成的note对应的事件在laneEvents列表中的序号
		int pendingEventIdx = 0;

		// 以下参数 用来在判定线对应按钮按下时 给予判定线缩放反馈
		Vector3 defaultScale;
		float scaleNormal = 1f;
		float scalePress = 1.4f;
		float scaleHold = 1.2f;

		// 记录当前轨道正在打击的note，如无则赋值为null（必须为全局变量）
		NoteObject hitNote = null;

		#endregion
		#region Properties

		// 获得note在屏幕顶端渲染出来时的位置
		public Vector3 SpawnPosition
		{
			get
			{
				return new Vector3(transform.position.x, spawnY);
			}
		}

		// 获得判定线所在的位置
		public Vector3 TargetPosition
		{
			get
			{
				return new Vector3(transform.position.x, transform.position.y);
			}
		}

		// 获得note的销毁位置，超过该位置的note会被回收到音符池
		public float DespawnY
		{
			get
			{
				return despawnY;
			}
		}

		#endregion
		#region Methods

		// 初始化游戏控制器
		public void Initialize(RhythmGameController controller)
		{
			gameController = controller;
		}

		// 清除所有轨道上的note，返回初始化状态
		public void Restart()
		{
			pendingEventIdx = 0;

			// Clear out the tracked notes.
			int numToClear = trackedNotes.Count;
			for (int i = 0; i < numToClear; ++i)
			{
				trackedNotes.Dequeue().OnClear();
			}
		}

		void Start()
		{
			// 获得相机的边界
			// Get the vertical bounds of the camera.  Offset by a bit to allow for offscreen spawning/removal.
			float cameraOffsetZ = -Camera.main.transform.position.z;
			// 将相机边界上下加上一点，作为出生点和销毁点，以便出生点和销毁点不在玩家的视野范围内
			// 下边界延长一点，以防过长的hold提前销毁
			spawnY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, cameraOffsetZ)).y + 1f;
			despawnY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, cameraOffsetZ)).y - 10f;

			// 更新轨道的标志色
			targetVisuals.color = color;

			// 获得在游戏场景里设定的默认缩放值
			defaultScale = targetVisuals.transform.localScale;
		}

		void Update()
		{
			// 清空队列里的已判定为miss的note
			while (trackedNotes.Count > 0 && trackedNotes.Peek().IsNoteMissed())
			{
				// 调用UI和判定信息处理函数，TODO：看能不能封装到里面，不再外部调用
				trackedNotes.Peek().scoreInfoUpdate(judgeClass: 0);
				// 将note弹出队列
				trackedNotes.Dequeue();
			}

			// 检查是否有新的note生成
			CheckSpawnNext();

			// 检查玩家输入
			// 如果要更改为触控输入，修改Event System的参数而无需修改以下输入检测代码
			// 触控输入系统没有“长按”判定的内置实现，如需实现，请额外编写代码
			// Note that touch controls are handled by the Event System, which is all
			//  configured within the Inspector on the buttons themselves, using the same functions as
			//  what is found here.  Touch input does not have a built-in concept of "Held", so it is not
			//  currently supported.


			if (Input.GetKeyDown(keyboardButton))
			{
				hitNote = CheckNoteHit(); // 判断是否打击到note
				SetScalePress(); // 触发判定线缩放的视觉效果（按下）
			}
			else if (Input.GetKey(keyboardButton))
			{
				CheckNoteHold(); // 判断玩家是否在长按note
				SetScaleHold(); // 触发判定线缩放的视觉效果（长按）
			}
			else if (Input.GetKeyUp(keyboardButton))
			{
				if(hitNote != null)
					CheckNoteRelese(hitNote); // 判断玩家是否松开长按note 
				SetScaleDefault(); // 当玩家松开按钮时，判定线效果重置为默认
			}
		}

		// 使得判定线贴图组件的大小缩放为指定的倍数
		void AdjustScale(float multiplier)
		{
			targetVisuals.transform.localScale = defaultScale * multiplier;
		}

		// 确定note的读谱时间的函数，返回其从生成到判定线的音频采样数量
		// Uses the Target position and the current Note Object speed to determine the audio sample
		//  "position" of the spawn location.  This value is relative to the audio sample position at
		//  the Target position (the "now" time).
		int GetSpawnSampleOffset()
		{
			// 获得出生点到判定线的距离
			float spawnDistToTarget = spawnY - transform.position.y;
			
			// 获得在当前下落速度下，从出生点到判定线的时间（单位：秒）
			double spawnSecsToTarget = (double)spawnDistToTarget / (double)gameController.noteSpeed;
			
			// 将时间单位转化为音频采样数量
			return (int)(spawnSecsToTarget * gameController.SampleRate);
		}

		// 检查该判定线是否击中note，如果是，将被击中的note从游戏场景和trackedNotes队列中移除。
		public NoteObject CheckNoteHit()
		{
			// 永远只检查队列中的第一个note是否被击中，所以记得在update一开始一定要清除missed的note
			if (trackedNotes.Count > 0 && trackedNotes.Peek().IsNoteHittable())
			{
				NoteObject hitNote = trackedNotes.Dequeue();

				hitNote.OnHit();
				return hitNote;
			}
			return null;
		}

		public void CheckNoteHold()
		{
			// Notice：好像没有什么要check的（啊这
			// 判断玩家是否在中途松开hold的情况会写在CheckNoteRelese()里
			return;
		}

		public void CheckNoteRelese(NoteObject cur_note)
		{
			if(cur_note.getNoteType() == "H")
			{
				if (cur_note.IsNoteReleseable())
				{

					cur_note.OnRelese();
				}
				else if (cur_note.IsNoteMissed(false))
				{
					cur_note.OnHoldLateRelesed();
				}
				else
				{
					cur_note.OnHoldBreak();
					// TODO：存在bug，其他note也会受影响变为半透明，暂时禁用
					//expireHoldNote(cur_note);
				}
			}
		}

		void expireHoldNote(NoteObject cur_note)
		{
			//将note颜色变半透明
			cur_note.headVisuals.color = expireVisualColor(cur_note.headVisuals.color);
			cur_note.bodyVisuals.color = expireVisualColor(cur_note.bodyVisuals.color);
			cur_note.endVisuals.color = expireVisualColor(cur_note.endVisuals.color);
		}
		Color expireVisualColor(Color color)
		{
			return new Color(color.r, color.g, color.b, color.a / 3);
		}

		// 检查下一个note是否应该在这一帧生成
		// 如果是，将其生成并加入trackedNotes队列
		void CheckSpawnNext()
		{
			// 获得note的读谱时间
			int samplesToTarget = GetSpawnSampleOffset();
			
			// 获得当前采样时间
			int currentTime = gameController.DelayedSampleTime;
			
			// 如果列表中还有未生成的note，且其 开始采样时间 已经在 当前时间+读谱时间 以后，则应该生成note
			while (pendingEventIdx < laneEvents.Count &&
				   laneEvents[pendingEventIdx].StartSample < currentTime + samplesToTarget)
			{
				KoreographyEvent evt = laneEvents[pendingEventIdx];
				
				NoteObject newObj = gameController.GetFreshNoteObject();
				newObj.Initialize(evt, color, this, gameController);

				// 将生成的note加入队列
				trackedNotes.Enqueue(newObj);
				
				// 生成note的序号++
				pendingEventIdx++;
			}
		}

		// 将特定Koreo事件加入laneEvents列表
		public void AddEventToLane(KoreographyEvent evt)
		{
			laneEvents.Add(evt);
		}

		// 判断音符事件所搭载的payload字符串在不在该轨道的标志payload列表中
		// 即单个音符事件payload所代表的音符是否应该出现在本轨道上
		// Checks to see if the string value passed in matches any of the configured values specified
		//  in the matchedPayloads List.
		public bool DoesMatchPayload(string payload)
		{
			bool bMatched = false;

			// 将payload的第二部分分离出来，作为轨道判定符
			string payload_flag = "";
			try
			{
				payload_flag = payload.Split(',')[1];
			}
			catch (System.Exception ex)
			{
				Debug.Log(ex.Message);
			}

			for (int i = 0; i < matchedPayloads.Count; ++i)
			{
				if (payload_flag == matchedPayloads[i])
				{
					bMatched = true;
					break;
				}
			}

			return bMatched;
		}

		//以下函数用来处理判定线的缩放视觉效果

		// Sets the Target scale to the original default scale.
		public void SetScaleDefault()
		{
			AdjustScale(scaleNormal);
		}

		// Sets the Target scale to the specified "initially pressed" scale.
		public void SetScalePress()
		{
			AdjustScale(scalePress);
		}

		// Sets the Target scale to the specified "continuously held" scale.
		public void SetScaleHold()
		{
			AdjustScale(scaleHold);
		}
		
		#endregion
	}
}
