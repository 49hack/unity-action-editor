using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("LayerMask")]
    public class SharedLayerMask : SharedObject<LayerMask>
    {
        
    }

    [System.Serializable]
    public class SharedLayerMaskContext : SharedValue<LayerMask>
    {
    }
}