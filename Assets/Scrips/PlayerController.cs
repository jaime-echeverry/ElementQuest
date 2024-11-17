using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputManagerSO inputManagerSO;
    public CharacterController controller;
    public Animator animator;  // Referencia al Animator
    public float speed = 12f;
    public float Run = 1;

    private Vector3 directionMovement = Vector3.zero;
    private Vector3 movement = Vector3.zero;

    public float jumpHeight = 3f;
    public float gravity = -9.8f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Vector3 velocity;
    private Vector3 direction;
    public bool isGrounded;
    public bool isRun;
    private bool blockRotation = false;
    public new Transform camera;


    // Variables para magia y agacharse
    public GameObject magicEffectPrefab; // Prefab de la magia
    public Transform magicSpawnPoint;

    private void OnEnable()
    {
        inputManagerSO.OnJump += Jumping;
        inputManagerSO.OnMove += Move;
        inputManagerSO.OnAttack += Attack;
        inputManagerSO.OnCrouch += Crouch;
        inputManagerSO.OnCrouchCanceled += CrouchCanceled;
        inputManagerSO.OnRun += Running;
        inputManagerSO.OnRunCanceled += RunCanceled;
    }

    private void Move(Vector2 ctx)
    {
        direction = new Vector3(ctx.x, -9.8f, ctx.y);
    }

    private void Jumping()
    {
        Debug.Log("JUMP");
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump"); // Trigger para animación de salto
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Attack()
    {
        // Aquí usamos un Raycast para disparar el ataque mágico
        RaycastHit hit;
        if (Physics.Raycast(magicSpawnPoint.position, magicSpawnPoint.forward, out hit, 100f))
        {
            // Se puede hacer algo con el objeto que es impactado, por ejemplo, hacerle daño
            Debug.Log("Magia impactó en: " + hit.collider.name);

            // Si deseas instanciar un prefab, lo puedes hacer aquí
            Instantiate(magicEffectPrefab, hit.point, Quaternion.identity);
        }

        // Activar la animación de ataque mágico
        animator.SetTrigger("AttackMagic");
    }

    private void Crouch()
    {
        controller.height = 0.5f; // Reducir altura cuando el personaje se agacha
        animator.SetBool("isCrouching", true); // Activar animación de agacharse
    }

    private void CrouchCanceled()
    {
        controller.height = 2f; // Volver a la altura normal
        animator.SetBool("isCrouching", false); // Desactivar animación de agacharse
    }

    private void Running()
    {
        isRun = true;
        Run = 6;
    }
    private void RunCanceled()
    {
        isRun = false;
        Run = 3;
    }

    private void Update()
    {
        // Comprobamos si el jugador está tocando el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Si está en el suelo, no aplicamos gravedad adicional
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reajustamos la velocidad vertical cuando está en el suelo
        }

        BlockRotation();
        // Movimiento del jugador
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 forward = camera.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = camera.right;
        forward.y = 0;
        forward.Normalize(); ;

        Vector3 move = (right * direction.x + forward * direction.z);

        movement = move * speed * Time.deltaTime;
        movement.y += gravity * Time.deltaTime;

        animator.SetFloat("X", direction.x);
        animator.SetFloat("Z", direction.z);

        WalkToRun();

        controller.Move(movement);

        if ((move.x != 0 || move.z != 0) && !blockRotation)
        {
            transform.rotation = Quaternion.LookRotation(move).normalized;
        }
    }

    private void WalkToRun()
    {
        // Animación de caminar/correr
        if (direction.x != 0 || direction.z != 0)
        {
            animator.SetBool("Walk", true);
            if (isRun)
            {
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }

    private void BlockRotation()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            blockRotation = true;
        }
        if (direction.z > 0)
        {
            blockRotation = false;
        }
    }
}
