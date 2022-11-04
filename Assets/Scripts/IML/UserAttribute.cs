using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot(ElementName = "Attribute")]
public class UserAttribute
{
    [XmlAttribute]
    public string visibility = "PUBLIC";
    [XmlAttribute]
    public string name;
    [XmlAttribute]
    public string type = "STRING";
    [XmlAttribute]
    public string value;
    [XmlAttribute]
    public string lowerBound = "0";
    [XmlAttribute]
    public string upperBound = "1";
    [XmlAttribute]
    public int position = 1;

    [XmlIgnore]
    public string displayString { get; private set; }
    [XmlIgnore]
    public GameObject gameObject;
    [XmlIgnore]
    public UserClass parent;

    public void CreateGameObject()
    {
        //GameObject templateAttribute = Resources.Load<GameObject>("AttributeObject");
        //GameObject attributeObject = UnityEngine.Object.Instantiate(templateAttribute);
        GameObject attributeObject = PhotonNetwork.Instantiate("AttributeObject", new Vector3(0, 0, 0), Quaternion.identity, 0);

        attributeObject.GetComponent<Identity>().attributeReference = this;
        attributeObject.name = "Attribute : " + name;
		attributeObject.tag = "IML";

        gameObject = attributeObject;
        gameObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);

        GenerateDisplayString();
    }

    public void AttachToClass(UserClass parent, int counter, int height, float width)
    {
        this.parent = parent;
        position = counter + 1;

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
            this.value = newValue;
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
        if (value != null && !value.Equals("")) {
            display += " = ";
            string seperator = type.Equals("STRING") ? "\",\"" : ",";
            int length = (value.Length - value.Replace(seperator, "").Length) / (type.Equals("STRING") ? 3 : 1) + 1; // fast way of counting length
            display += length < 5 ? value : type + "[" + length + "]";
        }

        displayString = display;

        if (gameObject != null)
        {
            gameObject.name = "Attribute : " + name;
            gameObject.transform.GetChild(1).gameObject.GetComponent<TextMesh>().text = display;
        }
        if(parent != null)
            parent.Resize();
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
        if (value == null || value.Equals(""))
            return value;
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
                    if(throwErrors && !ValidateValue(part))
                    {
                        message = "The entered value \"" + part + "\" is not a valid input for type " + type + ".";
                        return null;
                    }

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
                    if (throwErrors && !ValidateValue(part))
                    {
                        message = "The entered value \"" + part + "\" is not a valid input for type " + type + ".";
                        return null;
                    }
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
                    if (type.Equals("STRING") && (upperBound.Equals("*") || int.Parse(upperBound) > 1))
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
            if (value.Contains("\""))
                value = value.Substring(1, value.Length - 2);
            if (throwErrors && !ValidateValue(value))
            {
                message = "The entered value \"" + value + "\" is not a valid input for type " + type + ".";
                return null;
            }
            value = ConvertValue(value);
            if (value != null)
            {
                if (type.Equals("STRING") && (upperBound.Equals("*") || int.Parse(upperBound) > 1))
                {
                    value = "\"" + value + "\"";
                }
                value = PadValue(value, 1);
                return value;
            }
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
                if (((boolValues.IndexOf(value.ToUpper()) != -1 && boolValues.IndexOf(value.ToUpper()) < boolValues.Count / 2)) ||
                        (int.TryParse(value, out intTry) && intTry > 0) || (double.TryParse(value, out doubleTry) && doubleTry > 0))
                    return "TRUE";
                if (((boolValues.IndexOf(value.ToUpper()) != -1 && boolValues.IndexOf(value.ToUpper()) >= boolValues.Count / 2)) ||
                        (int.TryParse(value, out intTry) && intTry <= 0) || (double.TryParse(value, out doubleTry) && doubleTry <= 0))
                    return "FALSE";
                break;
            case "DOUBLE":
                if (int.TryParse(value, out _))
                    return value + ".0";
                if (double.TryParse(value, out _))
                    return value;
                if (value.Equals("TRUE"))
                    return "1.0";
                if (value.Equals("FALSE"))
                    return "0.0";
                break;
            case "INTEGER":
                if (int.TryParse(value, out _))
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

    private bool ValidateValue(string value)
    {
        string[] boolStrings = { "TRUE", "T", "FALSE", "F" };
        List<string> boolValues = boolStrings.ToList();

        switch (type)
        {
            case "STRING":
                return true;
            case "BOOLEAN":
                if (boolValues.IndexOf(value.ToUpper()) != -1)
                    return true;
                break;
            case "DOUBLE":
                if (int.TryParse(value, out _) || double.TryParse(value, out _))
                    return true;
                break;
            case "INTEGER":
                if (int.TryParse(value, out _))
                    return true;
                break;
        }
        return false;
    }

	[PunRPC]
	void DestroyAttribute()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}
