using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    [SerializeField] bool m_Exec;
    [SerializeField] bool m_returnExec;

    public enum Val
    {
        Start,
        Stop
    }

    private void Update()
    {
        if (m_Exec)
        {
            m_Exec = false;
            Print(new Vector3(1.5f, 0.2f, 0.8f));
            Print(1);
            Print(Val.Start);
            Print(AnimationCurve.Linear(0f, 0f, 1f, 1f));
            return;
        }
    }

    void Print<T>(T value)
    {
        var t = typeof(T);
        if (t.IsPrimitive || t.IsValueType && !t.IsEnum)
        {
            var bytes = ToByte(value);
            var obj = ToValue(bytes, typeof(T));
            Debug.LogError(string.Format("{0}: {1} = {2}", value.GetType().Name, value, obj));
            return;
        }

        if(t.IsEnum)
        {
            return;
        }

        var json = value.ToString();
        Debug.LogError(json);
    }

    byte[] ToByte(object obj)
    {
        int size = Marshal.SizeOf(obj);

        byte[] bytes = new byte[size];

        //アンマネージメモリからメモリを割り当て
        IntPtr ptr = Marshal.AllocHGlobal(size);

        //マネージオブジェクトからアンマネージメモリにデータをマーシャリング
        Marshal.StructureToPtr(obj, ptr, false);

        //アンマネージデータをマネージのbyte[]にコピーする
        Marshal.Copy(ptr, bytes, 0, size);

        Marshal.FreeHGlobal(ptr);

        return bytes;
    }

    object ToValue(byte[] bytes, System.Type type)
    {
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        var obj = Marshal.PtrToStructure(ptr, type);
        Marshal.FreeHGlobal(ptr);
        return obj;
    }

    public static bool IsStruct(System.Type type)
    {
        return type.IsValueType &&  // 値型に限定（classを除外）
            !type.IsPrimitive &&    // intやfloatなどを除外
            !type.IsEnum;           // enumを除外（enumはValueTypeだけどPrimitiveではない）
    }

    UnityEngine.Object Find(int id, System.Type type)
    {
        foreach (var obj in Resources.FindObjectsOfTypeAll(type))
        {
            if (obj.GetInstanceID() == id)
                return obj;
        }
        return null;
    }

    private Type GetTypeFrom(string valueType)
    {
        var type = Type.GetType(valueType);
        if (type != null)
            return type;

        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //To speed things up, we check first in the already loaded assemblies.
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(valueType);
                if (type != null)
                    break;
            }
            if (type != null)
                return type;

            var loadedAssemblies = assemblies.ToList();

            foreach (var loadedAssembly in assemblies)
            {
                foreach (var referencedAssemblyName in loadedAssembly.GetReferencedAssemblies())
                {
                    var found = loadedAssemblies.All(x => x.GetName() != referencedAssemblyName);

                    if (!found)
                    {
                        try
                        {
                            var referencedAssembly = Assembly.Load(referencedAssemblyName);
                            type = referencedAssembly.GetType(valueType);
                            if (type != null)
                                break;
                            loadedAssemblies.Add(referencedAssembly);
                        }
                        catch
                        {
                            //We will ignore this, because the Type might still be in one of the other Assemblies.
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.ToString());
            //throw my custom exception    
        }

        if (type == null)
        {
            //throw my custom exception.
        }

        return type;
    }
}
