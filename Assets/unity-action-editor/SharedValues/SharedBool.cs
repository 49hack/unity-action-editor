using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Bool")]
    public class SharedBool : SharedObject<bool>
    {
    }

    [System.Serializable]
    public class SharedBoolContext : SharedValue<bool>
    {
    }
}