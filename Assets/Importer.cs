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
    public RelationXml[] relations;

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

    [XmlElement("Attribute")]
    public AttributeXml[] attributes = new AttributeXml[0];

    [XmlIgnore]
    public GameObject gameObject;


    public void createGameObject()
    {
        GameObject container = new GameObject("Classes");
        GameObject templateClass = Resources.Load<GameObject>("ClassObject");

        GameObject classObject = UnityEngine.Object.Instantiate(templateClass);
        classObject.transform.position = new Vector3((float)(x / 15.0) - 20, 0, (float)(y / 15.0) - 20);
        classObject.transform.GetChild(1).gameObject.GetComponent<TextMesh>().text = name;
        classObject.name = name;
        classObject.transform.GetChild(0).gameObject.AddComponent<Moveable>();
        gameObject = classObject;

        classObject.transform.parent = container.transform;

        int counter = 0;
        foreach (AttributeXml attribute in attributes)
        {
            attribute.createGameObject();
            attribute.attachToClass(this, counter++);
            
        }

        Debug.Log("Created object for " + name + " at " + classObject.transform.position);
    }

}

[XmlRoot(ElementName = "Attribute")]
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

    [XmlIgnore]
    public GameObject gameObject;

    public void createGameObject()
    {
        GameObject templateAttribute = Resources.Load<GameObject>("AttributeObject");

        GameObject attributeObject = UnityEngine.Object.Instantiate(templateAttribute);
        attributeObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMesh>().text = name;
        attributeObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<TextMesh>().text = value;

        attributeObject.name = name + " : " + name;

        gameObject = attributeObject;
    }

    public void attachToClass(ClassXml parent, int counter)
    {
        Vector3 position = parent.gameObject.transform.position;
        position.x -= 5;
        position.y += (float)0.1;
        position.z -= counter * (float)1.5;
        gameObject.transform.position = position;
        gameObject.transform.parent = parent.gameObject.transform;
    }
}


[XmlRoot(ElementName = "relation")]
public class RelationXml
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

    [XmlIgnore]
    public GameObject gameObject = new GameObject();
    [XmlIgnore]
    LineRenderer lineRenderer;

    public void setPoints(Vector3 start, Vector3 end)
    {
        gameObject.name = name != null ? name : source + " " + destination;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}

public class Importer
{
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