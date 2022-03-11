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

        float cos = Mathf.Cos(Mathf.Deg2Rad * (90 - angle));

        if (destinationClass.height == 0)
            destinationClass.resize();
        gameObject.transform.position = position + (destination - source).normalized * -((1 + (destinationClass.height + 3) * 0.125f) * 0.05f * 3.5f) / cos;

        gameObject.transform.Rotate(0, angle, 0, Space.Self);
    }
}
