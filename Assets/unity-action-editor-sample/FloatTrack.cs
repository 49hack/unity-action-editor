using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Sample
{
    [MenuTitle("Primitive/Float Track")]
    public class FloatTrack : Track
    {
        public override void OnChangeClip(Clip fromClip, float fromWeight, Clip toClip, float toWeight)
        {
            var from = fromClip as FloatClip;
            var to = toClip as FloatClip;
            Print(from, fromWeight, to, toWeight);
        }

        public override void OnChangeWight(Clip fromClip, float fromWeight, Clip toClip, float toWeight)
        {
            var from = fromClip as FloatClip;
            var to = toClip as FloatClip;
            Print(from, fromWeight, to, toWeight);
        }

        void Print(FloatClip from, float fromWeight, FloatClip to, float toWeight)
        {
            if(from == null)
            {
                Debug.Log("Clip nothings.");
                return;
            }

            if(to == null)
            {
                Debug.Log("NonBlend Value: " + from.Value * fromWeight);
                return;
            }

            var val = from.Value * fromWeight + to.Value * toWeight;
            Debug.Log("Blend   Value: " + val);
        }
    }
}