using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public string inputfile;

    // Start is called before the first frame update
    void Start()
    {
        IMLXml iml = Importer.ImportXml(inputfile);
        Debug.Log(iml.structuralModel.classes[0].attributes.Length);

        GenerateClasses(iml);

        foreach (RelationXml relation in iml.structuralModel.relations)
        {
            string source = relation.source;
            Vector3 start = new Vector3();
            foreach (ClassXml classXml in iml.structuralModel.classes)
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
            foreach (ClassXml classXml in iml.structuralModel.classes)
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateClasses(IMLXml iml)
    {
       
        foreach (ClassXml classXml in iml.structuralModel.classes)
        {
            classXml.createGameObject();
        }
    }
}
