using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;
    [SerializeField] private AudioSource eatSound;
    private bool isResetting = false;

    private readonly List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;

    private void Start()
    {
        ResetState();
    }
private void Update()
{
    var keyboard = Keyboard.current;
    if (keyboard == null) return;

    if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame)
    {
        if (direction != Vector2Int.down)
            direction = Vector2Int.up;
    }
    else if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame)
    {
        if (direction != Vector2Int.up)
            direction = Vector2Int.down;
    }
    else if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
    {
        if (direction != Vector2Int.right)
            direction = Vector2Int.left;
    }
    else if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
    {
        if (direction != Vector2Int.left)
            direction = Vector2Int.right;
    }
}

    private void FixedUpdate()
    {
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero)
        {
            direction = input;
        }

        // Set each segment's position to be the same as the one it follows
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

       int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;

// 
if (Occupies(x, y))
{
    ResetState();
    return;
}

// Move the snake
transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
{
    isResetting = true;

    direction = Vector2Int.right;
    input = Vector2Int.zero;
    transform.position = Vector3.zero;

    for (int i = 1; i < segments.Count; i++)
    {
        if (segments[i] != null)
            Destroy(segments[i].gameObject);
    }

    segments.Clear();
    segments.Add(transform);

    for (int i = 0; i < initialSize - 1; i++)
    {
        Grow();
    }

    // re-enable triggers after a short delay
    StartCoroutine(ResetCooldown());
}

private IEnumerator ResetCooldown()
{
    yield return new WaitForSeconds(0.1f);
    isResetting = false;
}
    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (segment != null && 
                Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    if (isResetting) return;

    if (other.CompareTag("Apple"))
    {
        Grow();

        if (eatSound != null)
        {
            eatSound.Play();
        }

        Apple apple = other.GetComponentInParent<Apple>();
        if (apple != null)
        {
            apple.RandomizePosition();
        }
    }
    else if (other.CompareTag("Obstacle"))
    {
        ResetState();
    }
}
}