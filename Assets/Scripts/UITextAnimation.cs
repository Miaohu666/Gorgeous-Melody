using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITextAnimation : MonoBehaviour
{
    TMP_Text text;
    bool is_wave_playing;
    int scale_state = 0;
    float default_fontsize;
    public int max_fontsize;
    public void StartAnimWave()
    {
        is_wave_playing = true;
    }
    public void StopAnimWave()
    {
        is_wave_playing = false;
    }

    public void StartAnimScale()
    {
        text.fontSize = default_fontsize;
        scale_state = 1;
    }
    IEnumerator autoPlay()
    {
        yield return new WaitForSeconds(0.4f);
        is_wave_playing = false;
    }

    void UpdateTextAnim_Wave()
    {
        text.ForceMeshUpdate();
        // ��ȡtext�������Ϣ
        TMP_TextInfo textInfo = text.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            // ��ȡ��ǰÿ���ַ�����Ϣ
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
            {
                continue;
            }

            // ��ȡ�ַ���mesh�ϵĶ�����Ϣ��ÿ���ַ��������¡����ϡ����ϡ������ĸ���
            Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            for (int j = 0; j < 4; j++)
            {
                var old_vert = verts[charInfo.vertexIndex + j];
                // ����������ʵ�ֶ���Ч��
                verts[charInfo.vertexIndex + j] = old_vert +
                    new Vector3(0, Mathf.Sin(Time.time * 2f + old_vert.x * 0.01f) * 10f, 0);
            }
        }

        // ��������meshӦ�õ���Ⱦ�õ�mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];

            meshInfo.mesh.vertices = meshInfo.vertices;

            text.UpdateGeometry(meshInfo.mesh, i);

        }

    }

    void UpdateTextAnim_Scale(int state)
    {
        if (state == 1)
        {
            text.fontSize = Mathf.Lerp(text.fontSize, max_fontsize, Time.deltaTime * 150f);
        }
        else if (state == 2)
        {
            text.fontSize = Mathf.Lerp(text.fontSize, default_fontsize, Time.deltaTime * 20f);
        }

        if (text.fontSize >= max_fontsize)
        {
            scale_state = 2;
        }

        if (text.fontSize <= default_fontsize)
        {
            scale_state = 0;
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
        is_wave_playing = false;
        default_fontsize = text.fontSize;
        scale_state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTextAnim_Scale(scale_state);
        if (is_wave_playing)
        {
            UpdateTextAnim_Wave();
        }
    }
}
