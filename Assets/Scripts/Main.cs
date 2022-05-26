using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Main : MonoBehaviour
{
    public string inputfile;

    // Start is called before the first frame update
    public void Start()
    {
        Iml iml = Iml.GetSingleton();
        if (iml == null)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Iml", out object output))
            {
                iml = (Iml)output;
            } else {
                iml = Importer.ImportXml(inputfile);
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                hash.Add("Iml", iml);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            }
        } else
        {
            if (iml.structuralModel.classes.Count > 0 && iml.structuralModel.classes[0].gameObject != null)
                return;
        }

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
                        classXml.AddRelation(relation);
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
                        classXml.AddRelation(relation);
                        break;
                    }
                }
                relation.CreateGameObject();
                relation.AttachToClass(start, end);
            }
    }

    private void GenerateClasses(Iml iml)
    {
       
        foreach (UserClass classXml in iml.structuralModel.classes)
        {
            classXml.CreateGameObject();
        }
    }
    
}
