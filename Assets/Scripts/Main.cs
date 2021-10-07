using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public string inputfile;

    // Start is called before the first frame update
    void Start()
    {
        Iml iml = Importer.ImportXml(inputfile);
        Debug.Log(iml.structuralModel.classes[0].attributes.Count);

        GenerateClasses(iml);

        foreach (Relation relation in iml.structuralModel.relations)
        {
            string source = relation.source;
            Vector3 start = new Vector3();
            foreach (UserClass classXml in iml.structuralModel.classes)
            {
                if (classXml.id.Equals(source))
                {
                    start = classXml.gameObject.transform.position;
                    break;
                }
                start.y = -1;
            }

            string destination = relation.destination;
            Vector3 end = new Vector3();
            foreach (UserClass classXml in iml.structuralModel.classes)
            {
                if (classXml.id.Equals(destination))
                {
                    end = classXml.gameObject.transform.position;
                    break;
                }
            }
            start.y = -1;
            relation.setPoints(start, end);
        }
    }

    private void GenerateClasses(Iml iml)
    {
       
        foreach (UserClass classXml in iml.structuralModel.classes)
        {
            classXml.createGameObject();
        }
    }
}
