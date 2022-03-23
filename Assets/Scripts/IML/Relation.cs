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
    public string upperBound = "1";
    [XmlAttribute]
    public string lowerBound = "0";
    [XmlAttribute]
    public double nameDistance;
    [XmlAttribute]
    public double boundDistance;
    [XmlAttribute]
    public double nameOffset;
    [XmlAttribute]
    public double boundOffset;

    [XmlIgnore]
    public UserClass sourceClass;
    [XmlIgnore]
    public UserClass destinationClass;

    [XmlIgnore]
    public GameObject gameObject;
    [XmlIgnore]
    LineRenderer lineRenderer;

    public void CreateGameObject()
    {
        GameObject templateRelation = Resources.Load<GameObject>("RelationObject");

        GameObject relationObject = UnityEngine.Object.Instantiate(templateRelation);

        relationObject.GetComponent<Identity>().relationReference = this;

        gameObject = relationObject;

        if (type.Equals("INHERITENCE"))
        {
            GameObject.Destroy(gameObject.transform.GetChild(1).gameObject);
            GameObject.Destroy(gameObject.transform.GetChild(2).gameObject);
        }

        BuildStrings();
    }

    public void AttachToClass(UserClass source, UserClass destination)
    {
        sourceClass = source;
        destinationClass = destination;
        Vector3 sourcePos = source.gameObject.transform.position;
        Vector3 destinationPos = destination.gameObject.transform.position;
        sourcePos.z = 3.1f;
        destinationPos.z = 3.1f;
        gameObject.name = name != null ? name : this.source + " " + this.destination;
        SetPoints(sourcePos, destinationPos);
    }

    public void BuildStrings()
    {
        if (type.Equals("INHERITENCE"))
            return;
        gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = name;
        gameObject.transform.GetChild(2).GetComponent<TextMesh>().text = "[" + lowerBound + ".." + upperBound + "]";
    }

    public void SetPoints(Vector3 source, Vector3 destination)
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
        PlaceObjects();
        Debug.Log("Drew relation from " + source + " to " + destination);
    }

    public void UpdatePoints(Vector3? source, Vector3? destination)
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
        PlaceObjects();
    }

    private void PlaceObjects()
    {
        Transform arrow = gameObject.transform.GetChild(0);

        Vector3 source = lineRenderer.GetPosition(0);
        Vector3 destination = lineRenderer.GetPosition(1);
        source.z -= 0.01f;
        destination.z -= 0.01f;

        float angle = Vector3.Angle(destination - source, new Vector3(1, 0, 0));

        float cosA = Mathf.Cos(Mathf.Deg2Rad * (90 - angle));
        float cosB = Mathf.Cos(Mathf.Deg2Rad * angle) * -Mathf.Sign(90-angle);

        if (destinationClass.height == 0)
            destinationClass.Resize();
        float up = ((1 + (destinationClass.height + 3) * 0.125f) * 0.05f * 3.5f);
        float right = (destinationClass.width * 0.05f * 100 * 0.95f);
        Vector3 verticalOffset = (destination - source).normalized * -up / cosA;
        Vector3 horizontalOffset = (destination - source).normalized * right / cosB;

        Vector3 corner = destination;
        corner.y += up;
        corner.x += right;
        float cornerAngle = Vector3.Angle(destination - corner, new Vector3(1, 0, 0));
        //verticalOffset = (destination - source).magnitude > verticalOffset.magnitude ? verticalOffset : -(destination - source) / 2;
        if ((angle > 90 && angle > cornerAngle) || (angle < 90 && 180-cornerAngle > angle))
        {
            arrow.position = destination + horizontalOffset;
        } else
        {
            arrow.position = destination + verticalOffset;
        }
        Debug.Log(destinationClass.name + " " + angle + " : " + cornerAngle);
        arrow.localRotation = Quaternion.identity;
        arrow.Rotate(0, Mathf.Sign(source.y - destination.y) * angle, 0, Space.Self);

        if (!type.Equals("INHERITENCE"))
        {
            Transform name = gameObject.transform.GetChild(1);
            Transform bounds = gameObject.transform.GetChild(2);
            Transform block = gameObject.transform.GetChild(3);
            name.position = source + (destination - source) / 2;
            if ((angle > 90 && angle > cornerAngle) || (angle < 90 && cornerAngle > angle))
            {
                bounds.position = destination + 2 * horizontalOffset;
            } else
            {
                bounds.position = destination + 2 * verticalOffset;
            }
            name.GetComponent<TextMesh>().text = angle + ">" + cornerAngle;
            bounds.GetComponent<TextMesh>().text = angle + ">" + cornerAngle;
        }

    }

    public void UpdateBounds(string lower, string upper)
    {
        if (lower != null)
        {
            lowerBound = lower;
        }
        if (upper != null)
        {
            upperBound = upper;
        }
        BuildStrings();
    }

    public void SetName(string name)
    {
        this.name = name;
        BuildStrings();
    }
}
