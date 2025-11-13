using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InspectController : MonoBehaviour
{
    [Header("Rotation reference")]
    [SerializeField] private Transform rotationReference; // usually camera or XR Origin
    [Header("Inspection room")]
    [SerializeField] private Transform inspectStation;   // where XR Origin should stand in the inspect room
    [SerializeField] private Transform inspectSpawnPoint;   // where the miniature is placed
    
    [Header("Miniature size")]
    [SerializeField] private float targetHeight = 0.6f;     // final height in meters
    
    [Header("Locomotion (optional)")]
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private ActionBasedSnapTurnProvider snapTurnProvider;
    [SerializeField] private TeleportationProvider teleportProvider;

    [Header("Input (sticks)")]
    [SerializeField] private InputActionProperty rotateAction; // right stick (Vector2)
    [SerializeField] private InputActionProperty zoomAction;   // left stick (Vector2 or float Y)

    [Header("Zoom settings")]
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float minScaleMultiplier = 0.5f;
    [SerializeField] private float maxScaleMultiplier = 2.5f;
    
    [Header("Exit input")]
    [SerializeField] private InputActionProperty exitInspectAction; // e.g. Y/B button

    private Vector3 _savedPosition;
    private Quaternion _savedRotation;
    private bool _isInspecting;
    private GameObject _currentClone;

    private Vector3 _baseScale;
    private float _zoomScale = 1f;
    
    private void Reset()
    {
        // Try auto-grabbing locomotion providers from this object / children
        moveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        snapTurnProvider = GetComponentInChildren<ActionBasedSnapTurnProvider>();
        teleportProvider = GetComponentInChildren<TeleportationProvider>();
    }

    private void OnEnable()
    {
        if (exitInspectAction.action != null)
            exitInspectAction.action.Enable();
        if (rotateAction.action != null)
            rotateAction.action.Enable();
        if (zoomAction.action != null)
            zoomAction.action.Enable();
    }

    private void OnDisable()
    {
        if (exitInspectAction.action != null)
            exitInspectAction.action.Disable();
        if (rotateAction.action != null)
            rotateAction.action.Disable();
        if (zoomAction.action != null)
            zoomAction.action.Disable();
    }

    private void Update()
    {
        if (!_isInspecting || _currentClone == null)
            return;

        // --- Rotate with right stick ---
        if (rotateAction.action != null)
        {
            Vector2 input = rotateAction.action.ReadValue<Vector2>();
            if (input.sqrMagnitude > 0.0001f)
            {
                float yaw = input.x * rotateSpeed * Time.deltaTime;
                float pitch = -input.y * rotateSpeed * Time.deltaTime;

                Transform refTransform = rotationReference != null ? rotationReference : inspectStation;

                if (refTransform != null)
                {
                    Vector3 yawAxis = refTransform.up;    // horizontal, around player "up"
                    Vector3 pitchAxis = refTransform.right; // vertical, around player "right"

                    Vector3 center = _currentClone.transform.position;

                    _currentClone.transform.RotateAround(center, yawAxis, yaw);
                    _currentClone.transform.RotateAround(center, pitchAxis, pitch);
                }
            }
        }

        if (zoomAction.action != null)
        {
            Vector2 input = zoomAction.action.ReadValue<Vector2>();
            float zoomDelta = input.y * zoomSpeed * Time.deltaTime;

            _zoomScale = Mathf.Clamp(_zoomScale + zoomDelta, minScaleMultiplier, maxScaleMultiplier);
            _currentClone.transform.localScale = _baseScale * _zoomScale;
        }
        
        if (exitInspectAction.action != null &&
            exitInspectAction.action.WasPerformedThisFrame())
        {
            TeleportBackToMuseum();
        }
    }
    
    public void TeleportToInspectRoom(GameObject source)
    {
        if (_isInspecting)
            return;

        if (inspectStation == null || inspectSpawnPoint == null)
        {
            Debug.LogWarning("InspectTeleportController: inspectStation / inspectSpawnPoint not assigned.");
            return;
        }

        if (source == null)
            return;

        SetLocomotionEnabled(false);
        
        // Save player pose in museum
        _savedPosition = transform.position;
        _savedRotation = transform.rotation;

        // Teleport XR Origin to the inspection room
        transform.SetPositionAndRotation(inspectStation.position, inspectStation.rotation);

        // Destroy previous clone if any
        if (_currentClone != null)
        {
            Destroy(_currentClone);
            _currentClone = null;
        }

        // Instantiate a copy of the exhibit at the spawn point
        _currentClone = Instantiate(source, inspectSpawnPoint.position, inspectSpawnPoint.rotation);

        // No stripping needed if it's just a plain model

        AutoScaleCloneToTargetHeight();

        // Store base scale for zoom scaling
        _baseScale = _currentClone.transform.localScale;
        _zoomScale = 1f;

        _isInspecting = true;
    }

    public void TeleportBackToMuseum()
    {
        if (!_isInspecting)
            return;

        
        // Destroy spawned miniature
        if (_currentClone != null)
        {
            Destroy(_currentClone);
            _currentClone = null;
        }

        // Restore player pose
        SetLocomotionEnabled(true);
        transform.SetPositionAndRotation(_savedPosition, _savedRotation);
        _isInspecting = false;
    }

    private void AutoScaleCloneToTargetHeight()
    {
        if (_currentClone == null)
            return;

        Transform root = _currentClone.transform;
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        float currentHeight = bounds.size.y;
        if (currentHeight <= 0.0001f)
            return;

        float scaleFactor = targetHeight / currentHeight;
        root.localScale = root.localScale * scaleFactor;
    }
    
    private void SetLocomotionEnabled(bool enabled)
    {
        //if (moveProvider != null)
        //    moveProvider.enabled = enabled;

        if (snapTurnProvider != null)
            snapTurnProvider.enabled = enabled;

        if (teleportProvider != null)
            teleportProvider.enabled = enabled;
    }
    
}