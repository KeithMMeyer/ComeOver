using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
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

    public void CreateGameObject()
    {
        GameObject templateAttribute = Resources.Load<GameObject>("AttributeObject");

        GameObject attributeObject = UnityEngine.Object.Instantiate(templateAttribute);

        attributeObject.GetComponent<Identity>().attributeReference = this;
        attributeObject.name = "Attribute : " + name;

        gameObject = attributeObject;
        gameObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);

        GenerateDisplayString();
    }

    public void AttachToClass(UserClass parent, int counter, int height, float width)
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

    public void SetName(string name)
    {
        this.name = name;
        GenerateDisplayString();
    }

    public int GetTypeNumber()
    {
        string[] typeArray = { "STRING", "BOOLEAN", "DOUBLE", "INTEGER" };
        List<string> types = typeArray.ToList();
        return types.IndexOf(type);
    }

    public void SetType(int type)
    {
        string[] strings = { "STRING", "BOOLEAN", "DOUBLE", "INTEGER" };
        this.type = strings[type];
        value = ConvertValue(value, false, out _);
        GenerateDisplayString();
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
        value = ConvertValue(value, false, out _);
        GenerateDisplayString();
    }

    public string SetValue(string value)
    {
        string message;
        string newValue = ConvertValue(value, true, out message);
        if (newValue != null)
        {
            this.value = value;
            GenerateDisplayString();
            return null;
        }
        return message;
    }

    public void GenerateDisplayString()
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

    public float GetWidth()
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

    private string ConvertValue(string value, bool throwErrors, out string message)
    {
        message = null;
        if (value.Equals(""))
            return "";
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            List<string> values = new List<string>();
            if (value.Contains("\""))
            {
                string raw = value.Substring(2, value.Length - 4);
                string[] parts = raw.Split(new[] { "\",\"" }, System.StringSplitOptions.None);
                if (throwErrors && parts.Length < int.Parse(lowerBound))
                {
                    message = "The entered array " + value + " of size " + parts.Length + " has fewer attributes than the allowable lower bound of " + lowerBound + ".";
                    return null;
                }
                if (throwErrors && !upperBound.Equals("*") && parts.Length > int.Parse(upperBound))
                {
                    message = "The entered array " + value + " of size " + parts.Length + " has more attributes than the allowable upper bound of " + upperBound + ".";
                    return null;
                }

                foreach (string part in parts)
                {
                    string departed = ConvertValue(part);
                    if (departed != null)
                    {
                        values.Add(departed);
                        if (!(upperBound.Equals("*") || values.Count != int.Parse(upperBound)))
                            break;
                    }
                }
            }
            else
            {
                string raw = value.Substring(1, value.Length - 2);
                string[] parts = raw.Split(',');
                if (throwErrors && parts.Length < int.Parse(lowerBound))
                {
                    message = "The entered array " + value + " of size " + parts.Length + " has fewer attributes than the allowable lower bound of " + lowerBound + ".";
                    return null;
                }
                if (throwErrors && !upperBound.Equals("*") && parts.Length > int.Parse(upperBound))
                {
                    message = "The entered array " + value + " of size " + parts.Length + " has more attributes than the allowable upper bound of " + upperBound + ".";
                    return null;
                }

                foreach (string part in parts)
                {
                    string departed = ConvertValue(part);
                    if (departed != null)
                    {
                        values.Add(departed);
                        if (!(upperBound.Equals("*") || values.Count != int.Parse(upperBound)))
                            break;
                    }
                }
            }
            value = "";
            if (values.Count > 0)
            {
                foreach (string finalValue in values)
                {
                    if (type.Equals("STRING"))
                    {
                        value += ",\"" + finalValue + "\"";
                    }
                    else
                    {
                        value += "," + finalValue;
                    }
                }
                value = value.Substring(1, value.Length - 1);
                value = PadValue(value, values.Count);
                return value;
            }
        }
        else
        {
            value = ConvertValue(value);
            if (type.Equals("STRING"))
            {
                value = "\"" + value + "\"";
            }
            value = PadValue(value, 1);
            return value;
        }
        return null;
    }

    private string PadValue(string value, int currentLength)
    {
        if (upperBound.Equals("*") || int.Parse(upperBound) > 1)
        {
            if (int.Parse(lowerBound) > 1)
            {
                for (int i = currentLength; i < int.Parse(lowerBound); i++)
                {
                    if (type.Equals("STRING"))
                    {
                        value += "," + "\"\"";
                    }
                    else
                    {
                        value += "," + ConvertValue("FALSE");
                    }
                }
            }
            value = "[" + value + "]";
        }
        return value;
    }

    private string ConvertValue(string value)
    {
        string[] boolStrings = { "TRUE", "T", "FALSE", "F" };
        List<string> boolValues = boolStrings.ToList();

        int intTry;
        double doubleTry;
        switch (type)
        {
            case "STRING":
                return value;
            case "BOOLEAN":
                if ((boolValues.IndexOf(value.ToUpper()) != -1 && boolValues.IndexOf(value.ToUpper()) < boolValues.Count / 2) ||
                        (int.TryParse(value, out intTry) && intTry > 0) || (double.TryParse(value, out doubleTry) && doubleTry > 0))
                    return "TRUE";
                if ((boolValues.IndexOf(value.ToUpper()) != -1 && boolValues.IndexOf(value.ToUpper()) >= boolValues.Count / 2) ||
                        (int.TryParse(value, out intTry) && intTry <= 0) || (double.TryParse(value, out doubleTry) && doubleTry <= 0))
                    return "FALSE";
                break;
            case "DOUBLE":
                if (int.TryParse(value, out intTry))
                    return value + ".0";
                if (double.TryParse(value, out doubleTry))
                    return value;
                if (value.Equals("TRUE"))
                    return "1.0";
                if (value.Equals("FALSE"))
                    return "0.0";
                break;
            case "INTEGER":
                if (int.TryParse(value, out intTry))
                    return value;
                if (double.TryParse(value, out doubleTry))
                    return ((int)doubleTry).ToString();
                if (value.Equals("TRUE"))
                    return "1";
                if (value.Equals("FALSE"))
                    return "0";
                break;
        }
        return null;
    }

}
