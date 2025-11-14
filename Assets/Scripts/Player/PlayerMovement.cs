using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Properties
    [Header("Movement Settings")]
    public float moveSpeed = 5f;                // Velocidad de movimiento
    private Animator animator;                   // Referencia al Animator
    public Transform cameraTransform;           // Referencia a la c�mara

    Vector3 _moveDirection;                     // Movimiento aplicado
    Vector2 _input;                             // Input recibido
    public float initialSpeed;
    public int bewitched = 1;                   // 1 o -1 dependiendo de si ha sido hechizado por el Mago.


    [SerializeField, Range(0.1f, 10f)]
    [Tooltip("Sensibility of the rotation")]
    private float rotationSmoothness = 2f;
    #endregion

    #region Monobehavior
    // Start is called before the first frame update
    void Awake()
    {
        _moveDirection = Vector3.zero;
        initialSpeed = moveSpeed;
        //animator = GetComponent<Animator>();
        //cameraTransform = GameObject.Find("MainCamera").transform;
    }

    private void Start()
    {
        if(animator != null) 
            animator.SetBool("Grounded", true);
    }

    void Update()
    {
        //_moveDirection = (cameraTransform.forward * _input.y + cameraTransform.right * _input.x).normalized;
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        // Calculo del movimiento del personaje
        MovePlayer();
    }
    #endregion

    #region Player methods
    // Mover el jugador
    void MovePlayer()
    {
        //_moveDirection.y = 0f; // Asegurarnos de que el movimiento es horizontal (sin componente Y)

        _moveDirection = new Vector3(_input.x, 0f, _input.y).normalized;

        // Mover el jugador usando el Transform
        if (_moveDirection.magnitude > 0.1f)
        {
            // Rotación suave hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);

            // Movimiento en el espacio mundial
            transform.Translate(_moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    // Controlar animacion de movimiento del jugador
    void HandleAnimations()
    {
        float speed = Mathf.Abs(_input.x) + Mathf.Abs(_input.y);

        if (animator != null && animator.layerCount > 0)
        {
            animator.SetFloat("MoveSpeed", speed);
        }
    }
    //Asigna el valor correspondiente a la variable bewitched para invertir los controles.
    public void TriggerSpell()
    {
        bewitched *= -1;
    }
    #endregion

    #region Input actions

    // Funcion llamada automaticamente cuando se recibe un movimiento vertical u horizontal (WASD-Flechas)
    public void OnMove(InputAction.CallbackContext ctx)
    {
        _input = ctx.ReadValue<Vector2>() * bewitched; //Se guarda localmente
    }
    #endregion

    #region CollideFunction

    public void OnTriggerEnter(Collider other)
    {
        Collectable collectable;
        if (other != null)
        {
            collectable = other.GetComponent<Collectable>();
            if (collectable != null)
            {
                PlayerInventory.instance.Add(collectable);
                Destroy(collectable.gameObject);
            }
        }
    }
    #endregion
}
