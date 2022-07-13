using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeAdapter : ObjectAdapter
{
    [PunRPC]
    public void PlacingAttribute(string classId, string attributeText)
    {
        transform.GetComponentInChildren<Drag>().PlacingAttribute(classId, attributeText);
    }

    [PunRPC]
    public void EditAttribute(string type, string value)
    {
        EditAttribute ea = transform.GetComponentInChildren<EditAttribute>();
        if (type.Equals("VISIBILITY"))
            ea.SaveVisibility(int.Parse(value));
        if (type.Equals("NAME"))
            ea.SaveName(value);
        if (type.Equals("TYPE"))
            ea.SaveType(int.Parse(value));
        if (type.Equals("LOWER"))
            ea.SaveLower(value);
        if (type.Equals("UPPER"))
            ea.SaveUpper(value);
        if (type.Equals("VALUE"))
            ea.SaveValue(value);
    }
}
