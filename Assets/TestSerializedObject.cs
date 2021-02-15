using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionEditor;

[CreateAssetMenu(menuName = "Action Editor/Test", fileName = "Test")]
public class TestSerializedObject : ScriptableObject
{
    [SerializeField] SharedFloatContext m_Private;
    public SharedFloatContext m_Public;
    [SerializeField] List<SharedFloatContext> m_List = new List<SharedFloatContext>();
    [SerializeField] SharedFloatContext[] m_Array = new SharedFloatContext[3];
}
