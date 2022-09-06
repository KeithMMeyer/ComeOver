using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Main : MonoBehaviour, IPunObservable
{
    public string inputfile;
    Iml iml;
    public string modelName;
    public string routingMode;
    public bool doImport = true;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(iml.structuralModel.name);
            stream.SendNext(iml.structuralModel.routingMode);
            //Debug.Log("Writing!");
        }
        else
        {
            modelName = (string)stream.ReceiveNext();
            routingMode = (string)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        iml = Iml.GetSingleton();
        if (PhotonNetwork.IsMasterClient)
        {
            if (iml == null && doImport)
            {
                iml = Importer.ImportXml(inputfile);
            }
            else
            {
                if ((iml.structuralModel.classes.Count > 0 && iml.structuralModel.classes[0].gameObject != null))
                    return;
                if (iml == null && !doImport)
                {
                    iml = new Iml();
                    StructuralModel sm = new StructuralModel();
                    List<UserClass> classes = new List<UserClass>();
                    List<Relation> relations = new List<Relation>();
                    sm.classes = classes;
                    sm.relations = relations;
                    iml.structuralModel = sm;
                }

                Debug.LogWarning("Rerendering Iml.");
            }
        } else
        {
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
