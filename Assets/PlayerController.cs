using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform rightLanePos;
    public Transform leftLanePos;
    public Transform centerKneesPos;
    public Transform leftKneePos;
    public Transform rightKneePos;
    public float moveSpeed = 20f;
    private Transform targetTransform;
    private Transform currentLane;

    void Start()
    {
        currentLane = rightLanePos;
        targetTransform = currentLane;
    }

    void Update()
    {
        HandleInputs();
        if (targetTransform == null) return;
        transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * moveSpeed);
    }

    void HandleInputs()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        bool pressUp = keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame;
        bool pressDown = keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame;
        bool pressLeft = keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame;
        bool pressRight = keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame;
        if (targetTransform == rightLanePos || targetTransform == leftLanePos)
        {
            if (pressLeft) targetTransform = leftLanePos;
            if (pressRight) targetTransform = rightLanePos;

            if (pressDown)
            {
                currentLane = targetTransform;
                targetTransform = centerKneesPos;
            }
        }
        else
        {
            if (pressUp) targetTransform = currentLane;
            if (pressLeft) targetTransform = leftKneePos;
            if (pressRight) targetTransform = rightKneePos;
            if (pressDown) targetTransform = centerKneesPos;
        }
    }
}