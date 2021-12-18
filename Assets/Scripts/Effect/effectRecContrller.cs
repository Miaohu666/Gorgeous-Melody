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
        // ��ȡ��ǰeffect�����еĺ��ӽڵ㣨ParticleSystem��
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
        // ����һ�ݶ������ڲ�����Ч
        GameObject particles = (GameObject)Instantiate(ps[effectChoice]);
        // ����ʹ�õ�ǰ��GameObject
        particles.SetActive(true);

        // ����ȡ������Ч���ת��ΪParticleSystem
        ParticleSystem playPs = particles.GetComponent<ParticleSystem>();
        // ����
        playPs.Play();
    }
}
