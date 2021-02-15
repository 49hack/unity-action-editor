using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Bounds")]
    public class SharedBounds : SharedObject<Bounds>
    {
        
    }

    [System.Serializable]
    public class SharedBoundsContext : SharedValue<Bounds>
    {
    }
}