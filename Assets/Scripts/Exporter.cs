using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Exporter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void Save()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Upload());
        } else
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("Save", RpcTarget.MasterClient);
        }
    }

    IEnumerator Upload()
    {
        string output = SerializeObject(Iml.GetSingleton());
        //output = output.Replace("type=\"INHERITENCE\" lowerBound=\"0\" upperBound=\"1\" nameDistance=\"0.5\" boundDistance=\"0.9\" nameOffset=\"30\" boundOffset=\"-30\" /", "type=\"INHERITENCE\" /");
        //output = output.Replace("value=\"\" ", "");
        output = output.Replace("\r", "");


        WWWForm form = new WWWForm();
        form.AddField("iml", output);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ceclnx01.cec.miamioh.edu/~meyerkm6/research/cse700/", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(www.error);
                Transform errorPanel = GameObject.Find("Main Canvas").transform.GetChild(1);
                errorPanel.gameObject.SetActive(true);
                errorPanel.GetChild(1).GetComponent<Text>().text = "Could not upload IML.";
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    public static string SerializeObject(Iml toSerialize)
    {
        //XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

        // Remove Declaration
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "\t",
            OmitXmlDeclaration = true
        };

        // Remove Namespace
        var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

        using (var stream = new StringWriter())
        using (var writer = XmlWriter.Create(stream, settings))
        {
            var serializer = new XmlSerializer(typeof(Iml));
            serializer.Serialize(writer, toSerialize, ns);
            return stream.ToString();
        }
    }
}
