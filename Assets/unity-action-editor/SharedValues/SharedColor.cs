using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Color")]
    public class SharedColor : SharedObject<Color>
    {
        
    }

    [System.Serializable]
    public class SharedColorContext : SharedValue<Color>
    {
    }
}