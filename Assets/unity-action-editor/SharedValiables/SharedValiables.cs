using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [System.Serializable]
    public class SharedFloat : SharedValue<float>
    {
    }
    public class SharedFloatObject : SharedObject<float>
    {
    }

    [System.Serializable]
    public class SharedGameObject : SharedValue<GameObject>
    {
    }
    public class SharedGameObjectObject : SharedObject<GameObject>
    {
    }
}
