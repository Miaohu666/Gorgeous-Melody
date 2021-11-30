using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts;
using UnityEngine.UI;

/* ReadInputField�� ���ı����������ȡ�ַ�����Ȼ�󴫵ݸ�ת���ű�
 * �㼶��UI
 */
public class ReadInputField : MonoBehaviour
{
    public TMP_InputField pathIF;
    public TMP_InputField beatmapIF;
    public TMP_InputField audioIF;
    public Slider slider;
    public TMP_Text readoutSpeedValue;
    public Button startButton;

    public void GetInputValue()
    {
        ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
        conveter.sourceFilePath = pathIF.text;
        conveter.sourceBeatmapName = beatmapIF.text;
        conveter.sourceAudioName = audioIF.text;
    }
    public void OnLoadCliked()
    {
        ConvetBeatmapOSUMania conveter = gameObject.GetComponent<ConvetBeatmapOSUMania>();
        GetInputValue();
        conveter.StartConvet();
        startButton.interactable = true;
    }

    public void OnNoteSpeedValueChanged()
    {
        readoutSpeedValue.text = ((int)slider.value).ToString();
        PlayerPrefs.SetFloat("NoteSpeed", (int)slider.value);
    }

    // Start is called before the first frame update
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("NoteSpeed", 8.0f);
        readoutSpeedValue.text = slider.value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
