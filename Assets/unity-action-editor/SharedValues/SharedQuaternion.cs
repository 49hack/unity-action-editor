using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Quaternion")]
    public class SharedQuaternion : SharedObject<Quaternion>
    {
    }

    [System.Serializable]
    public class SharedQuaternionContext : SharedValue<Quaternion>
    {
    }
}