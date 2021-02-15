using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Rect")]
    public class SharedRect : SharedObject<Rect>
    {
        
    }

    [System.Serializable]
    public class SharedRectContext : SharedValue<Rect>
    {
    }
}