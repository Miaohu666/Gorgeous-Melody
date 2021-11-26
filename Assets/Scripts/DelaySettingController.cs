using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DelaySettingController : MonoBehaviour
{
	[Tooltip("The Text Component that will display the Event Delay number.")]
	public TMP_Text readoutText;

	[Tooltip("The Slider that controls the Event Delay number.")]
	public Slider slider;

	/*
	 * 设置滑动条的值
	 */
	public void SetNewDelay(int newDelay)
	{
		Koreographer.Instance.EventDelayInSeconds = newDelay * 0.001f;
		readoutText.text = newDelay.ToString() + "ms";
	}

	/*
	 * 滑动条的值更新时，更新延迟值
	 */
	public void UpdateDelay()
	{
		int newDelay = (int)slider.value;
		// 立即更新当前的EventDelayInSeconds，使玩家可以看到效果
		Koreographer.Instance.EventDelayInSeconds = newDelay * 0.001f;
		readoutText.text = newDelay.ToString() + "ms";
	}

	/*
	 * 保存滑动条的值到持久数据
	 */
	public void SaveDelayValue(){
		// 获取滑动条的值
		int delay = (int)slider.value;
		// 将值写入PlayerPrefs
		PlayerPrefs.SetInt("DelayInMS", delay);
	}

	void Start()
	{
		// 从PlayerPrefs中获取之前的延迟设置值（如果有）
		Koreographer.Instance.EventDelayInSeconds = PlayerPrefs.GetInt("DelayInMS", 0) * 0.001f;
		float delayTime = PlayerPrefs.GetInt("DelayInMS", 0) * 1f;

		// 将滑动条定位到相应位置
		slider.value = delayTime;
		SetNewDelay((int)slider.value);
	}

}
