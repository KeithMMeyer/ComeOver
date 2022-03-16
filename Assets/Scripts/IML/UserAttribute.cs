﻿using System.Xml.Serialization;
using UnityEngine;

[XmlRoot(ElementName = "Attribute")]
public class UserAttribute
{
    [XmlAttribute]
    public string name = "";
    [XmlAttribute]
    public string type = "STRING";
    [XmlAttribute]
    public string value = "";
    [XmlAttribute]
    public string visibility = "PUBLIC";
    [XmlAttribute]
    public string upperBound = "1";
    [XmlAttribute]
    public string lowerBound = "0";
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
        position = counter;

        Vector3 gamePosition = parent.gameObject.transform.position;
        gamePosition.x -= (5 * 0.05f);
        gamePosition.y -= counter * (1.5f * 0.05f) - height * 0.045f;

        gameObject.transform.position = gamePosition;

        gameObject.transform.parent = parent.gameObject.transform;

        gamePosition = gameObject.transform.localPosition;
        gamePosition.y = 0.001f;
        gameObject.transform.localPosition = gamePosition;

        gameObject.name = parent.name + " : " + name;

        Vector3 meshScale = gameObject.transform.GetChild(0).localScale;
        meshScale.x = width;
        gameObject.transform.GetChild(0).localScale = meshScale;
        gameObject.transform.GetChild(1).localPosition = new Vector3(-0.225f, 0, 0.06f);
        Vector3 textPos = gameObject.transform.GetChild(1).localPosition;
        textPos.x *= 20f * width;
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
        display += " " + name + " : " + type;
        display += value != null && !value.Equals("") ? " = " + value : "";

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