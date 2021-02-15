using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Int")]
    public class SharedInt : SharedObject<int>
    {
    }

    [System.Serializable]
    public class SharedIntContext : SharedValue<int>
    {
    }
}