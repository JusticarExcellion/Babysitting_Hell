using UnityEngine;
using UnityEngine.Assertions;

public struct Mouse
{
    public Vector2 mousePos;
    public float mouseScrollDelta;

    public bool leftClicked;
    public bool middleClicked;
    public bool rightClicked;
}

public struct PlayerInput
{
    public Vector2 keyboardInput;
    public Mouse mouse;

    //NOTE: Specially handled keys
    public bool escPressed;
    public bool spacePressed;
    public bool enterPressed;
    public bool backSpacePressed;
}

public class UserInput : MonoBehaviour
{

    Transform MyTransform;

    [Header("Camera Settings")] // NOTE: Camera Parameters
    [SerializeField] private BoxCollider2D CameraBounds;
    private Camera SceneCamera;
    Transform CameraTransform;
    [HideInInspector] public float CurrentZoom;
    private float CameraScrollSpeed;
    private float ZoomSpeed;
    private float CameraSpeed;
    private float CameraMoveSpeed;
    public float MinCameraSize;
    public float MaxCameraSize;
    private float CameraBoundsWidth;
    private float CameraBoundsHeight;
    private float CameraZ;

    [Header("Physics Tests")]
    [SerializeField] private LayerMask InteractableLayer;
    [SerializeField] private LayerMask PlayableLayer;
    [SerializeField] private LayerMask WallLayer;
    [SerializeField] private Vector2 BoxcastSize;

    [Header("Player Character")] //NOTE: Reference to character script attached to the player character

    [SerializeField] private Character PC;

    void Start()
    {
        MyTransform = this.transform;
        //NOTE: Camera Parameters
        SceneCamera = Camera.main;
        SceneCamera.transform.position = new Vector3( PC.transform.position.x, PC.transform.position.y, SceneCamera.transform.position.z );
        CameraTransform = SceneCamera.transform;
        CameraZ = CameraTransform.position.z;

        CurrentZoom = SceneCamera.orthographicSize;
        CameraScrollSpeed = 1.0f; //Changes the size of the Camera
        ZoomSpeed = 2.0f;
        CameraSpeed = 4.0f;
        CameraMoveSpeed = 2.0f;

        MinCameraSize = 1.0f;
        MaxCameraSize = 15.0f;

        if( !CameraBounds )
        {
            Debug.LogError(" ERROR: CAMERA BOUNDS NOT DEFINED ");
            Debug.Break();
        }

        BoxcastSize = new Vector2( 2.0f , 2.0f );

        CameraBoundsWidth = CameraBounds.size.x;
        CameraBoundsHeight = CameraBounds.size.y;
    }

    void Update()
    {
        PlayerInput input = CaptureInput();
        HandleCameraMovement( in input );

        if( input.mouse.leftClicked )
        {

            Vector2 point = new Vector2( input.mouse.mousePos.x, input.mouse.mousePos.y );
            if( !Physics2D.OverlapPoint( point, PlayableLayer ) || Physics2D.OverlapPoint( point, WallLayer ))
            {
                //We didn't click on the playable surface
                return;
            }

            Vector3 newDestination = new Vector3( input.mouse.mousePos.x, input.mouse.mousePos.y, MyTransform.position.z );

            RaycastHit2D[] allHit = Physics2D.BoxCastAll( input.mouse.mousePos, BoxcastSize, 0, new Vector2(0,0),  Mathf.Infinity ,InteractableLayer );

            if( allHit.Length > 0 ) // NOTE: If we click close enough to a interactable we assume the player wants to click the interactable
            {
                //NOTE: Find the Closest Interactable to the mouse
                Transform ClosestToMouse = allHit[0].transform;
                foreach( RaycastHit2D hit in allHit )
                {
                    Vector3 mouseWorldCoordinates = new Vector3( input.mouse.mousePos.x, input.mouse.mousePos.y, 0 );
                    Vector3 direction = hit.transform.position - mouseWorldCoordinates;

                    Vector3 ClosestTransformToMouse = ClosestToMouse.position - mouseWorldCoordinates;

                    float Length = direction.magnitude;
                    float ClosestTransformLength = ClosestTransformToMouse.magnitude;

                    if( Length < ClosestTransformLength )
                    {
                        ClosestToMouse = hit.transform;
                    }
                }

                PC.SetNewTarget( ClosestToMouse );

                PC.SetNewPathToTarget( ClosestToMouse );
            }
            else
            {
                Vector2 mouseWorldCoordinates = new Vector2( input.mouse.mousePos.x, input.mouse.mousePos.y );
                PC.SetNewPath( mouseWorldCoordinates );
            }
        }
    }

    PlayerInput
    CaptureInput()
    {
        PlayerInput result = new PlayerInput();

        //NOTE: WASD/ Arrow-Keys

        if( Input.GetKey(KeyCode.W) )
        {
            result.keyboardInput.y = 1.0f;
        }
        else if( Input.GetKey(KeyCode.S) )
        {
            result.keyboardInput.y = -1.0f;
        }
        else if( Input.GetKey( KeyCode.UpArrow ))
        {
            result.keyboardInput.y = 1.0f;
        }
        else if( Input.GetKey( KeyCode.DownArrow ))
        {
            result.keyboardInput.y = -1.0f;
        }

        if( Input.GetKey(KeyCode.A) )
        {
            result.keyboardInput.x = -1.0f;
        }
        else if( Input.GetKey(KeyCode.D) )
        {
            result.keyboardInput.x = 1.0f;
        }
        else if( Input.GetKey( KeyCode.LeftArrow ))
        {
            result.keyboardInput.x = -1.0f;
        }
        else if( Input.GetKey( KeyCode.RightArrow ))
        {
            result.keyboardInput.x = 1.0f;
        }

        //NOTE: Mouse Input

        result.mouse.mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        result.mouse.mouseScrollDelta = Input.mouseScrollDelta.y;
        result.mouse.leftClicked = Input.GetMouseButtonDown( 0 );
        result.mouse.middleClicked = Input.GetMouseButtonDown( 1 );
        result.mouse.rightClicked = Input.GetMouseButtonDown( 2 );

        //NOTE: Special Keys

        result.escPressed = Input.GetKeyDown( KeyCode.Escape );
        result.spacePressed = Input.GetKeyDown( KeyCode.Space );
        result.enterPressed = Input.GetKeyDown( KeyCode.Return );
        result.backSpacePressed = Input.GetKeyDown( KeyCode.Backspace );

        return result;
    }

    void
    HandleCameraMovement( in PlayerInput input )
    {
        //NOTE: Camera Zoom
        CurrentZoom += (input.mouse.mouseScrollDelta * CameraScrollSpeed);
        if( CurrentZoom > MaxCameraSize )
        {
            CurrentZoom = MaxCameraSize;
        }
        else if( CurrentZoom < MinCameraSize)
        {
            CurrentZoom = MinCameraSize;
        }

        if( CurrentZoom != SceneCamera.orthographicSize )
        {
            SceneCamera.orthographicSize = Mathf.Lerp( SceneCamera.orthographicSize, CurrentZoom, Time.deltaTime * ZoomSpeed );
        }

        //TODO: This is ok but make sure to test this thoroughly towards the end of the semester
        if( CurrentZoom >  ( 2 * MaxCameraSize ) / 3 )
        {
            CameraBounds.size = new Vector2( (2 * CameraBoundsWidth) / 3, ( 2 * CameraBoundsHeight ) / 2 );
        }
        else
        {
            CameraBounds.size = new Vector2( CameraBoundsWidth , CameraBoundsHeight );
        }

        //NOTE: Camera can move freely around the bounds of the house

        float cameraX = CameraTransform.position.x;
        float cameraY = CameraTransform.position.y;

        cameraX += input.keyboardInput.x * CameraSpeed;
        cameraY += input.keyboardInput.y * CameraSpeed;

        float boundsWidth = CameraBounds.size.x;
        float boundsHeight = CameraBounds.size.y;

        float boundsLeft = CameraBounds.transform.position.x - boundsWidth/2;
        float boundsRight = CameraBounds.transform.position.x + boundsWidth/2;
        float boundsTop = CameraBounds.transform.position.y + boundsWidth/2;
        float boundsBottom = CameraBounds.transform.position.y - boundsWidth/2;

        if( cameraX < boundsLeft )
        {
            cameraX = boundsLeft;
        }
        else if( cameraX > boundsRight )
        {
            cameraX = boundsRight;
        }

        if( cameraY < boundsBottom )
        {
            cameraY = boundsBottom;
        }
        else if( cameraY > boundsTop )
        {
            cameraY = boundsTop;
        }

        Vector3 newCameraPosition = new Vector3( cameraX, cameraY, CameraZ);

        CameraTransform.position = Vector3.Lerp(CameraTransform.position, newCameraPosition, Time.deltaTime * CameraMoveSpeed );
    }

}
