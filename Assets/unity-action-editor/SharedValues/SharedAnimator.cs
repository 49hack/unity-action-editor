using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Animator")]
    public class SharedAnimator : SharedObject<Animator>
    {
    }

    [System.Serializable]
    public class SharedAnimatorContext : SharedValue<Animator>
    {
    }
}