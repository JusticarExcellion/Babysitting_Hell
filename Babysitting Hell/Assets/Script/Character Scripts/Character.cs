using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    //TODO: We are overhauling the Interaction system to properly diversify between the player and the Children, Children perform activities and "interact" with objects, players will interact with interactable objects only available to the player, or other objects
    [HideInInspector] public Vector3 Destination;
    private WorldDecomposition WorldStatus;
    private List<Vector2> Path;
    private int PathIndex;

    [Header("Character States")]
    public bool Moving;
    public bool Interacting;
    public Transform Target;
    private Interaction TargetInteraction;
    [HideInInspector] public float InteractionTime;

    Transform MyTransform;
    [Header("Character Attributes")]
    [SerializeField] private float CharacterSpeed;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float SatisfiedDistance;
    [SerializeField] private float InteractionDistance;
    [SerializeField] private float SeekThreshold;

    [Header("Character AI Settings")]
    public bool AIControlled;
    [HideInInspector] public ChildAI_test AI;
    [HideInInspector] public int ChildNumber;

    [Header("UI Manager")]
    [HideInInspector] private UIManager UIManager;

    [Header("Holding Objects")]
    [SerializeField] private GameObject HoldingObject;
    private ScriptableHeldObject CurrentHeldObject;

    private float WaitTimer;
    private float CurrentWaitTimer;
    [Header("Physics Collision:")]
    private BoxCollider2D Collider;
    [SerializeField] private LayerMask CollidingLayer;
    private bool TargetOverride;
    private bool SwitchPath;

    void
    Start()
    {
        MyTransform = this.transform;
        UIManager = FindObjectOfType<UIManager>();
        WorldStatus = FindObjectOfType<WorldDecomposition>();
        Destination = Vector3.zero;
        Moving = false;
        CharacterSpeed = 5.0f;
        MoveSpeed = 1.0f;
        SatisfiedDistance = 0.5f;
        InteractionTime = 0.0f;
        WaitTimer = 2.0f;
        CurrentWaitTimer = 0.0f;
        CurrentHeldObject = null;
        PathIndex = 0;
        Path = null;
        Collider = this.GetComponent<BoxCollider2D>();
        TargetOverride = false;
        SwitchPath = false;
    }

    void
    Update()
    {
        CharacterMovement();
        TargetHandling();

        if(Destination != Vector3.zero )
        {
            Debug.DrawLine( MyTransform.position, Destination, Color.black );
        }

        if( AIControlled )
        {
            HandleAIDecisions();
        }

        HandleCharacterInteraction();
    }

    private void
    HandleAIDecisions()
    {
        if ( !Target ) // Then character is being controlled by the AI
        {
            if( CurrentWaitTimer > 0.0f )
            {
                CurrentWaitTimer -= ( 1 / CurrentWaitTimer ) * Time.deltaTime;
            }
            else
            {
                CurrentWaitTimer = WaitTimer;
                AI.InteractionDecision();
            }
        }
    }

    private void
    HandleCharacterInteraction()
    {
        if( Target )
        {
            Vector3 ToTarget = Target.position - MyTransform.position;
            if( ToTarget.magnitude < InteractionDistance && !Interacting ) // NOTE: If we are close enough to the object then we start interacting with it
            {
                Moving = false;
                TargetOverride = false;
                Interacting = true;

                if( AI ) AI.ResetWaitTimer();

                if( TargetInteraction.InteractableData )
                {
                    InteractionTime = TargetInteraction.InteractableData.InteractionTime;
                    UIManager.CreateProgressBar( MyTransform, InteractionTime );

                    if( TargetInteraction.InteractableData.ObjectType == InteractableType.Child )
                    {
                        Character Child = TargetInteraction.GetComponent<Character>();
                        Child.Moving = false;
                    }
                }
            }
        }

        if( Interacting )
        {
            if( InteractionTime > 0 )
            {
                InteractionTime -= Time.deltaTime;
            }
            else // NOTE: Character Interacting with Interactable objects
            {

                //NOTE: Interacting with object first then performing the necessary action on it
                if( AIControlled && !TargetInteraction.ConsumesInteractable )
                {
                    CharacterInteraction InteractingObject = TargetInteraction.GetComponent<CharacterInteraction>();
                    InteractingObject.Interact( CurrentHeldObject, this );
                }
                else{
                    TargetInteraction.Interact( CurrentHeldObject );
                }

                if( !CurrentHeldObject && TargetInteraction.InteractableData.ObjectType != InteractableType.Child )
                {
                    if( TargetInteraction.InteractableData )
                    {
                        HoldObject( TargetInteraction.InteractableData.HeldObjectData );
                    }
                    else
                    {
                        Debug.LogError("ERROR: NO INTERACTABLE DATA");
                        Debug.Break();
                    }
                }

                if( CurrentHeldObject )
                {

                    print(" Using: " + CurrentHeldObject.Name + " on " + TargetInteraction.gameObject.name );
                    UseItemOnObject( Target );
                }

                Interacting = false;
                InteractionTime = 0.0f;
                Target = null;
                TargetInteraction = null;
                Debug.Log("No More Target");
            }
        }
    }

    void
    CharacterMovement()
    {
        if( Interacting ) return;

        if( Moving )
        {
            Vector3 direction;
            //TODO: This will need to be changed, if our target is moving we need to look at where it's going and path to that point we will also need to see if we are close enough to just predict it's path and move towards that
            if( !TargetOverride )
            {
                direction = Destination - MyTransform.position;
            }
            else
            {
                direction = Target.position - MyTransform.position;
            }

            if( direction.magnitude < SatisfiedDistance )
            {
                Destination = Vector3.zero;
                Moving = false;
            }
             direction.Normalize();
             Vector3 newPosition = direction * CharacterSpeed;
             MyTransform.position += newPosition * Time.deltaTime * MoveSpeed;
        }
        else if( Path != null && !Moving )
        {
            if( PathIndex >= Path.Count )
            {
                Path = null;
                PathIndex = 0;
                if( SwitchPath ) SwitchPath = false;
            }
            else
            {
                Vector2 NewDestination = Path[ PathIndex ];
                SetNewDestination( NewDestination );
                PathIndex++;
            }
        }


    }
private void
TargetHandling()
{
    if( !Target && !SwitchPath ) return;

    Vector3 ToTarget = Target.position - MyTransform.position;
    if( ToTarget.magnitude < SeekThreshold )
    {
        Vector2 ColliderSize = Collider.size;
        if( !Physics2D.BoxCast( MyTransform.position, ColliderSize, 0, ToTarget, 15) )
        {
            TargetOverride = true;
            Path = null;
        }
        else
        {
            if( Path == null )
            {
                SetNewPathToTarget( Target ); // Repath
                SwitchPath = true;
            }
            TargetOverride = false;
        }
    }
}

    void
    CharacterRotation( Vector3 Direction )
    {
        float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        MyTransform.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
    }

    public void
    SetNewDestination( Vector3 NewDestination )
    {
        Moving = true;
        Destination = NewDestination;
    }

    public void
    SetNewDestination( Vector2 NewDestination )
    {
        Moving = true;
        Destination = new Vector3( NewDestination.x, NewDestination.y, MyTransform.position.z );
    }

    public void
    SetNewTarget( Transform targetTransform )
    {
        Target = targetTransform;
        TargetInteraction = targetTransform.GetComponent<Interaction>();
    }

    public void
    SetNewPath( Vector2 TargetPosition )
    {
        Vector3 myPosition = MyTransform.position;
        int Rows = WorldStatus.totalRows;
        int Columns = WorldStatus.totalColumns;

        Vector2 position = new Vector2( myPosition.x, myPosition.y );
        WorldNode startNode = WorldStatus.GetWorldNodeFromPosition( position );

        WorldNode goalNode = WorldStatus.GetWorldNodeFromPosition( TargetPosition );
        if( !goalNode.Walkable ) // If we target is unreachable perform a sweep test on the mouse position to see if where the user clicked is walkable, if it is then our goal becomes the closest walkable node and then move we move to the target position
        //NOTE: There are some cases where this isn't the right answer but in about ~85% of the cases this is the best case
        {
            Vector2 Offset = Collider.offset;
            Vector2 ColliderSize = Collider.size;
            Vector2 PredictedColliderPosition = TargetPosition - Offset;

            if( Physics2D.BoxCast( PredictedColliderPosition, ColliderSize, 0, Vector2.zero, 1, CollidingLayer ) )
            {
                return;
            }

            Debug.Log("Finding Nearest Node");
            goalNode = WorldStatus.FindNearestPathableNode( in goalNode );
        }

        Path = AStar.Path( in WorldStatus.WorldState, Rows, Columns, in startNode, in goalNode );
        if( Path != null ) Path.Add( TargetPosition );
    }

    public void
    SetNewPathToTarget( Transform Interactable )
    {
        Vector3 myPosition = MyTransform.position;
        int Rows = WorldStatus.totalRows;
        int Columns = WorldStatus.totalColumns;

        Vector2 position = new Vector2( myPosition.x, myPosition.y );
        WorldNode startNode = WorldStatus.GetWorldNodeFromPosition( position );

        WorldNode goalNode = WorldStatus.GetWorldNodeFromPosition( Interactable.position );
        if( !goalNode.Walkable )
        {
            Vector2 TargetPosition = new Vector2( Interactable.position.x, Interactable.position.y );
            Vector2 Offset = Collider.offset;
            Vector2 ColliderSize = Collider.size;
            Vector2 PredictedColliderPosition = TargetPosition - Offset;

            if( Physics2D.BoxCast( PredictedColliderPosition, ColliderSize, 0, Vector2.zero, 1, CollidingLayer ) )
            {
                return;
            }

            Debug.Log("Finding Nearest Node");
            goalNode = WorldStatus.FindNearestPathableNode( in goalNode );
        }

        Path = AStar.Path( in WorldStatus.WorldState, Rows, Columns, in startNode, in goalNode );

        if( Path != null )
        {
            Target = Interactable;
            TargetInteraction = Interactable.GetComponent<Interaction>();
            Path.Add( Interactable.position );
        }
    }

    public void
    HoldObject( in ScriptableHeldObject PickupObject )
    {
        if ( !PickupObject ) return;

        SpriteRenderer HoldingRenderer = HoldingObject.GetComponent<SpriteRenderer>();

        HoldingRenderer.sprite = PickupObject.HeldObjectSprite;
        CurrentHeldObject = PickupObject;
    }

    public void
    UseItemOnObject( Transform InteractionObject )
    {
        Interaction Entity = InteractionObject.GetComponent<Interaction>();
        if( Entity.ConsumesInteractable )
        {
            DiscardObject();
        }
    }

    public void
    DiscardObject()
    {
        SpriteRenderer HoldingRenderer = HoldingObject.GetComponent<SpriteRenderer>();
        HoldingRenderer.sprite = null;
        CurrentHeldObject = null;
    }

}
