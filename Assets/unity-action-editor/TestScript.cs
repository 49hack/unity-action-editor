using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionEditor;

[CreateAssetMenu(menuName = "Action Editor/Test", fileName = "TestValueScript")]
public class TestScript : ScriptableObject
{
    [SerializeField] SharedInt m_Int;
}
