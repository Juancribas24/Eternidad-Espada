using System.Collections;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    private float speed = 5.0f;
    private float dashDistance = 1.5f;
    private float dashCooldown = 1f; // Cooldown en segundos

    private Rigidbody2D rb;
    private Vector2 moveVelocity;
    private Animator animator;
    private TrailRenderer tr;

    private bool isDashing;
    private float dashCooldownTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        tr = GetComponent<TrailRenderer>();
        tr.emitting = false;
        dashCooldownTimer = 0;
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.Normalize();
        moveVelocity = moveInput * speed;

        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        animator.SetFloat("Speed", moveVelocity.magnitude);

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (moveInput != Vector2.zero)
        {
            moveInput.Normalize();
            animator.SetFloat("UltimoX", moveInput.x);
            animator.SetFloat("UltimoY", moveInput.y);

            // Reproduce el sonido de pasos si es el momento adecuado
            //if (Time.time >= nextFootstepTime)
            //{
            //    footstepSound.clip = footstepClip;
            //  footstepSound.Play();
            //nextFootstepTime = Time.time + footstepDelay;
            //}
        }

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0)
        {
            StartCoroutine(PerformDash(moveInput));
            dashCooldownTimer = dashCooldown;
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
        }
    }

    private IEnumerator PerformDash(Vector2 direction)
    {
        isDashing = true;
        tr.emitting = true;

        Vector2 startDashPosition = rb.position;
        Vector2 dashPosition = rb.position + direction * dashDistance;

        float dashTime = 0.1f;
        float dashSpeed = Vector2.Distance(dashPosition, startDashPosition) / dashTime;

        float elapsedTime = 0;
        while (elapsedTime < dashTime)
        {
            rb.MovePosition(Vector2.Lerp(startDashPosition, dashPosition, elapsedTime / dashTime));
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.position = dashPosition;
        isDashing = false;
        tr.emitting = false;
    }
}
