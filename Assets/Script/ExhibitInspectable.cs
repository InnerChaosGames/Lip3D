using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRBaseInteractable))]
public class ExhibitInspectButton : MonoBehaviour
{
    [SerializeField] private InspectController inspectControllerOverride;
    [SerializeField] private GameObject exhibitPrefab; 
    [SerializeField] private string title;
    [SerializeField] private string description;
    
    private XRBaseInteractable _interactable;
    private InspectController _inspectController;

    private void Awake()
    {
        print(description);
        _interactable = GetComponent<XRBaseInteractable>();
        _inspectController = inspectControllerOverride != null
            ? inspectControllerOverride
            : FindObjectOfType<InspectController>();
    }

    private void OnEnable()
    {
        if (_interactable != null)
            _interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        if (_interactable != null)
            _interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs _)
    {
        print("Interactable entered");
        _inspectController?.TeleportToInspectRoom(exhibitPrefab, title, description);
    }
}