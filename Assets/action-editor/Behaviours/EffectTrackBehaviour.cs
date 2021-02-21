using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class EffectTrackBehaviour : TrackBehaviour
    {
        public static string PropNameLocator { get { return nameof(m_Locator); } }
        public static string PropNameEffect { get { return nameof(m_Effect); } }

        [SerializeField] SharedTransformContext m_Locator;
        [SerializeField] SharedGameObjectContext m_Effect;

        Particle m_Particle;
        bool m_IsInterrupted;

        public override void OnPlay()
        {
            m_IsInterrupted = false;
        }

        public override void OnStop()
        {
            m_IsInterrupted = false;
        }

        public override void OnInterrupt()
        {
            m_IsInterrupted = true;
        }

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            Blackboard.Bind(blackboards, m_Effect);
            Blackboard.Bind(blackboards, m_Locator);

            m_Particle = new Particle(m_Effect.Value, m_Locator.Value);
        }

        public override void OnChangeClip(ClipBehaviour fromClip, float fromWeight, ClipBehaviour toClip, float toWeight)
        {
            if (m_IsInterrupted)
                return;

            var effectClip = fromClip as EffectClipBehaviour;
            if(effectClip != null)
            {
                effectClip.SetParticle(m_Particle);
            }
        }

        public override void OnDispose()
        {
            m_IsInterrupted = false;
            m_Particle?.Dispose();
        }

        public class Particle
        {
            static List<ParticleSystem> m_Work = new List<ParticleSystem>();
            
            GameObject m_Root;
            ParticleSystem[] m_ParticleList = new ParticleSystem[0];

            public Particle(GameObject root, Transform parent)
            {
                if (root == null)
                    return;

                m_Root = GameObject.Instantiate(root);
                if (parent != null)
                {
                    m_Root.transform.SetParent(parent, false);
                }

                m_ParticleList = m_Root.GetComponentsInChildren<ParticleSystem>(true);
                End();
            }

            public void Begin()
            {
                if (m_Root != null)
                {
                    m_Root.SetActive(true);
                }
            }

            public void End()
            {
                if (m_Root != null)
                {
                    m_Root.SetActive(false);
                }
            }

            public void Evaluate(float time)
            {
                if (Application.isPlaying)
                    return;

                for(int i = 0; i < m_ParticleList.Length; i++)
                {
                    var ps = m_ParticleList[i];
                    if (ps == null)
                        continue;

                    ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                    var seed = ps.randomSeed;
                    var autoSeed = ps.useAutoRandomSeed;
                    ps.randomSeed = 1;
                    ps.useAutoRandomSeed = false;

                    m_ParticleList[i].Simulate(time, false, true, false);

                    ps.randomSeed = seed;
                    ps.useAutoRandomSeed = autoSeed;
                }
            }

            public void Dispose()
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(m_Root);
                } else
                {
                    GameObject.DestroyImmediate(m_Root);
                }
            }
        }
    }
}