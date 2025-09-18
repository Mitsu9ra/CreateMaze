using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveX, moveY);

        // •¨—ƒGƒ“ƒWƒ“‚É]‚Á‚½ˆÚ“®
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
}