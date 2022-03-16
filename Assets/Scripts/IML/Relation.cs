using System.Xml.Serialization;
using UnityEngine;

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
    public UserClass sourceClass;
    [XmlIgnore]
    public UserClass destinationClass;

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
        buildStrings();
    }

    public void attachToClass(UserClass source, UserClass destination)
    {
        sourceClass = source;
        destinationClass = destination;
        Vector3 sourcePos = source.gameObject.transform.position;
        Vector3 destinationPos = destination.gameObject.transform.position;
        sourcePos.z = 3.1f;
        destinationPos.z = 3.1f;
        gameObject.name = name != null ? name : this.source + " " + this.destination;
        setPoints(sourcePos, destinationPos);
    }

    public void buildStrings()
    {
        gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = name;
        gameObject.transform.GetChild(2).GetComponent<TextMesh>().text = "[" + lowerBound + ".." + upperBound + "]";
    }

    public void setPoints(Vector3 source, Vector3 destination)
    {
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        Color color = type.Equals("REFERENCE") ? new Color(255, 0, 0) : type.Equals("COMPOSITION") ? new Color(0, 255, 0) : new Color(0, 0, 255);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        gameObject.transform.GetChild(0).GetChild(1).GetComponent<TextMesh>().color = color;
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
            point.z += 0.01f;
            lineRenderer.SetPosition(0, point);
        }
        if (destination.HasValue)
        {
            Vector3 point = destination.Value;
            point.z += 0.01f;
            lineRenderer.SetPosition(1, point);
        }
        placeObjects();
    }

    private void placeObjects()
    {
        Transform arrow = gameObject.transform.GetChild(0);
        Transform name = gameObject.transform.GetChild(1);
        Transform bounds = gameObject.transform.GetChild(2);
        Transform block = gameObject.transform.GetChild(3);

        Vector3 source = lineRenderer.GetPosition(0);
        Vector3 destination = lineRenderer.GetPosition(1);
        source.z -= 0.01f;
        destination.z -= 0.01f;
        Vector3 position = destination;

        name.position = source + (destination - source) / 2;

        float angle = Vector3.Angle(destination - source, new Vector3(1, 0, 0));

        float cos = Mathf.Cos(Mathf.Deg2Rad * (90 - angle));

        if (destinationClass.height == 0)
            destinationClass.resize();
        Vector3 offset = (destination - source).normalized * -((1 + (destinationClass.height + 3) * 0.125f) * 0.05f * 3.5f) / cos;
        offset = (destination - source).magnitude > offset.magnitude ? offset : -(destination - source) / 2;
        arrow.position = position + offset;
        bounds.position = position + 2 * offset;

        arrow.localRotation = Quaternion.identity;
        arrow.Rotate(0, Mathf.Sign(source.y - destination.y) * angle, 0, Space.Self);

        //GameObject.Destroy(block.gameObject);
    }

    public void updateBounds(string lower, string upper)
    {
        if (lower != null)
        {
            lowerBound = lower;
        }
        if (upper != null)
        {
            upperBound = upper;
        }
        buildStrings();
    }

    public void setName(string name)
    {
        this.name = name;
        buildStrings();
    }
}
