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
        generateAttributes();
        resize();
        generateAttributes();

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
        
        reattachAttributes();
        //updateRelations();
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

    private void reattachAttributes()
    {
        int counter = 0;
        foreach (UserAttribute attribute in attributes)
        {
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
