using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class MenuTitle : System.Attribute
    {
        public string Name { get; private set; }

        public MenuTitle(string name)
        {
            Name = name;
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class CustomTrackEditor : System.Attribute
    {
        public System.Type Target { get; private set; }

        public CustomTrackEditor(System.Type type)
        {
            Target = type;
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class CustomClipEditor : System.Attribute
    {
        public System.Type Target { get; private set; }

        public CustomClipEditor(System.Type type)
        {
            Target = type;
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ParentTrack : System.Attribute
    {
        public System.Type Target { get; private set; }

        public ParentTrack(System.Type type)
        {
            Target = type;
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ParentSequence : System.Attribute
    {
        public System.Type Target { get; private set; }

        public ParentSequence(System.Type type)
        {
            Target = type;
        }
    }

    public class BindingType : System.Attribute
    {
        public System.Type Type { get; private set; }

        public BindingType(System.Type type)
        {
            Type = type;
        }
    }
}