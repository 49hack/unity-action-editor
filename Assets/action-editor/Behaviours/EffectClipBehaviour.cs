﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [ParentTrack(typeof(EffectTrackBehaviour))]
    public class EffectClipBehaviour : ClipBehaviour
    {
        EffectTrackBehaviour.Particle m_Particle;

        public override void OnEnd(float time, float absoluteTime)
        {
            m_Particle?.End();
            m_Particle = null;
        }

        public override void OnInterrupt(float time, float absoluteTime)
        {
            OnEnd(time, absoluteTime);
        }

        public override void OnSetTime(float time)
        {
            m_Particle?.Evaluate(time);
        }

        public void SetParticle(EffectTrackBehaviour.Particle particle)
        {
            m_Particle = particle;
            m_Particle?.Begin();
        }
    }
}