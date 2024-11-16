using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;  // Referencia al Animator
    public float speed = 12f;
    public float Run = 1;

    public float jumpHeight = 3f;
    public float gravity = -9.8f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Vector3 velocity;
    public bool isGrounded;
    public bool isRun;

    // Variables para magia y agacharse
    public GameObject magicEffectPrefab; // Prefab de la magia
    public Transform magicSpawnPoint;

    private void Update()
    {
        // Comprobamos si el jugador est� tocando el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Si est� en el suelo, no aplicamos gravedad adicional
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reajustamos la velocidad vertical cuando est� en el suelo
        }

        // Movimiento del jugador
        MovePlayer();

        // Salto
        Jump();

        // Ataque de magia (Raycast)
        if (Input.GetKeyDown(KeyCode.F))
        {
            AttackMagic();
        }

        // Agacharse
        Crouch();
    }

    private void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift)) // Usamos GetKey para comprobar si la tecla se mantiene presionada
        {
            isRun = true;
            Run = 2;
        }
        else
        {
            isRun = false;
            Run = 1;
        }




        Vector3 move = transform.right * x + transform.forward * z;

        animator.SetFloat("X", x);
        animator.SetFloat("Z", z);
        // Animaci�n de caminar/correr
        if (x != 0 || z != 0)
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

        controller.Move(move * speed * Time.deltaTime * Run);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump"); // Trigger para animaci�n de salto
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void AttackMagic()
    {
        // Aqu� usamos un Raycast para disparar el ataque m�gico
        RaycastHit hit;
        if (Physics.Raycast(magicSpawnPoint.position, magicSpawnPoint.forward, out hit, 100f))
        {
            // Se puede hacer algo con el objeto que es impactado, por ejemplo, hacerle da�o
            Debug.Log("Magia impact� en: " + hit.collider.name);

            // Si deseas instanciar un prefab, lo puedes hacer aqu�
            Instantiate(magicEffectPrefab, hit.point, Quaternion.identity);
        }

        // Activar la animaci�n de ataque m�gico
        animator.SetTrigger("AttackMagic");
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            controller.height = 0.5f; // Reducir altura cuando el personaje se agacha
            animator.SetBool("isCrouching", true); // Activar animaci�n de agacharse
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            controller.height = 2f; // Volver a la altura normal
            animator.SetBool("isCrouching", false); // Desactivar animaci�n de agacharse
        }
    }
}
