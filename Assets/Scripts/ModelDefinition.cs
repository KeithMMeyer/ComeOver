using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot(ElementName = "iml")]
public class Iml
{
    [XmlAttribute]
    public string version;
    [XmlElement(ElementName = "StructuralModel")]
    public StructuralModel structuralModel;
}

[XmlRoot(ElementName = "StructuralModel")]
public class StructuralModel
{
    [XmlAttribute]
    public string name;
    [XmlAttribute]
    public string conformsTo;
    [XmlArray("Classes")]
    [XmlArrayItem("Class")]
    public List<UserClass> classes;
    [XmlArray("Relations")]
    [XmlArrayItem("Relation")]
    public List<Relation> relations;

}

[XmlRoot(ElementName = "Class")]
public class UserClass
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
    public List<UserAttribute> attributes = new List<UserAttribute>();

    [XmlIgnore]
    public List<Relation> relations = new List<Relation>();

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
        classObject.transform.GetChild(0).GetComponent<Moveable>().classReference = this;
        gameObject = classObject;

        classObject.transform.parent = container.transform;

        int counter = 0;
        foreach (UserAttribute attribute in attributes)
        {
            attribute.createGameObject();
            attribute.attachToClass(this, counter++);

        }

        Debug.Log("Created object for " + name + " at " + classObject.transform.position);
    }

    public void addRelation(Relation target) {
        relations.Add(target);
    }

    public void updateRelations()
    {
        foreach(Relation relation in relations)
        {
            if (relation.source.Equals(id))
                relation.updatePoints(gameObject.transform.position, null);
            if (relation.destination.Equals(id))
                relation.updatePoints(null, gameObject.transform.position);
        }
    }


}

[XmlRoot(ElementName = "Attribute")]
public class UserAttribute
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

        attributeObject.name = "Attribute : " + name;

        gameObject = attributeObject;
    }

    public void attachToClass(UserClass parent, int counter)
    {
        Vector3 position = parent.gameObject.transform.position;
        position.x -= 5;
        position.y += (float)0.1;
        position.z -= counter * (float)1.5;
        gameObject.transform.position = position;
        gameObject.transform.parent = parent.gameObject.transform;
        gameObject.name = parent.name + " : " + name;
    }
}


[XmlRoot(ElementName = "relation")]
public class Relation
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

    public void setPoints(Vector3 source, Vector3 destination)
    {
        gameObject.name = name != null ? name : this.source + " " + this.destination;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, source);
        lineRenderer.SetPosition(1, destination);
    }

    public void updatePoints(Vector3? source, Vector3? destination)
    {
        if (source.HasValue)
        {
            Vector3 point = source.Value;
            point.y -= 1;
            lineRenderer.SetPosition(0, point);
        }
        if (destination.HasValue)
        {
            Vector3 point = destination.Value;
            point.y -= 1;
            lineRenderer.SetPosition(1, point);
        }

    }
}

enum VisibilityType { Public_Type, Double_Type, Boolean_Type, String_Type };
enum AttributeType { Integer_Type, Double_Type, Boolean_Type, String_Type };
