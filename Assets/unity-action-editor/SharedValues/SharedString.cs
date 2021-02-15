using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("String")]
    public class SharedString : SharedObject<string>
    {
    }

    [System.Serializable]
    public class SharedStringContext : SharedValue<string>
    {
    }
}