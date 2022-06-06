using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Main : MonoBehaviour, IPunObservable
{
    public string inputfile;
    Iml iml;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(iml.structuralModel);
            //Debug.Log("Writing!");
        }
        else
        {
            //iml.structuralModel = (StructuralModel)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        iml = Iml.GetSingleton();
        if (PhotonNetwork.IsMasterClient)
        {
            if (iml == null)
            {
                iml = Importer.ImportXml(inputfile);
            }
            else
            {
                if ((iml.structuralModel.classes.Count > 0 && iml.structuralModel.classes[0].gameObject != null))
                    return;

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
