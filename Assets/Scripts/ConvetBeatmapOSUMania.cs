using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SonicBloom.Koreo.Demos
{
    public class ConvetBeatmapOSUMania : MonoBehaviour
    {
        #region Fields
        public Koreography targetKoreo;
        private KoreographyTrack targetTrack;
        #endregion

        #region Methods
        KoreographyEvent geneEvent(int sp, int ep, string ps)
        {
            KoreographyEvent evt = new KoreographyEvent();
            evt.StartSample = sp; // ���ÿ�ʼ����ʱ��
            evt.EndSample = ep; // ���ý�������ʱ��
            TextPayload payload  = new TextPayload(); // newһ���ַ���paylod
            payload.TextVal = ps; // ����payload�ַ���
            evt.Payload = payload;
            return evt;
        }

        void addEvents(KoreographyTrack track, List<KoreographyEvent> events)
        {
            foreach(KoreographyEvent evt in events)
            {
                track.AddEvent(evt);
            }
        }
        void modifyTargetKoreo()
        {
            
            targetKoreo.AddTrack(targetTrack);
        }
        void Start()
        {

        }

        
        void Update()
        {

        }
        #endregion
    }
}
