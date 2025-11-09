using UnityEngine;
using UnityEngine.InputSystem; // 새 Input System

public class PlayerController : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 100f;

    [Header("Shooting")]
    public GameObject bubblePrefab;   // Bubble 프리팹 할당
    public Transform firePoint;       // 입 위치(자식 Transform)
    public float fireInterval = 0.5f; // 0.5초 자동 발사
    public float bubbleSpeed = 8f;

    private float fireTimer = 0f;

    void Update()
    {
        HandleRotation();
        HandleAutoFire();
    }

    private void HandleRotation()
    {
        if (Keyboard.current == null) return;

        float rotationInput = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) rotationInput -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) rotationInput += 1f;

        transform.Rotate(Vector3.forward * -rotationInput * rotationSpeed * Time.deltaTime);
    }

    private void HandleAutoFire()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            FireBubble();
        }
    }

    private void FireBubble()
    {
        if (bubblePrefab == null || firePoint == null) return;

        Vector2 dir = transform.right;

        GameObject go = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        BubbleProjectile proj = go.GetComponent<BubbleProjectile>();

        if (proj != null)
        {
            proj.Launch(dir, bubbleSpeed);
        }
    }
}
