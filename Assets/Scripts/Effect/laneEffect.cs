using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laneEffect : MonoBehaviour
{
    private ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = this.GetComponent<ParticleSystem>();
    }

    // 在轨道上播放相应的特效
    void playParticlewithPos(Vector3 pos)
    {
        // 拷贝原本的特效对象
        ParticleSystem psNew = GameObject.Instantiate<ParticleSystem>(ps);
        psNew.transform.position = pos;
        psNew.Play();
        /*ps.transform.position = pos;
        ps.Play();*/
        // Destroy(psNew, psNew.main.duration * 2);

        // 在延迟到粒子特效播放结束后，销毁粒子特效
        // CheckIfAlive(psNew);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
