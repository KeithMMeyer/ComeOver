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

    private static Iml singleton;

    public Iml()
    {
        if (singleton != null)
            throw new UnityException();
        singleton = this;
    }

    public static Iml getSingleton()
    {
        return singleton;
    }

    public static Vector3 to3dPosition(float x, float y, float z)
    {
        return new Vector3((float)(x / 200.0) - 2, (float)(y / 200.0) + 1, z);
    }

    public static Vector2 to2dPosition(Vector3 position)
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
    public string isAbstract = "FALSE";
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
        classObject.transform.position = Iml.to3dPosition(x, y, 3);
        classObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
        classObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        classObject.GetComponent<Identity>().classReference = this;

        gameObject = classObject;

        classObject.transform.parent = container.transform;

        setName(name);
        setAbstract(isAbstract.Equals("TRUE"));
        generateAttributes();

        Debug.Log("Created object for " + name + " at " + classObject.transform.position);
    }

    public void generateAttributes()
    {
        attributes.RemoveAll(item => item == null);
        int size = resize();
        int counter = 0;
        foreach (UserAttribute attribute in attributes)
        {
            if (attribute.gameObject != null)
            {
                GameObject.Destroy(attribute.gameObject);
                attribute.gameObject = null;
            }
            attribute.createGameObject();
            attribute.attachToClass(this, counter++, size);

        }
    }

    private int resize()
    {
        int size = Mathf.Clamp(attributes.Count, 3, 10) - 3;
        Vector3 meshScale = new Vector3(2, 1, 1 + size * 0.125f);
        gameObject.transform.GetChild(0).transform.localScale = meshScale;
        Vector3 namePosition = new Vector3(-9, 0, 4 + size * 0.66f);
        gameObject.transform.GetChild(1).localPosition = namePosition;
        return size;
    }

    public void setAbstract(bool isAbstract)
    {
        this.isAbstract = isAbstract ? "TRUE" : "FALSE";

        if (isAbstract)
        {
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/AbstractColor");

        }
        else
        {
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/UIColor");
        }
    }

    public void setName(string name)
    {
        this.name = name;
        if (gameObject != null)
        {
            gameObject.name = name;
            gameObject.transform.GetChild(1).gameObject.GetComponent<TextMesh>().text = name;
        }
    }

    public void setPosition(Vector3 position)
    {
        if (gameObject != null)
        {
            gameObject.transform.position = position;
        }
        Vector2 imlPostion = Iml.to2dPosition(position);
        x = Mathf.RoundToInt(imlPostion.x);
        y = Mathf.RoundToInt(imlPostion.y);
    }

    public void addRelation(Relation target)
    {
        relations.Add(target);
    }

    public void updateRelations()
    {
        foreach (Relation relation in relations)
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
    public string type;
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
    [XmlIgnore]
    public UserClass parent;

    public void createGameObject()
    {
        GameObject templateAttribute = Resources.Load<GameObject>("AttributeObject");

        GameObject attributeObject = UnityEngine.Object.Instantiate(templateAttribute);
        attributeObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMesh>().text = name;
        attributeObject.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMesh>().text = value;

        attributeObject.GetComponent<Identity>().attributeReference = this;
        attributeObject.name = "Attribute : " + name;

        gameObject = attributeObject;
        gameObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
        gameObject.transform.localScale = new Vector3((float)0.05, (float)0.05, (float)0.05);
    }

    public void attachToClass(UserClass parent, int counter, int size)
    {
        this.parent = parent;
        Vector3 position = parent.gameObject.transform.position;
        position.x -= (5 * 0.05f);
        position.y -= counter * (1.5f * 0.05f) - size * 0.045f;

        gameObject.transform.position = position;

        gameObject.transform.parent = parent.gameObject.transform;

        position = gameObject.transform.localPosition;
        position.y = 0.05f;
        gameObject.transform.localPosition = position;

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
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, source);
        lineRenderer.SetPosition(1, destination);
        Debug.Log("Drew relation from " + source + " to " + destination);
    }

    public void updatePoints(Vector3? source, Vector3? destination)
    {
        if (source.HasValue)
        {
            Vector3 point = source.Value;
            point.z += (float)0.1;
            lineRenderer.SetPosition(0, point);
        }
        if (destination.HasValue)
        {
            Vector3 point = destination.Value;
            point.z += (float)0.1;
            lineRenderer.SetPosition(1, point);
        }

    }
}

enum VisibilityType { Public_Type, Double_Type, Boolean_Type, String_Type };
enum AttributeType { Integer_Type, Double_Type, Boolean_Type, String_Type };
