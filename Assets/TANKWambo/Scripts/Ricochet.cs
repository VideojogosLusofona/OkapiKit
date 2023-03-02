using UnityEngine;

public class Ricochet : MonoBehaviour
{
    public float speed = 10f;
    public int maxBounces = 3;
    public LayerMask obstacleMask;

    private int bounces = 0;
    private Vector2 direction;

    void Start()
    {
        direction = transform.up;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        // Raycast to check for obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.1f, obstacleMask);

        if (hit.collider != null)
        {
            // Reflect the bullet's direction
            direction = Vector2.Reflect(direction, hit.normal);
            bounces++;

            if (bounces >= maxBounces)
            {
                Destroy(gameObject);
            }
        }
    }
}