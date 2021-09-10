 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public class StructureModel
    {
        string Name { get; set; }
        string FileName { get; set; }
        string IsComposedOf { get; set; }
    }

    public class UserClass
    {
        string Name { get; set; }
        bool IsAbstract { get; set; }
    }

    public class Attribute
    {
        string Name { get; set; }
        int LowerBoundt { get; set; }
        int UpperBoundt { get; set; }
        AttributeType Type { get; set; }
    }

    public class Relation
    {
        string Name { get; set; }
        UserClass Source { get; set; }
        UserClass Destination { get; set; }
    }

    enum VisibilityType { Public_Type, Double_Type, Boolean_Type, String_Type };
    enum AttributeType { Integer_Type, Double_Type, Boolean_Type, String_Type };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
