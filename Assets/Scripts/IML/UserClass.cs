using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

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


    public void CreateGameObject()
    {
        GameObject container = new GameObject("Classes");
        GameObject templateClass = Resources.Load<GameObject>("ClassObject");

        GameObject classObject = UnityEngine.Object.Instantiate(templateClass);
        classObject.transform.position = Iml.To3dPosition(x, y, 3);
        classObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
        classObject.GetComponent<Identity>().classReference = this;

        gameObject = classObject;

        classObject.transform.parent = container.transform;

        SetName(name);
        SetAbstract(isAbstract.Equals("TRUE"));
        GenerateAttributes();
        Resize();
        GenerateAttributes();

        Debug.Log("Created object for " + name + " at " + classObject.transform.position);
    }
    public void Resize()
    {
        int height = Mathf.Clamp(attributes.Count, 3, 99) - 3;
        float width = 0;
        foreach (UserAttribute attribute in attributes)
        {
            if (attribute.gameObject == null)
            {
                attribute.CreateGameObject();
            }
            float newWidth = attribute.GetWidth();
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

        ReattachAttributes();
        //updateRelations();
    }

    private void GenerateAttributes()
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
            attribute.CreateGameObject();
            attribute.AttachToClass(this, counter++, height, width);
        }
    }

    private void ReattachAttributes()
    {
        int counter = 0;
        foreach (UserAttribute attribute in attributes)
        {
            attribute.AttachToClass(this, counter++, height, width);
        }
    }

    public void SetAbstract(bool isAbstract)
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

    public void SetName(string name)
    {
        this.name = name;
        if (gameObject != null)
        {
            gameObject.name = name;
            gameObject.transform.GetChild(1).gameObject.GetComponent<TextMesh>().text = name;
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (gameObject != null)
        {
            gameObject.transform.position = position;
        }
        Vector2 imlPostion = Iml.To2dPosition(position);
        x = Mathf.RoundToInt(imlPostion.x);
        y = Mathf.RoundToInt(imlPostion.y);
    }

    public void AddRelation(Relation target)
    {
        relations.Add(target);
    }

    public void UpdateRelations()
    {
        foreach (Relation relation in relations)
        {
            if (relation.source.Equals(id))
                relation.UpdatePoints(gameObject.transform.position, null);
            if (relation.destination.Equals(id))
                relation.UpdatePoints(null, gameObject.transform.position);
        }
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(UserClass))
            return false;
        UserClass other = (UserClass) obj;

        return id.Equals(other.id);
    }

    public override int GetHashCode()
    {
        return 1877310944 + EqualityComparer<string>.Default.GetHashCode(id);
    }
}
