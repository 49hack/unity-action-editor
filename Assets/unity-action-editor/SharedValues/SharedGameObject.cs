using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("GameObject")]
    public class SharedGameObject : SharedObject<GameObject>
    {
    }

    [System.Serializable]
    public class SharedGameObjectContext : SharedValue<GameObject>
    {
    }
}