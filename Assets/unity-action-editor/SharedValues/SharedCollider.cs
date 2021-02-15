using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Collider")]
    public class SharedCollider : SharedObject<Collider>
    {
    }

    [System.Serializable]
    public class SharedColliderContext : SharedValue<Collider>
    {
    }
}