using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot(ElementName = "iml")]
public class IMLXml
{
    [XmlAttribute]
    public string version;
    [XmlElement(ElementName = "StructuralModel")]
    public StructuralModelXml structuralModel;
}

[XmlRoot(ElementName = "StructuralModel")]
public class StructuralModelXml
{
    [XmlAttribute]
    public string name;
    [XmlAttribute]
    public string conformsTo;
    [XmlArray("Classes")]
    [XmlArrayItem("Class")]
    public ClassXml[] classes;
    [XmlArray("Relations")]
    [XmlArrayItem("Relation")]
    public RelationXML[] relations;

}

[XmlRoot(ElementName = "Class")]
public class ClassXml
{
    [XmlAttribute]
    public string name;
    [XmlAttribute]
    public string isAbstract;
    [XmlAttribute]
    public int x;
    [XmlAttribute]
    public int y;
    [XmlAttribute]
    public string id;
    [XmlArrayAttribute()]
    [XmlArrayItem("attribute")]
    public AttributeXml[] attributes;

}

[XmlRoot(ElementName = "attribute")]
public class AttributeXml
{
    [XmlAttribute]
    public string name;
    [XmlAttribute]
    public string value;
    [XmlAttribute]
    public string visibility;
    [XmlAttribute]
    public string upperBound;
    [XmlAttribute]
    public string lowerBound;
    [XmlAttribute]
    public int position;

}

[XmlRoot(ElementName = "relation")]
public class RelationXML
{
    [XmlAttribute]
    public string name;
    [XmlAttribute]
    public string source;
    [XmlAttribute]
    public string destination;
    [XmlAttribute]
    public string type;
    [XmlAttribute]
    public string upperBound;
    [XmlAttribute]
    public string lowerBound;
    [XmlAttribute]
    public double nameDistance;
    [XmlAttribute]
    public int boundDistance;
    [XmlAttribute]
    public int nameOffset;
    [XmlAttribute]
    public int boundOffset;
}

public class Importer{
    public static IMLXml ImportXml(string file)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IMLXml));
            using (var stream = new FileStream(file, FileMode.Open))
            {
                return (IMLXml)serializer.Deserialize(stream);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception importing xml file: " + e);
            return default;
        }
    }
}