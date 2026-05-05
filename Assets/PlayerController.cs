using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform rightLanePos;
    public Transform leftLanePos;
    public Transform centerKneesPos;
    public Transform leftKneePos;
    public Transform rightKneePos;
    private float moveSpeed = 20f;
    public GameObject dashboardUI;
    public GameObject kneesUI;
    public RectTransform kneesRect;
    public CanvasGroup blinkCanvasGroup;

    public GameObject leftHandBrush;
    public GameObject rightHandBrush;
    public float handFollowSpeed = 10f;
    public Vector2 handOffsetRange = new Vector2(150f, 150f);

    public float blinkDuration = 0.08f;
    public Vector2 blinkInterval = new Vector2(3f, 10f);
    private float zoomAmount = 2.9f;
    private float kneeShiftAmount = 750f;

    public Transform steeringWheel;
    public float wheelRotationSpeed = 200f;
    public float maxWheelAngle = 30f;

    private Transform targetTransform;
    private Transform currentLane;
    private float nextBlinkTime;
    private Coroutine currentBlinkCoroutine;
    private float currentWheelAngle = 0f;

    void Start()
    {
        currentLane = rightLanePos;
        targetTransform = currentLane;
        nextBlinkTime = Time.time + Random.Range(blinkInterval.x, blinkInterval.y);
    }

    void Update()
    {
        HandleInputs();
        bool isLookingDown = (targetTransform == centerKneesPos ||
                              targetTransform == leftKneePos ||
                              targetTransform == rightKneePos);

        HandleSteering(isLookingDown);
        if (targetTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * moveSpeed);
        }
        if (dashboardUI != null) dashboardUI.SetActive(!isLookingDown);
        if (kneesUI != null) kneesUI.SetActive(isLookingDown);

        UpdateHandsLogic();
        if (kneesRect != null)
        {
            bool isZoomedKnee = (targetTransform == leftKneePos || targetTransform == rightKneePos);
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

        if (Time.time >= nextBlinkTime)
        {
            TriggerBlink();
            nextBlinkTime = Time.time + Random.Range(blinkInterval.x, blinkInterval.y);
        }
    }

    void UpdateHandsLogic()
    {
        if (leftHandBrush == null || rightHandBrush == null) return;
        bool isLeftZoom = (targetTransform == leftKneePos);
        bool isRightZoom = (targetTransform == rightKneePos);
        rightHandBrush.SetActive(isLeftZoom);
        leftHandBrush.SetActive(isRightZoom);
        float targetScale = (isLeftZoom || isRightZoom) ? zoomAmount : 1f;
        if (isLeftZoom)
        {
            RectTransform rt = rightHandBrush.GetComponent<RectTransform>();
            HandleHandMovement(rt);
            rt.localScale = Vector3.Lerp(rt.localScale, new Vector3(targetScale, targetScale, 1f), Time.deltaTime * moveSpeed);
        }
        if (isRightZoom)
        {
            RectTransform rt = leftHandBrush.GetComponent<RectTransform>();
            HandleHandMovement(rt);
            rt.localScale = Vector3.Lerp(rt.localScale, new Vector3(targetScale, targetScale, 1f), Time.deltaTime * moveSpeed);
        }
    }

    void HandleHandMovement(RectTransform handRect)
    {
        if (handRect == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        float distanceToHand = 0.95f;
        Vector3 screenPoint = new Vector3(mousePos.x, mousePos.y, distanceToHand);
        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        handRect.position = Vector3.Lerp(handRect.position, targetWorldPos, Time.deltaTime * handFollowSpeed);
    }

    void HandleSteering(bool autoCenter)
    {
        if (steeringWheel == null) return;
        float input = 0f;
        var keyboard = Keyboard.current;
        if (!autoCenter && keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input = -1f;
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input = 1f;
        }
        if (input != 0) currentWheelAngle -= input * wheelRotationSpeed * Time.deltaTime;
        else currentWheelAngle = Mathf.MoveTowards(currentWheelAngle, 0f, wheelRotationSpeed * 0.6f * Time.deltaTime);
        currentWheelAngle = Mathf.Clamp(currentWheelAngle, -maxWheelAngle, maxWheelAngle);
        steeringWheel.localRotation = Quaternion.Euler(0, 0, currentWheelAngle);
    }

    void HandleInputs()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        bool pressUp = keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame;
        bool pressDown = keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame;
        bool pressLeft = keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame;
        bool pressRight = keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame;
        if (pressUp || pressDown) TriggerBlink();
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

    public void TriggerBlink()
    {
        if (blinkCanvasGroup == null) return;
        if (currentBlinkCoroutine != null)
            StopCoroutine(currentBlinkCoroutine);
        currentBlinkCoroutine = StartCoroutine(DoBlink());
    }

    IEnumerator DoBlink()
    {
        float elapsed = 0;
        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            blinkCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / blinkDuration);
            yield return null;
        }
        yield return new WaitForSeconds(0.02f);
        elapsed = 0;
        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            blinkCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / blinkDuration);
            yield return null;
        }
    }
}