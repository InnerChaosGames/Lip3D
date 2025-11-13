using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty pinchAnimationAction;
    [SerializeField] private InputActionProperty gripAnimationAction;
    [SerializeField] private Animator handAnimator;

    void Update()
    {
        var triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);
        var gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
}
