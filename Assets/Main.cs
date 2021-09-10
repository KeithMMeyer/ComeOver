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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
