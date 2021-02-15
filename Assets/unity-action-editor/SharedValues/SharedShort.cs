using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Short")]
    public class SharedShort : SharedObject<short>
    {
    }

    [System.Serializable]
    public class SharedShortContext : SharedValue<short>
    {
    }
}