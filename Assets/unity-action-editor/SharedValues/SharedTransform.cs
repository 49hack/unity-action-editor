using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Transform")]
    public class SharedTransform : SharedObject<Transform>
    {
    }

    [System.Serializable]
    public class SharedTransformContext : SharedValue<Transform>
    {
    }
}