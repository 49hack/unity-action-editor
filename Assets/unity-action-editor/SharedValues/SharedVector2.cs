using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Vector2")]
    public class SharedVector2 : SharedObject<Vector2>
    {
    }

    [System.Serializable]
    public class SharedVector2Context : SharedValue<Vector2>
    {
    }
}