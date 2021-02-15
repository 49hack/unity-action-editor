using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("AnimationCurve")]
    public class SharedAnimationCurve : SharedObject<AnimationCurve>
    {
        
    }

    [System.Serializable]
    public class SharedAnimationCurveContext : SharedValue<AnimationCurve>
    {
    }
}