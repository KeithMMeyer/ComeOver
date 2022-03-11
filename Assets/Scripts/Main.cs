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

        GenerateClasses(iml);

        foreach (Relation relation in iml.structuralModel.relations)
        {
            string source = relation.source;
            UserClass start = null;
            foreach (UserClass classXml in iml.structuralModel.classes)
            {
                if (classXml.id.Equals(source))
                {
                    start = classXml;
                    classXml.addRelation(relation);
                    break;
                }
            }

            string destination = relation.destination;
            UserClass end = null;
            foreach (UserClass classXml in iml.structuralModel.classes)
            {
                if (classXml.id.Equals(destination))
                {
                    end = classXml;
                    classXml.addRelation(relation);
                    break;
                }
            }
            relation.createGameObject();
            relation.attachToClass(start, end);
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
