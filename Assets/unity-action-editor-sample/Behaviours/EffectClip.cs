using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Sample
{
    [ParentTrack(typeof(EffectTrack))]
    public class EffectClip : Clip
    {
        EffectTrack.Particle m_Particle;

        public override void OnEnd()
        {
            m_Particle?.End();
            m_Particle = null;
        }

        public override void OnSetTime(float time)
        {
            m_Particle?.Evaluate(time);
        }

        public void SetParticle(EffectTrack.Particle particle)
        {
            m_Particle = particle;
            m_Particle?.Begin();
        }
    }
}