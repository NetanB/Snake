using UnityEngine;
using System.Collections;

public class Apple : MonoBehaviour
{
    public Collider2D gridArea;
    private Snake snake;

    private void Awake()
    {
        snake = Object.FindFirstObjectByType<Snake>();
    }

    private void Start()
    {
        if (snake == null)
        {
            snake = Object.FindFirstObjectByType<Snake>();
        }
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        if (gridArea == null || snake == null) return;
        
        Bounds bounds = gridArea.bounds;

        // Pick a random position inside the bounds
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Prevent the food from spawning on the snake
        int attempts = 0;
        while (snake.Occupies(x, y) && attempts < 100)
        {
            x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
            y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));
            attempts++;
        }

        transform.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Apple is eaten by snake - handled by Snake script
        // We don't need to do anything here since Snake calls RandomizePosition
    }
}