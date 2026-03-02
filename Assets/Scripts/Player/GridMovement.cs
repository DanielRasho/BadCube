using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections;

public class GridMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Animator animator;
    
    [Header("Animation")]
    [SerializeField] private float timeToMove = 0.2f;
    
    private bool isMoving;
    private Vector2 lastDir = Vector2.right; // default facing

    [Header("Tilemaps")] 
    [SerializeField] private Tilemap ground;
    [SerializeField] private Tilemap collision;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input != Vector2.zero)
        {
            Vector2 snapped = SnapToCardinal(input);
            lastDir = snapped;
        }

        // Always drive animator from lastDir, never reset to zero
        animator.SetBool("isWalking", isMoving);
        animator.SetFloat("InputX", lastDir.x);
        animator.SetFloat("InputY", lastDir.y);

        if (!isMoving)
            Move(input);
    }

    Vector2 SnapToCardinal(Vector2 raw)
    {
        if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
            return raw.x > 0 ? Vector2.right : Vector2.left;
        else
            return raw.y > 0 ? Vector2.up : Vector2.down;
    }

    void Move(Vector2 raw)
    {
        if (raw == Vector2.zero) return;

        Vector2Int dir = Vector2Int.RoundToInt(SnapToCardinal(raw));
        Vector3Int currentCell = ground.WorldToCell(transform.position);
        Vector3Int targetCell  = currentCell + (Vector3Int)dir;

        if (!ground.HasTile(targetCell)) return;
        if (collision.HasTile(targetCell)) return;

        Vector3 targetWorld = ground.CellToWorld(targetCell) + (Vector3)(ground.cellSize * 0.5f);
        StartCoroutine(SmoothMove(targetWorld));
    }

    private IEnumerator SmoothMove(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < timeToMove)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / timeToMove);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}
