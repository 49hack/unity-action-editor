using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace ActionEditor.Sample
{
    [CreateAssetMenu(menuName = "Action Editor/Playable Sequence", fileName = "PlayableSequence")]
    public class PlayableSequence : SequenceBehaviour
    {
        PlayableGraph m_Graph;

        public PlayableGraph Graph { get { return m_Graph; } }

        public override void OnCreate(IReadOnlyList<Blackboard> blackboards)
        {
            m_Graph = PlayableGraph.Create(name);
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        }
        public override void OnDispose()
        {
            if (Graph.IsValid())
                Graph.Destroy();
        }
        private void OnDestroy()
        {
            OnDispose();
        }

        public override void OnSetTime(float time)
        {
            if(Graph.IsValid())
                Graph.Evaluate();
        }

        public override void OnPlay()
        {
            if(Graph.GetTimeUpdateMode() == DirectorUpdateMode.Manual)
            {
                Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
                Graph.Stop();
            }

            Graph.Play();
        }

        public override void OnStop()
        {
            Graph.Stop();
        }
        public override void OnPause()
        {
            Graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        }
        public override void OnResume()
        {
            Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            Graph.Stop();
            Graph.Play();
        }

        public override void OnProgress(float fromTime, float toTime)
        {
            if (Application.isPlaying)
                return;
            if (Graph.GetTimeUpdateMode() == DirectorUpdateMode.Manual)
                return;

            Graph.Evaluate(toTime - fromTime);
        }
    }

}