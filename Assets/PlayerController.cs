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
    public GameObject dashboardUI;
    public GameObject kneesUI;
    public RectTransform kneesRect;
    public float zoomAmount = 1.8f;
    public float kneeShiftAmount = 400f;
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
        bool isLookingDown = (targetTransform == centerKneesPos || targetTransform == leftKneePos || targetTransform == rightKneePos);
        bool isZoomedKnee = (targetTransform == leftKneePos || targetTransform == rightKneePos);
        if (dashboardUI != null) dashboardUI.SetActive(!isLookingDown);
        if (kneesUI != null) kneesUI.SetActive(isLookingDown);
        if (kneesRect != null)
        {
            float targetScale = isZoomedKnee ? zoomAmount : 1f;
            float targetX = 0f;
            if (isLookingDown)
            {
                if (targetTransform == leftKneePos) targetX = kneeShiftAmount;
                else if (targetTransform == rightKneePos) targetX = -kneeShiftAmount;
            }
            kneesRect.localScale = Vector3.Lerp(kneesRect.localScale, new Vector3(targetScale, targetScale, 1f), Time.deltaTime * moveSpeed);
            Vector2 targetPos = new Vector2(targetX, kneesRect.anchoredPosition.y);
            kneesRect.anchoredPosition = Vector2.Lerp(kneesRect.anchoredPosition, targetPos, Time.deltaTime * moveSpeed);
        }
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