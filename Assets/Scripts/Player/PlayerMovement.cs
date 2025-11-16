using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Properties
    [Header("Movement Settings")]
    public float moveSpeed = 5f;                // Velocidad de movimiento
    public Animator animator;                   // Referencia al Animator

    Vector3 _moveDirection;                     // Movimiento aplicado
    Vector2 _input;                             // Input recibido
    public float initialSpeed;
    public bool trapEffect = false;
    public int bewitched = 1;                   // 1 o -1 dependiendo de si ha sido hechizado por el Mago.


    [SerializeField, Range(0.1f, 10f)]
    [Tooltip("Sensibility of the rotation")]
    private float rotationSmoothness = 2f;

    [SerializeField]
    private GameObject[] characterModels;


    private bool _sprint;
    private bool _crouch;
    #endregion

    #region Monobehavior
    // Start is called before the first frame update
    void Awake()
    {
        _moveDirection = Vector3.zero;
        initialSpeed = moveSpeed;
        
    }

    void Start()
    {
        int index = CharacterSelection.Instance.selectedCharacterIndex;
        
        foreach(var model in characterModels)
            model.SetActive(false);

        if(index >= 0 && index < characterModels.Length)
            characterModels[index].SetActive(true);
    }

    void Update()
    {
        
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
        
        if (!_sprint && !_crouch && !trapEffect && moveSpeed != initialSpeed)
        {
            moveSpeed = initialSpeed;
        }
    }

    // Controlar animacion de movimiento del jugador
    void HandleAnimations()
    {
        float speed = Mathf.Abs(_input.x) + Mathf.Abs(_input.y);


        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
            animator.SetBool("isRunning", _sprint);
            animator.SetBool("isCrouch", _crouch);
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

    public void OnInventoryUsePressed(InputAction.CallbackContext ctx)
    {
        string name = ctx.control.name;
        PlayerInventory.instance.Use(ctx.control.name[0] - '1');
    }
    
    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (!_sprint && !_crouch && trapEffect)
        {
            moveSpeed *= 1.5f;
        }
        _sprint = ctx.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        if (!_crouch && !_sprint && trapEffect)
        {
            moveSpeed = initialSpeed;
            moveSpeed *= 0.5f;
        }  
        _crouch = ctx.ReadValueAsButton();
    }
    #endregion

    #region CollideFunction
    public void OnTriggerEnter(Collider other)
    {
        Collectable collectable;
        if (other != null)
        {
            collectable = other.GetComponent<Collectable>();
            if (collectable != null && !PlayerInventory.instance.IsFull())
            {
                PlayerInventory.instance.Add(collectable);
                Destroy(other.gameObject);
            }
        }
    }
    #endregion
}
