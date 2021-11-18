using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts;
using UnityEngine.UI;

/* ReadInputField： 从文本输入框中提取字符串，然后传递给转换脚本
 * 层级：UI
 */
public class ReadInputField : MonoBehaviour
{
    public TMP_InputField pathIF;
    public TMP_InputField beatmapIF;
    public TMP_InputField audioIF;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
