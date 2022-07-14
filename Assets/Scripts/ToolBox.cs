using Photon.Pun;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolBox : MonoBehaviour
{
    public UnityEvent closedWindows;
    public Camera viewCamera;
    public Recorder recorder;

    private PrimaryButtonWatcher primary;
    private SecondaryButtonWatcher secondary;
    public bool PrimaryPressed = false; // used to display button state in the Unity Inspector window
    public bool SecondaryPressed = false; // used to display button state in the Unity Inspector window
    public string relationMode;
    private bool micStatus = true;
    private bool speakerStatus = true;

    void Start()
    {
        relationMode = null;
        primary = GetComponent<PrimaryButtonWatcher>();
        secondary = GetComponent<SecondaryButtonWatcher>();
        primary.primaryButtonPress.AddListener(OnPrimaryButtonEvent);
        secondary.secondaryButtonPress.AddListener(OnSecondaryButtonEvent);
        closeAll();
        transform.parent.GetChild(1).gameObject.SetActive(true);
    }

    public void OnPrimaryButtonEvent(bool pressed)
    {
        closeAll();

        PrimaryPressed = pressed;

        if (pressed)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            relationMode = null;
        } else
        {
            transform.parent.GetChild(1).gameObject.SetActive(true);
        }

    }

    public void OnSecondaryButtonEvent(bool pressed)
    {
        SecondaryPressed = pressed;

        if (pressed)
        {
            closeAll();

            Transform editPanel = transform.GetChild(0).GetChild(4);
            editPanel.gameObject.SetActive(true);
            relationMode = null;
            InputField modelField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
            //modelField.onEndEdit.RemoveAllListeners();
            modelField.placeholder.GetComponent<Text>().text = Iml.GetSingleton().structuralModel.name;
            modelField.text = Iml.GetSingleton().structuralModel.name;
            modelField.onEndEdit.AddListener(delegate (string name) { Iml.GetSingleton().structuralModel.name = name; }); //likely need to add error checking

            InputField fileField = editPanel.GetChild(1).GetChild(1).GetComponent<InputField>();
            //modelField.onEndEdit.RemoveAllListeners();
            fileField.placeholder.GetComponent<Text>().text = Iml.GetSingleton().structuralModel.name + ".iml";
            fileField.text = Iml.GetSingleton().structuralModel.name + ".iml";
            //modelField.onEndEdit.AddListener(delegate (string name) { classReference.SetName(name); });

            string[] routeArray = { "simpleRoute", "orthogonalRoute", "manhattanRoute", "metroRoute" };
            List<string> routes = routeArray.ToList();
            Dropdown routeField = editPanel.GetChild(2).GetChild(1).GetComponent<Dropdown>();
            routeField.onValueChanged.RemoveAllListeners();
            routeField.value = routes.IndexOf(Iml.GetSingleton().structuralModel.routingMode);
            routeField.onValueChanged.AddListener(delegate (int route) { Iml.GetSingleton().structuralModel.routingMode = routeArray[route]; });

            Dropdown micField = editPanel.GetChild(3).GetChild(1).GetComponent<Dropdown>();
            micField.onValueChanged.RemoveAllListeners();
            micField.value = micStatus ? 0 : 1;
            micField.onValueChanged.AddListener(delegate (int mic) { micStatus = mic == 0; recorder.TransmitEnabled = micStatus; });

            Dropdown speakerField = editPanel.GetChild(3).GetChild(2).GetComponent<Dropdown>();
            speakerField.onValueChanged.RemoveAllListeners();
            speakerField.value = speakerStatus ? 0 : 1;
            speakerField.onValueChanged.AddListener(delegate (int speaker) { speakerStatus = speaker == 0; AudioListener.volume = speaker; });
        }
    }

    public void closeAll()
    {
        closedWindows.Invoke();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
        transform.parent.GetChild(1).gameObject.SetActive(false);
    }

    private void Update()
    {
        Vector3 lookAtPos = new Vector3(viewCamera.transform.position.x, viewCamera.transform.position.y, viewCamera.transform.position.z);
        transform.LookAt(lookAtPos);

        if (relationMode == null)
        {
            GameObject controller = GameObject.Find("RightHand Controller");
            Gradient blueGradient = new Gradient();
            blueGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );
            Gradient redGradient = new Gradient();
            redGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );
            controller.GetComponent<XRInteractorLineVisual>().validColorGradient = blueGradient;
            controller.GetComponent<XRInteractorLineVisual>().invalidColorGradient = redGradient;
        }
    }

    public void CreateObject(string type)
    {
        if (!PhotonNetwork.IsMasterClient && (type.Equals("CLASS") || type.Equals("ATTRIBUTE")))
        {
            PhotonView photonView = PhotonView.Get(GameObject.Find("Main"));
            photonView.RPC("CreateObject", RpcTarget.MasterClient, type);
        }
        if (type.Equals("CLASS"))
        {
            int num = 0;
            bool match = true;
            while (match)
            {
                num++;
                match = false;
                foreach (UserClass uc in Iml.GetSingleton().structuralModel.classes)
                    if (("NewClass" + num).Equals(uc.name))
                        match = true;
            }
            UserClass newClass = new UserClass();
            newClass.SetName("NewClass" + num);
            newClass.id = Guid.NewGuid().ToString();
            newClass.SetPosition(new Vector3(0, 0, 3));
            Iml.GetSingleton().structuralModel.classes.Add(newClass);
            newClass.CreateGameObject();

            //GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectEnter(GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>(), (IXRSelectInteractable) newClass.gameObject.transform.GetChild(0).GetComponent<XRSimpleInteractable>());
            //GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>().StartManualInteraction((IXRSelectInteractable)newClass.gameObject.transform.GetChild(0).GetComponent<XRSimpleInteractable>());
            //newClass.gameObject.transform.GetChild(0).GetComponent<Drag>().Grabbed(null);
        }
        else
        {
            if (type.Equals("ATTRIBUTE"))
            {
                UserAttribute newAttribute = new UserAttribute();
                newAttribute.name = "newAttr1";
                newAttribute.value = "";
                newAttribute.CreateGameObject();

                Vector3 position = Iml.To3dPosition(0, 0, 2.99f);
                newAttribute.gameObject.transform.position = position;
                Vector3 meshScale = newAttribute.gameObject.transform.GetChild(0).transform.localScale;
                meshScale.x = newAttribute.GetWidth();
                newAttribute.gameObject.transform.GetChild(0).transform.localScale = meshScale;

            }
            else
            {

                GameObject controller = GameObject.Find("RightHand Controller");
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
                );
                controller.GetComponent<XRInteractorLineVisual>().validColorGradient = gradient;
                controller.GetComponent<XRInteractorLineVisual>().invalidColorGradient = gradient;
                //normalMask = controller.GetComponent<XRRayInteractor>().raycastMask;
                //controller.GetComponent<XRRayInteractor>().raycastMask = LayerMask.NameToLayer("Classes");
                relationMode = type;

            }
        }
    }

    public void InsertRelation(UserClass classReference, SelectEnterEventArgs args)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(GameObject.Find("Main"));
            photonView.RPC("InsertRelation", RpcTarget.MasterClient, classReference.id, relationMode);
            GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectExit(args.interactorObject, args.interactableObject);
            relationMode = null;
        }
        int num = 0;
        bool match = true;
        while (match) {
            num++;
            match = false;
            foreach (Relation r in classReference.relations)
                if (("newRelation" + num).Equals(r.name))
                    match = true;
        }
        Relation relation = new Relation
        {
            name = relationMode.Equals("INHERITENCE") ? null : "newRelation" + num,
            type = relationMode
        };
        relation.CreateGameObject();
        relation.sourceClass = classReference;
        Vector3 other = classReference.gameObject.transform.position;
        other.x += 1;
        relation.SetPoints(classReference.gameObject.transform.position, other);
        //relation.AttachToClass(classReference, Iml.GetSingleton().structuralModel.classes[0]);
        if (args != null)
        {
            GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectExit(args.interactorObject, args.interactableObject);
            GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectEnter(
                args.interactorObject, relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<XRSimpleInteractable>());
            relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<EditRelation>().Init(); // so edit listeners will be called first
            relation.gameObject.GetComponent<LockView>().RequestLock(); // need to update for remote clients
            relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Drag>().Grabbed(args);
            relationMode = null;
        }

        return;
    }

}