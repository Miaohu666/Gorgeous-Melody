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
            evt.StartSample = sp; // 设置开始采样时间
            evt.EndSample = ep; // 设置结束采样时间
            TextPayload payload  = new TextPayload(); // new一个字符串paylod
            payload.TextVal = ps; // 设置payload字符串
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
