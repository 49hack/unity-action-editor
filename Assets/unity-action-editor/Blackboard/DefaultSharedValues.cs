using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [System.Serializable, MenuTitle("int")] public class SharedInt : SharedValue<int>{ public SharedInt(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedIntContext : SharedValueContext<int>{}

    [System.Serializable, MenuTitle("uint")] public class SharedUInt : SharedValue<uint> { public SharedUInt(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedUIntContext : SharedValueContext<uint> { }

    [System.Serializable, MenuTitle("long")] public class SharedLong : SharedValue<long> { public SharedLong(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedLongContext : SharedValueContext<long> { }

    [System.Serializable, MenuTitle("ulong")] public class SharedULong : SharedValue<ulong> { public SharedULong(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedULongContext : SharedValueContext<ulong> { }

    [System.Serializable, MenuTitle("short")] public class SharedShort : SharedValue<short> { public SharedShort(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedShortContext : SharedValueContext<short> { }

    [System.Serializable, MenuTitle("ushort")] public class SharedUShort : SharedValue<ushort> { public SharedUShort(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedUShortContext : SharedValueContext<ushort> { }

    [System.Serializable, MenuTitle("byte")] public class SharedByte : SharedValue<byte> { public SharedByte(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedByteContext : SharedValueContext<byte> { }

    [System.Serializable, MenuTitle("float")] public class SharedFloat : SharedValue<float> { public SharedFloat(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedFloatContext : SharedValueContext<float> { }

    [System.Serializable, MenuTitle("double")] public class SharedDouble : SharedValue<double> { public SharedDouble(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedDoubleContext : SharedValueContext<double> { }

    [System.Serializable, MenuTitle("string")] public class SharedString : SharedValue<string> { public SharedString(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedStringContext : SharedValueContext<string> { }

    [System.Serializable, MenuTitle("Vector2")] public class SharedVector2 : SharedValue<Vector2> { public SharedVector2(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Vector2Int")] public class SharedVector2Int : SharedValue<Vector2Int> { public SharedVector2Int(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Vector3")] public class SharedVector3 : SharedValue<Vector3> { public SharedVector3(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Vector3Int")] public class SharedVector3Int : SharedValue<Vector3Int> { public SharedVector3Int(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Vector4")] public class SharedVector4 : SharedValue<Vector4> { public SharedVector4(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Rect")] public class SharedRect : SharedValue<Rect> { public SharedRect(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Bounds")] public class SharedBounds : SharedValue<Bounds> { public SharedBounds(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Color")] public class SharedColor : SharedValue<Color> { public SharedColor(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Color32")] public class SharedColor32 : SharedValue<Color32> { public SharedColor32(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("AnimationCurve")] public class SharedAnimationCurve : SharedValue<AnimationCurve> { public SharedAnimationCurve(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("GameObject")] public class SharedGameObject : SharedValue<GameObject> { public SharedGameObject(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Transform")] public class SharedTransform : SharedValue<Transform> { public SharedTransform(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Rigidbody")] public class SharedRigidbody : SharedValue<Rigidbody> { public SharedRigidbody(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("Collider")] public class SharedCollider : SharedValue<Collider> { public SharedCollider(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("BoxCollider")] public class SharedBoxCollider : SharedValue<BoxCollider> { public SharedBoxCollider(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("SphereCollider")] public class SharedSphereCollider : SharedValue<SphereCollider> { public SharedSphereCollider(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("CapsuleCollider")] public class SharedCapsuleCollider : SharedValue<CapsuleCollider> { public SharedCapsuleCollider(string propertyName) : base(propertyName) { } }
    [System.Serializable, MenuTitle("MeshCollider")] public class SharedMeshCollider : SharedValue<MeshCollider> { public SharedMeshCollider(string propertyName) : base(propertyName) { } }

    [System.Serializable, MenuTitle("Animator")] public class SharedAnimator : SharedValue<Animator> { public SharedAnimator(string propertyName) : base(propertyName) { } }
    [System.Serializable] public class SharedAnimatorContext : SharedValueContext<Animator> { }
}
