using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Object")]
    public class SharedUnityObject : SharedObject<Object>
    {

    }

    [System.Serializable]
    public class SharedUnityObjectContext : SharedValue<Object>
    {
    }
}
