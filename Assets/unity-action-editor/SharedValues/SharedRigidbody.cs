using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Rigidbody")]
    public class SharedRigidbody : SharedObject<Rigidbody>
    {
    }

    [System.Serializable]
    public class SharedRigidbodyContext : SharedValue<Rigidbody>
    {
    }
}