using Photon.Pun;
using System.Collections.Generic;
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
        //GameObject templateRelation = Resources.Load<GameObject>("RelationObject");
        //GameObject relationObject = UnityEngine.Object.Instantiate(templateRelation);
        GameObject relationObject = PhotonNetwork.Instantiate("RelationObject", new Vector3(0, 0, 0), Quaternion.identity, 0);
        relationObject.transform.Rotate(new Vector3(-90, 0, 0));

        relationObject.GetComponent<Identity>().relationReference = this;

        gameObject = relationObject;

        if (!type.Equals("COMPOSITION"))
        {
            GameObject.Destroy(gameObject.transform.GetChild(1).GetChild(1).gameObject);
        }
        if (type.Equals("INHERITENCE"))
        {
            PhotonNetwork.Destroy(gameObject.transform.GetChild(2).gameObject);
            PhotonNetwork.Destroy(gameObject.transform.GetChild(2).gameObject);
        }

        BuildStrings();
    }

    public void AttachToClass(UserClass source, UserClass destination)
    {
        sourceClass = source;
        this.source = source.id;
        destinationClass = destination;
        this.destination = destination.id;
        if (!source.relations.Contains(this))
            source.relations.Add(this);
        if (!destination.relations.Contains(this))
            destination.relations.Add(this);
        Vector3 sourcePos = source.gameObject.transform.position;
        Vector3 destinationPos = destination.gameObject.transform.position;
        gameObject.name = name != null ? name : this.source + " " + this.destination;
        SetPoints(sourcePos, destinationPos);
    }

    public void BuildStrings()
    {
        if (type.Equals("INHERITENCE"))
            return;
        gameObject.transform.GetChild(2).GetComponent<TextMesh>().text = name;
        gameObject.transform.GetChild(3).GetComponent<TextMesh>().text = "[" + lowerBound + ".." + upperBound + "]";
    }

    public void SetPoints(Vector3 source, Vector3 destination)
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
            Color color = type.Equals("REFERENCE") ? new Color(0, 0, 0) : type.Equals("COMPOSITION") ? new Color(0, 0, 0) : new Color(255, 255, 255);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            gameObject.transform.GetChild(0).GetChild(1).GetComponent<TextMesh>().color = color;
            if (type.Equals("COMPOSITION"))
            {
                gameObject.transform.GetChild(1).GetChild(1).GetComponent<TextMesh>().color = color;
            }
        }
        lineRenderer.positionCount = 2;

        if (source.Equals(destination))
            source.x += (destinationClass.width * 0.05f * 100 * 0.95f) * 2;
        source.z += 0.01f;
        destination.z += 0.01f;
        lineRenderer.SetPosition(0, source);
        lineRenderer.SetPosition(1, destination);

        lineRenderer.sortingOrder = -1;
        PlaceObjects(source, destination);
        //Debug.Log("Drew relation from " + source + " to " + destination);
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
            if (source.Equals(destination))
                point.x += (destinationClass.width * 0.05f * 100 * 0.95f) * 2;
            lineRenderer.SetPosition(1, point);
        }
        PlaceObjects(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
    }

    private void PlaceObjects(Vector3 source, Vector3 destination)
    {
        Transform arrow = gameObject.transform.GetChild(0);
        Transform block = gameObject.transform.GetChild(1);

        source.z -= 0.01f;
        destination.z -= 0.01f;

        float angle = Vector3.Angle(destination - source, new Vector3(1, 0, 0));

        block.position = source + CalculateOffset(sourceClass, (source - destination), angle, 0);
        block.localRotation = Quaternion.identity;
        block.Rotate(0, Mathf.Sign(destination.y - source.y) * (180 - angle), 0, Space.Self);

        arrow.position = destination + CalculateOffset(destinationClass, (destination - source), angle, 0);
        arrow.localRotation = Quaternion.identity;
        arrow.Rotate(0, Mathf.Sign(source.y - destination.y) * angle, 0, Space.Self);

        if (!type.Equals("INHERITENCE"))
        {
            Transform name = gameObject.transform.GetChild(2);
            Transform bounds = gameObject.transform.GetChild(3);
            Vector3 namePosition = source + (destination - source) / 2;
            name.position = namePosition + 0.1f * Vector3.Cross((source - destination).normalized, new Vector3(0, 0, 1));

            Vector3 boundPosition;
            Vector3 offset = CalculateOffset(destinationClass, (destination - source), angle, 0.25f);
            boundPosition = destination + offset;
            bounds.position = boundPosition + 0.15f * Vector3.Cross((source - destination).normalized, new Vector3(0, 0, 1));
        }

    }

    private Vector3 CalculateOffset(UserClass target, Vector3 line, float angle, float padding)
    {
        if (target == null)
            return new Vector3(0, 0, 0);
        float cosA = Mathf.Cos(Mathf.Deg2Rad * (90 - angle));
        float cosB = Mathf.Cos(Mathf.Deg2Rad * angle) * -Mathf.Sign(90 - angle);

        if (target.height == 0)
            target.Resize();
        Vector3 position = target.gameObject.transform.position;
        float up = ((1 + (target.height + 3) * 0.125f) * 0.05f * 3.5f);
        float right = (target.width * 0.05f * 100 * 0.95f);
        Vector3 verticalOffset = line.normalized * (-up / cosA + -Mathf.Sign(cosA) * padding);
        Vector3 horizontalOffset = line.normalized * (right / cosB + Mathf.Sign(cosB) * padding);

        Vector3 corner = position;
        corner.y += up;
        corner.x += right;
        float cornerAngle = Vector3.Angle(position - corner, new Vector3(1, 0, 0));
        //verticalOffset = (destination - source).magnitude > verticalOffset.magnitude ? verticalOffset : -(destination - source) / 2;
        if ((angle > 90 && angle > cornerAngle) || (angle < 90 && 180 - cornerAngle > angle))
        {
            return horizontalOffset;
        }
        else
        {
            return verticalOffset;
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

    public bool CanAttach(UserClass src, UserClass dest, out string message)
    {
        message = "";
        if (src.id.Equals(source) && dest.id.Equals(destination))
            return true;

        if (type.Equals("INHERITENCE"))
        {
            if (src.Equals(dest))
            {
                message = "An inheritance relation cannot have the same source and target.";
                //return false;
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (Relation r in Iml.GetSingleton().structuralModel.relations)
            {
                if (r.type.Equals("INHERITENCE") && r != this)
                {
                    if (src.id.Equals(r.source))
                    {
                        message = "IML does not allow multiple inheritances; a class can only inherit from one other class. (" + src.name + ")";
                        return false;
                    }
                    dict.Add(r.source, r.destination);
                }
            }
            string target = dest.id;
            bool found = true;
            while (found)
            {
                if (target.Equals(src.id))
                {
                    message = "Adding this relation would create a cycle of inheritance; insertion aborted.";
                    return false;
                }
                found = dict.TryGetValue(target, out target);
            }

        }
        if (name != null)
        {
            Relation other = src.FindRelation(name);
            if (src.FindAttribute(name) != null || (other != null && other != this))
            {
                message = "Changing this relation would result a duplicate name for attributes and/or relations; update aborted.";
                return false;
            }
        }
    message = "";
        return true;
    }
}
