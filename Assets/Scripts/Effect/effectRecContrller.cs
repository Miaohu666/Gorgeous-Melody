using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effectRecContrller : MonoBehaviour
{
    GameObject[] ps;
    int effectNum;
    public int effectChoice = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取当前effect下所有的孩子节点（ParticleSystem）
        List<GameObject> psList = new List<GameObject>();
        effectNum = transform.childCount;
        for(int i = 0; i < effectNum; i++)
        {
            psList.Add(transform.GetChild(effectNum).gameObject);
        }
        ps = psList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        // 拷贝一份对象用于播放特效
        GameObject particles = (GameObject)Instantiate(ps[effectChoice]);
        // 激活使用当前的GameObject
        particles.SetActive(true);

        // 将获取到的特效间接转换为ParticleSystem
        ParticleSystem playPs = particles.GetComponent<ParticleSystem>();
        // 播放
        playPs.Play();
    }
}
