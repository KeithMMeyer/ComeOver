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
        GameObject templateClass = Resources.Load<GameObject>("ClassObject");
        GameObject templateAttribute = Resources.Load<GameObject>("AttributeObject");
        foreach (ClassXml classXml in iml.structuralModel.classes)
        {
            GameObject classObject = Instantiate(templateClass);
            classObject.transform.position = new Vector3((float)(classXml.x / 20.0)-15, 0, (float)(classXml.y / 20.0)-15);
            classObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = classXml.name;
            classXml.gameObject = classObject;

            int counter = 0;
            foreach(AttributeXml attribute in classXml.attributes)
            {
                GameObject attributeObject = Instantiate(templateAttribute);
                attributeObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = attribute.name;
                Vector3 position = classObject.transform.position;
                position.z -= counter++ * (float) 1.1;
                attributeObject.transform.position = position;

                attributeObject.transform.parent = classObject.transform;
            }

            Debug.Log("Created object for " + classXml.name + " at " + classObject.transform.position);
        }

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
}
