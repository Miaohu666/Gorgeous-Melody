using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newparticle : MonoBehaviour
{
    private ParticleSystem ps;
    public float playTime = 3f;

    void playParticle()
    {
        ps.Play();
    }

    // ��������λ���ϣ����ŵ�ǰ��������Ч
    void playParticlewithPos(Vector3 pos)
    {
        ps.transform.position = pos;
        ps.Play();
    }
    void stopParticle()
    {
        ps.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        ps = this.GetComponent<ParticleSystem>();
        // ps.Play();
    }

    // Update is called once per frame
    void Update()
    {
        playTime -= Time.deltaTime;
        if(playTime < 0)
        {
            // ps.Play();
            playTime = 3f;
        }
    }
}
