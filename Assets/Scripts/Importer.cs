using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class Importer
{
    public static Iml ImportXml(string file)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Iml));
            using (var stream = new FileStream(file, FileMode.Open))
            {
                return (Iml)serializer.Deserialize(stream);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception importing xml file: " + e);
            return default;
        }
    }
}