using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newparticle : MonoBehaviour
{
    private ParticleSystem ps;
    // 用于表示当前对象是否可以销毁，若不能销毁，则采用停用的方式来进行隐藏
    public bool OnlyDeactivate;

    void playParticle()
    {
        ps.Play();
    }

    // 在音符的位置上，播放当前的粒子特效
    void playParticlewithPos(Vector3 pos)
    {
        // 拷贝原本的特效对象
        ParticleSystem psNew = GameObject.Instantiate<ParticleSystem>(ps);
        psNew.transform.position = pos;
        psNew.Play();

        // 在延迟到粒子特效播放结束后，销毁粒子特效
        // CheckIfAlive(psNew);
        // Destroy(psNew, psNew.main.duration * 3);
    }
    void stopParticle()
    {
        ps.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        ps = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 对当前粒子是否存活进行检查
    IEnumerator CheckIfAlive(ParticleSystem ps)
    {
        while (true && ps != null)
        {
            yield return new WaitForSeconds(0.1f);
            if (!ps.IsAlive(true))
            {
                if (OnlyDeactivate)
                {
                    this.gameObject.SetActive(false);
                }
                else
                    GameObject.Destroy(ps.gameObject);
                break;
            }
        }
    }
}
