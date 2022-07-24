using UnityEngine;

public class HoveringEnemy : BaseEnemy
{
    [SerializeField] private float movementRangeX;

    private Vector2 movementRange;

    private void Awake()
    {
        movementRange = new Vector2(transform.position.x - movementRangeX,
            transform.position.x + movementRangeX);

        velocity = new Vector2(speed, 0f);
        spriteRenderer.flipX = true;
    }
    protected override void Move()
    {
        rb.velocity = velocity;

        float posX = transform.position.x;

        if (posX < movementRange.x)
        {
            velocity.x = speed;
            spriteRenderer.flipX = true;
        }
        else if (posX > movementRange.y)
        {
            velocity.x = -speed;
            spriteRenderer.flipX = false;
        }
    }
    protected override void OnCollisionEnter2D(Collision2D collision2D)
    {
        base.OnCollisionEnter2D(collision2D);
    }
}
