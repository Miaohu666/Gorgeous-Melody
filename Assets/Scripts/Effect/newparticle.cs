using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newparticle : MonoBehaviour
{
    private ParticleSystem ps;
    // ���ڱ�ʾ��ǰ�����Ƿ�������٣����������٣������ͣ�õķ�ʽ����������
    public bool OnlyDeactivate;

    void playParticle()
    {
        ps.Play();
    }

    // ��������λ���ϣ����ŵ�ǰ��������Ч
    void playParticlewithPos(Vector3 pos)
    {
        // ����ԭ������Ч����
        ParticleSystem psNew = GameObject.Instantiate<ParticleSystem>(ps);
        psNew.transform.position = pos;
        psNew.Play();

        // ���ӳٵ�������Ч���Ž���������������Ч
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

    // �Ե�ǰ�����Ƿ�����м��
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
