using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, Destructible
{
    [SerializeField]
    private InputManagerSO inputManagerSO;
    [SerializeField]
    public AudioSource spell;
    [SerializeField]
    public AudioSource steps;
    [SerializeField]
    Image lifeBar;
    public CharacterController controller;
    public Animator animator;  // Referencia al Animator
    public float speed = 20f;
    public float Run = 4;

    private Vector3 directionMovement = Vector3.zero;
    private Vector3 movement = Vector3.zero;

    public float jumpHeight = 6f;
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
    private float life;
    private float shootDistance = 100f;
    private float damage = 20f;
    private float maxLife=100;

    // Variables para magia y agacharse
    public GameObject magicEffectPrefab; // Prefab de la magia
    public Transform magicSpawnPoint;

    private void OnEnable()
    {
        inputManagerSO.OnJump += Jumping;
        inputManagerSO.OnMove += Move;
        inputManagerSO.OnMoveCanceled += MoveCanceled;
        inputManagerSO.OnAttack += Attack;
        inputManagerSO.OnCrouch += Crouch;
        inputManagerSO.OnCrouchCanceled += CrouchCanceled;
        inputManagerSO.OnRun += Running;
        inputManagerSO.OnRunCanceled += RunCanceled;
    }

    void Start()
    {
        life = maxLife;
    }

    private void Move(Vector2 ctx)
    {
        steps.Play();
        direction = new Vector3(ctx.x, -9.8f, ctx.y);
    }

    private void MoveCanceled(Vector2 ctx)
    {
        steps.Pause();
        direction = new Vector3(ctx.x, -9.8f, ctx.y);
    }

    private void Update()
    {
        // Comprobamos si el jugador est� tocando el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Si est� en el suelo, no aplicamos gravedad adicional
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reajustamos la velocidad vertical cuando est� en el suelo
        }

        BlockRotation();
        // Movimiento del jugador
        MovePlayer();
    }

    private void Jumping()
    {
        Debug.Log("JUMP");
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump"); // Trigger para animaci�n de salto
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Attack()
    {
        // Aqu� usamos un Raycast para disparar el ataque m�gico
        RaycastHit hit;
        spell.Play();
        if (Physics.Raycast(magicSpawnPoint.position, magicSpawnPoint.forward, out hit, shootDistance))
        {
            // Se puede hacer algo con el objeto que es impactado, por ejemplo, hacerle da�o
            Debug.Log("Magia impact� en: " + hit.collider.name);

            // Si deseas instanciar un prefab, lo puedes hacer aqu�
            Instantiate(magicEffectPrefab, hit.point, Quaternion.identity);

            //Mira si impacta en enemigo
            if (hit.transform.TryGetComponent(out Destructible damageSystem)) {
                damageSystem.getDamage(damage);
            }
        }

        // Activar la animaci�n de ataque m�gico
        animator.SetTrigger("AttackMagic");
    }

    private void Crouch()
    {
        controller.height = 0.5f; // Reducir altura cuando el personaje se agacha
        animator.SetBool("isCrouching", true); // Activar animaci�n de agacharse
    }

    private void CrouchCanceled()
    {
        controller.height = 2f; // Volver a la altura normal
        animator.SetBool("isCrouching", false); // Desactivar animaci�n de agacharse
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
        // Animaci�n de caminar/correr
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

    public void getDamage(float damage)
    {
        life -= damage;
        lifeBar.fillAmount = life / maxLife;

        if (life <= 0) {
            animator.SetBool("Die", true);
            SceneManager.LoadScene("Menu");
        }
    }
}
