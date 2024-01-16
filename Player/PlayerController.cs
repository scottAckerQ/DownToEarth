using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private GameController _gc;
    //Components
    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _sr;
    private Vector3 _currentMousePos;
    private Camera _cam;

    //how fast the player will move
    [SerializeField]
    private float _speed = 3f;

    [HideInInspector]
    public float playerEnergy = 100;

    private float _lastInteractTime;
    private const float InteractCooldown = .1f;

    private bool _isMouseActive;
    private float _lastMouseMovedTime;

    //keep track of which direction the character is facing
    private enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }
    private Directions _facing;


    private PlayerInventoryController _pInvControl;
    //Tools
    private const float ToolDistance = 2.5f; //how far away a tool should spawn when used
    public GameObject marker; //signifies where tools will be used
    public TileTarget tileTarget;
    public GameObject nowTargeting;

    void Start()
    {
        

        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _gc = GameController.Instance;
        _cam = Camera.main;

        _facing = Directions.Down;

        _pInvControl = GetComponent<PlayerInventoryController>();
        marker.SetActive(false);
        _lastInteractTime = 0;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (_gc.inMenu) return;
        if (_pInvControl.usingTool) return;
        if(DialogMaster_Scr.Instance.inDialog) return;
        
        Interact();
        Movement();
        Marker();
    }
    
    /// <summary>
    /// Reads info from the nowTargeting variable to determine what type of thing is being interacted with.
    /// If it's a tool pick up, the tool is added to the tool inventory.
    /// If it's a regular item, it is added to the main inventory.
    /// If it's a character, a dialogue box will open
    /// </summary>
    private void Interact()
    {
        if (!Input.GetButtonDown("Interact") || !tileTarget.TouchingObject()) return; //Check for button press conditions
        if (Time.realtimeSinceStartup - _lastInteractTime < InteractCooldown) return; //Check interact cooldown
        _lastInteractTime = Time.realtimeSinceStartup;
            
        //for every object in contact with the target marker
        for(int i = 0; i < tileTarget.GetCurrentTargets().Count; i++)
        {
            GameObject thisObject = tileTarget.GetCurrentTargets()[i];

            Interactable interactInterface = thisObject.GetComponent<Interactable>();
            interactInterface?.Interact(gameObject);

            //if this object is able to be picked up, remove if from the targeter and add it to inventory
            if (thisObject.GetComponent<PickUp>() != null && thisObject.GetComponent<PickUp>().GetActive())
            {
                if (!_pInvControl.InventoryHasSpace(thisObject))
                {
                    Debug.LogError("Interact: No space in inventory");
                }
                //tileTarget.GetCurrentTargets().Remove(thisObject);
                _pInvControl.AddToInventory(thisObject);
            }

        }
    }

    /// <summary>
    /// Movement controls for the player, based on the x and y axis
    /// </summary>
    private void Movement()
    {
        float xMove = Input.GetAxis("Horizontal");
        float yMove = Input.GetAxis("Vertical");

        //determine which way the player is facing
        if (Mathf.Abs(xMove) > Mathf.Abs(yMove))
        {
            if (xMove > 0.1)
            {
                _facing = Directions.Right;
            }
            else if (xMove < -0.1)
            {
                _facing = Directions.Left;
            }
        }
        else
        {
            if (yMove < -0.1)
            {
                _facing = Directions.Down;
            }
            else if (yMove > 0.1)
            {
                _facing = Directions.Up;
            }
        }

        float currentSpeedMod = ScaleManager_Scr.Instance.currentSize == ScaleManager_Scr.ObjectScales.Human ? 4 : 1;
        //move the player around
        Vector3 move = new Vector3(xMove, yMove, 0) * _speed * currentSpeedMod * Time.deltaTime;
        transform.Translate(move);
    }

    /// <summary>
    /// Determines where the character marker and tool spawn point exists
    /// KNOWN GLITCHES: Marker often appears in a strange position relative to player and marker itself
    /// </summary>
    private void Marker()
    {
        Cursor.visible = tileTarget.MouseIsActive();
        
        if(!tileTarget.MouseIsActive())
        {
            Transform tileTransform = tileTarget.transform;
            Vector3 playerPos = transform.position;
            
            tileTransform.position = _facing switch
            {
                Directions.Up => new Vector3(0, ToolDistance * 2) + playerPos,
                Directions.Down => new Vector3(0, -ToolDistance * 2) + playerPos,
                Directions.Left => new Vector3(-ToolDistance, 0) + playerPos,
                Directions.Right => new Vector3(ToolDistance, 0) + playerPos,
                _ => tileTransform.position
            };
        }
        else
        {
            tileTarget.MouseTarget(transform.position, 15f);
        }

        if (tileTarget.TouchingObject())
        {
            nowTargeting = tileTarget.GetCurrentTargets()[0];
            marker.SetActive(true);

            marker.transform.position = nowTargeting.transform.position;
        }
        else
        {
            marker.SetActive(false);
            nowTargeting = null;
        }
    }
}
