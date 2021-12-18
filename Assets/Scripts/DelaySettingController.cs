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
	 * ���û�������ֵ
	 */
	public void SetNewDelay(int newDelay)
	{
		Koreographer.Instance.EventDelayInSeconds = newDelay * 0.001f;
		readoutText.text = newDelay.ToString() + "ms";
	}

	/*
	 * ��������ֵ����ʱ�������ӳ�ֵ
	 */
	public void UpdateDelay()
	{
		int newDelay = (int)slider.value;
		// �������µ�ǰ��EventDelayInSeconds��ʹ��ҿ��Կ���Ч��
		Koreographer.Instance.EventDelayInSeconds = newDelay * 0.001f;
		readoutText.text = newDelay.ToString() + "ms";
	}

	/*
	 * ���滬������ֵ���־�����
	 */
	public void SaveDelayValue(){
		// ��ȡ��������ֵ
		int delay = (int)slider.value;
		// ��ֵд��PlayerPrefs
		PlayerPrefs.SetInt("DelayInMS", delay);
	}

	void Start()
	{
		// ��PlayerPrefs�л�ȡ֮ǰ���ӳ�����ֵ������У�
		Koreographer.Instance.EventDelayInSeconds = PlayerPrefs.GetInt("DelayInMS", 0) * 0.001f;
		float delayTime = PlayerPrefs.GetInt("DelayInMS", 0) * 1f;

		// ����������λ����Ӧλ��
		slider.value = delayTime;
		SetNewDelay((int)slider.value);
	}

}
