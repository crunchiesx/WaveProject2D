using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler OnCurrentWeaponChange;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;

    [Header("References")]
    [SerializeField] private Transform currentPlayerWeapon;

    [Space]
    [SerializeField] private LayerMask targetMask;

    private Rigidbody2D playerRb;
    private Damageable playerDamageable;

    private void Awake()
    {
        Instance = this;

        playerRb = GetComponent<Rigidbody2D>();
        playerDamageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        playerDamageable.OnDeath += Damageable_OnDeath;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            HandlePlayerMovement();
            HandlePlayerRotation();
        }
    }

    private void HandlePlayerMovement()
    {
        Vector2 moveDir = GameInput.Instance.GetMovementInputNormalized();

        bool isMoving = moveDir.magnitude > 0.1f;

        if (isMoving)
        {
            Vector2 forwardDir = transform.up;

            float dot = Vector2.Dot(moveDir, forwardDir);

            float currentSpeed = moveSpeed;

            if (dot < -0.1f)
            {
                currentSpeed *= 0.5f;
            }

            playerRb.MovePosition(playerRb.position + currentSpeed * Time.fixedDeltaTime * moveDir);
        }
    }

    private void HandlePlayerRotation()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 lookDir = mousePos - (Vector2)transform.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        playerRb.MoveRotation(Mathf.LerpAngle(playerRb.rotation, angle, Time.fixedDeltaTime * rotateSpeed));
    }

    public void SetCurrentPlayerWeapon(WeaponTypeSO weaponTypeSO)
    {
        if (playerDamageable.IsDead()) return;

        if (currentPlayerWeapon.GetComponent<PlayerWeapon>().GetWeaponTypeSO().weaponType != weaponTypeSO.weaponType)
        {
            Transform playerWeapon = currentPlayerWeapon;
            Transform newWeaponTransform = Instantiate(weaponTypeSO.weaponGameObject.transform, transform);

            currentPlayerWeapon = newWeaponTransform;

            playerWeapon.GetComponent<PlayerWeapon>().DestroyWeapon();

            OnCurrentWeaponChange?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Damageable_OnDeath(object sender, EventArgs e)
    {
        GameManager.Instance.SetGameOver();
    }

    public WeaponTypeSO GetCurrentPlayerWeaponTypeSO() => GetCurrentPlayerWeapon().GetComponent<PlayerWeapon>().GetWeaponTypeSO();

    public Transform GetCurrentPlayerWeapon() => currentPlayerWeapon;

    public LayerMask GetTargetMask() => targetMask;

    public Damageable GetPlayerDamageable() => playerDamageable;
}
