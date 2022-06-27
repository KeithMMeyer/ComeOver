using System.Collections.Generic;
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
        if (singleton != null)
            throw new UnityException();
        singleton = this;
    }

    public static Iml GetSingleton()
    {
        return singleton;
    }

    public static Vector3 To3dPosition(float x, float y, float z)
    {
        return new Vector3((float)(x / 200.0) - 2, (float)(y / 200.0) + 1, z);
    }

    public static Vector2 To2dPosition(Vector3 position)
    {
        return new Vector3(position.x, position.y);
    }
}

[XmlRoot(ElementName = "StructuralModel")]
public class StructuralModel
{
    [XmlAttribute]
    public string name;
    [XmlAttribute]
    public string conformsTo;
    [XmlAttribute]
    public string routingMode = "simpleRoute";
    [XmlArray("Classes")]
    [XmlArrayItem("Class")]
    public List<UserClass> classes;
    [XmlArray("Relations")]
    [XmlArrayItem("Relation")]
    public List<Relation> relations;

}
