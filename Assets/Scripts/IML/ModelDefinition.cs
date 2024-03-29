﻿using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot(ElementName = "iml")]
public class Iml
{
    [XmlAttribute]
    public string version = "0.1";
    [XmlElement(ElementName = "StructuralModel")]
    public StructuralModel structuralModel;

    private static Iml singleton;

    public Iml()
    {
		// Totally ruins the singleton pattern, but it's the only way to get the XML deserializer to work.
		singleton = this;
    }

    public static Iml GetSingleton()
    {
        return singleton;
    }

    public static Vector3 To3dPosition(float x, float y, float z)
    {
        return new Vector3((float)(x / 175.0) - 2, (float)(y / -175.0) + 3.5f, z);
    }

    public static Vector2 To2dPosition(Vector3 position)
    {
        return new Vector3((position.x + 2)* 175, (position.y - 3.5f)*-175);
    }
}

[XmlRoot(ElementName = "StructuralModel")]
public class StructuralModel
{
    [XmlAttribute]
    public string name = "IML_Structural_Model";
    [XmlAttribute]
    public string conformsTo = "IML Definition";
    [XmlAttribute]
    public string routingMode = "simpleRoute";
    [XmlArray("Classes")]
    [XmlArrayItem("Class")]
    public List<UserClass> classes;
    [XmlArray("Relations")]
    [XmlArrayItem("Relation")]
    public List<Relation> relations;

}
