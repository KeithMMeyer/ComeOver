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

    [XmlIgnore]
    public int height;
    [XmlIgnore]
    public float width;


    public void createGameObject()
    {
        GameObject container = new GameObject("Classes");
        GameObject templateClass = Resources.Load<GameObject>("ClassObject");

        GameObject classObject = UnityEngine.Object.Instantiate(templateClass);
        classObject.transform.position = Iml.to3dPosition(x, y, 3);
        classObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
        classObject.GetComponent<Identity>().classReference = this;

        gameObject = classObject;

        classObject.transform.parent = container.transform;

        setName(name);
        setAbstract(isAbstract.Equals("TRUE"));
        resize();

        Debug.Log("Created object for " + name + " at " + classObject.transform.position);
    }
    public void resize()
    {
        int height = Mathf.Clamp(attributes.Count, 3, 99) - 3;
        float width = 0;
        foreach (UserAttribute attribute in attributes)
        {
            if (attribute.gameObject == null)
            {
                attribute.createGameObject();
            }
            float newWidth = attribute.getWidth();
            width = newWidth > width ? newWidth : width;
        }
        width = Mathf.Clamp(width, 2 * 0.05f, 99);

        Vector3 meshScale = gameObject.transform.GetChild(0).transform.localScale;
        meshScale.x = width;
        meshScale.z = (1 + height * 0.125f) * 0.05f;
        gameObject.transform.GetChild(0).transform.localScale = meshScale;
        Vector3 namePosition = gameObject.transform.GetChild(1).localPosition;
        namePosition.x = -0.5f * 10.1f * width;
        namePosition.x += 0.05f;
        namePosition.z = (4 + height * 0.66f) * 0.05f;
        gameObject.transform.GetChild(1).localPosition = namePosition;
        this.height = height;
        this.width = width;

        generateAttributes();
    }

    private void generateAttributes()
    {
        attributes.RemoveAll(item => item == null);
        int counter = 0;
        foreach (UserAttribute attribute in attributes)
        {
            if (attribute.gameObject != null)
            {
                GameObject.Destroy(attribute.gameObject);
                attribute.gameObject = null;
            }
            attribute.createGameObject();
            attribute.attachToClass(this, counter++, height, width);

        }
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

        attributeObject.GetComponent<Identity>().attributeReference = this;
        attributeObject.name = "Attribute : " + name;

        gameObject = attributeObject;
        gameObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);

        generateDisplayString();
    }

    public void attachToClass(UserClass parent, int counter, int height, float width)
    {
        this.parent = parent;
        Vector3 position = parent.gameObject.transform.position;
        position.x -= (5 * 0.05f);
        position.y -= counter * (1.5f * 0.05f) - height * 0.045f;

        gameObject.transform.position = position;

        gameObject.transform.parent = parent.gameObject.transform;

        position = gameObject.transform.localPosition;
        position.y = 0.001f;
        gameObject.transform.localPosition = position;

        gameObject.name = parent.name + " : " + name;

        Vector3 meshScale = gameObject.transform.GetChild(0).transform.localScale;
        meshScale.x = width;
        gameObject.transform.GetChild(0).transform.localScale = meshScale;
        Vector3 textPos = gameObject.transform.GetChild(1).localPosition;
        textPos.x *= 20 * width;
        textPos.x += 0.225f;
        gameObject.transform.GetChild(1).localPosition = textPos;

    }

    public void setName(string name)
    {
        this.name = name;
        generateDisplayString();
    }

    public void setValue(string value)
    {
        this.value = value;
        generateDisplayString();
    }

    public void generateDisplayString()
    {
        string display = int.Parse(lowerBound) > 0 ? "■" : "□";
        int upper;
        if (!(int.TryParse(upperBound, out upper) && upper == 1))
        {
            display += "⃞   ";
        }
        else
        {
            display += "   ";
        }
        display += visibility.Equals("PUBLIC") ? "+" : visibility.Equals("PRIVATE") ? "-" : "#";
        display += " " + name + " : " + type + " = " + value;

        if (gameObject != null)
        {
            gameObject.name = "Attribute : " + name;
            gameObject.transform.GetChild(1).gameObject.GetComponent<TextMesh>().text = display;
        }
    }

    public float getWidth()
    {
        TextMesh mesh = gameObject.transform.GetChild(1).gameObject.GetComponent<TextMesh>();
        float width = 0;
        float bonus = 0;
        foreach (char symbol in mesh.text)
        {
            CharacterInfo info;
            if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
            {
                width += info.advance;
                bonus = info.advance;
            }
        }
        width += bonus * 4;
        return width * mesh.characterSize * 0.05f * 0.01f;
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
    UserClass sourceClass;
    [XmlIgnore]
    UserClass destinationClass;

    [XmlIgnore]
    public GameObject gameObject;
    [XmlIgnore]
    LineRenderer lineRenderer;

    public void createGameObject()
    {
        GameObject templateRelation = Resources.Load<GameObject>("RelationObject");

        GameObject relationObject = UnityEngine.Object.Instantiate(templateRelation);

        relationObject.GetComponent<Identity>().relationReference = this;

        gameObject = relationObject;

    }

    public void attachToClass(UserClass source, UserClass destination)
    {
        sourceClass = source;
        destinationClass = destination;
        Vector3 sourcePos = source.gameObject.transform.position;
        Vector3 destinationPos = destination.gameObject.transform.position;
        sourcePos.z = 3.1f;
        destinationPos.z = 3.1f;
        setPoints(sourcePos, destinationPos);
    }

    public void setPoints(Vector3 source, Vector3 destination)
    {
        gameObject.name = name != null ? name : this.source + " " + this.destination;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, source);
        lineRenderer.SetPosition(1, destination);
        placeObjects();
        Debug.Log("Drew relation from " + source + " to " + destination);
    }

    public void updatePoints(Vector3? source, Vector3? destination)
    {
        if (source.HasValue)
        {
            Vector3 point = source.Value;
            point.z += 0.1f;
            lineRenderer.SetPosition(0, point);
        }
        if (destination.HasValue)
        {
            Vector3 point = destination.Value;
            point.z += 0.1f;
            lineRenderer.SetPosition(1, point);
        }
        placeObjects();
    }

    private void placeObjects()
    {
        Vector3 source = lineRenderer.GetPosition(0);
        Vector3 destination = lineRenderer.GetPosition(1);
        Vector3 position = destination;
        position.z -= 0.1f;
        //position.y += ((1 + 3 * 0.125f) * 0.05f * 3.5f);

        float angle = Vector3.Angle(destination - source, new Vector3(1, 0, 0));

        float cos = Mathf.Cos(Mathf.Deg2Rad*(90-angle));

        if (destinationClass.height == 0)
            destinationClass.resize();
        gameObject.transform.position = position + (destination - source).normalized * -((1 + (destinationClass.height + 3) * 0.125f) * 0.05f * 3.5f)/cos;

        gameObject.transform.Rotate(0, angle, 0, Space.Self);
    }
}

enum VisibilityType { Public_Type, Double_Type, Boolean_Type, String_Type };
enum AttributeType { Integer_Type, Double_Type, Boolean_Type, String_Type };
