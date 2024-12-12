using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Character))]
public class ChildAI_test : MonoBehaviour
{
    //TODO: If Player is close and interacting with Child then child should stop moving
    private LevelManager levelManager;
    private UIManager UIManager;
    private Character AICharacter;
    [HideInInspector] public ScriptableChild Sprites;
    [HideInInspector] public ScriptablePersonality Personality;
    [HideInInspector] public ChildUI UI;

    [Header("AI Values")]
    [SerializeField] private float WaitTime;
    private float CurrentWaitTimer;
    public int CurrentActivityProgress;
    public Activity SelectedActivity;

    [Header("CharacterStats")]
    public int Health;
    public int Happiness;
    private float TickDuration;
    private float CurrentTick;
    private int PreviousCharacterStates;

    [HideInInspector] public int ChildNumber;
    private int PreviousTarget;

    void Start()
    {
        CurrentWaitTimer = 0.0f;
        TickDuration = 3.0f;
        CurrentTick = TickDuration;
        Health = 100;
        Happiness = 50;
        CurrentActivityProgress = -1;
        SelectedActivity = null;
        PreviousTarget = -1;
        levelManager = FindObjectOfType<LevelManager>();
        UIManager = FindObjectOfType<UIManager>();
        AICharacter = this.GetComponent<Character>();

        AICharacter.AIControlled = true;
        ChildAI_test MyAI = this.GetComponent<ChildAI_test>();
        AICharacter.AI = MyAI;
    }

    void
    FixedUpdate()
    {
        int CharacterState = 0;

        if( CurrentTick > 0 )
        {
            CurrentTick -= ( 1 / CurrentTick ) * Time.deltaTime;
        }
        else
        {
            CurrentTick = TickDuration;
            Happiness -= 1;
        }

        if( Happiness < 30 ) //TODO: Filling out the character state
        {
            CharacterState = 1 << 1;
        }
        else
        {
            CharacterState = 0 << 1;
        }

        if( Health < 50 ) //TODO: Filling out the character state
        {
            CharacterState = 1 << 2;
        }
        else
        {
            CharacterState = 0 << 2;
        }

        if( PreviousCharacterStates != CharacterState )
        {

            CharacterBark( CharacterState );
        }

        if( UI ) UI.UpdateChildStatus( (float)Health / 100, (float)Happiness / 100 );
    }

    public void
    InteractionDecision()
    {
        //TODO: Refactor this to select an activity and then find the relevant object to interact with


        Transform selection;

        if( !SelectedActivity )
        {
            List< Activity > AvailableActivities = LevelManager.Instance.SceneActivities;

            //TODO: We should be selecting an activity based on our personality
            //this will need to factor in child personality and certain attributes of the action itself, is it good? is it bad? is it annoying? is it increasing or decreasing an attribute
            //Activities will need associated paramters that will be factored in to determine whether or not we select one action over another
            int selectionNumber = Random.Range(0, AvailableActivities.Count - 1);

            if ( selectionNumber == PreviousTarget )
            {
                //print("Reselecting"); //TODO: Debug
                selectionNumber--;
                if( selectionNumber < 0 )
                {
                    selectionNumber = AvailableActivities.Count - 1;
                }
            }

            Debug.Log("Selected Activity: " + selectionNumber);
            SelectedActivity = AvailableActivities[ selectionNumber ];
            PreviousTarget = selectionNumber;
            CurrentActivityProgress = 0;

#if UNITY_EDITOR
            print( this.gameObject.name + " Selecting new activity: " + SelectedActivity.Name );
#endif
        }
        else{

            selection = SelectedActivity.InteractionLocations[ CurrentActivityProgress ];

            Debug.Log("Current Progress: " + CurrentActivityProgress );

            CurrentActivityProgress++;

            if( CurrentActivityProgress >= SelectedActivity.InteractionLocations.Count )
            {
                SelectedActivity = null;
            }

            //Set character's destination and target
            AICharacter.SetNewPathToTarget( selection );
            Interaction objectLogic = selection.GetComponent<Interaction>();

            if( UI ) ObjectBark( objectLogic.InteractableData.ObjectType );
        }

    }

    public void ResetWaitTimer()
    {
        CurrentWaitTimer = WaitTime;
    }

    public void
    ObjectBark( InteractableType objectType )
    {

        string message = "";
        switch( objectType )
        {
            case InteractableType.Test:
                message = "Message: Test";
                break;

            case InteractableType.Test2:
                message = "This looks neat";
                break;

            case InteractableType.Bed:
                message = "I'm gonna jump on the bed!!!";
                break;

            case InteractableType.Toybox:
                message = "I'm gonna play with some toys.";
                break;

            case InteractableType.Skateboard:
                message = "I'm gonna skateboard!!!";
                break;

            case InteractableType.TV:
                message = "I wanna watch TV!!!";
                break;

        }
        UI.ChildMessage( message );
    }

    public void
    CharacterBark( int CharacterStates )
    {

        string message = "";

        if( CharacterStates == 1 << 1 ) //NOTE: Happiness fell;
        {
            message = "Ahhhh this sucks...";
        }

        if( CharacterStates == 1 << 2 )
        {
            message = "Ow I'm hurt...";
        }

        if( UI ) UI.ChildMessage( message );
        PreviousCharacterStates = CharacterStates;
    }

    public void
    GenerateChild( in ScriptableChild LoadedSprites, in ScriptablePersonality LoadedPersonality )
    {
        Sprites = LoadedSprites;
        Personality = LoadedPersonality;
        Transform ChildSpriteTransform = this.transform.Find("ChildSprite");
        SpriteRenderer ChildSprite = ChildSpriteTransform.GetComponent<SpriteRenderer>();

        ChildSprite.sprite = Sprites.ChildSprite;
    }

    public void
    IncreaseHappiness( int HappinessAmount )
    {
        Happiness += HappinessAmount;
    }

    public void
    IncreaseHealth( int HealthAmount )
    {
        Health += HealthAmount;
    }

    public void
    DecreaseHealth( int HealthAmount )
    {
        Health -= HealthAmount;
    }

    public void
    DecreaseHappiness( int HappinessAmount )
    {
        Happiness -= HappinessAmount;
    }

    private void
    OnDestroy()
    {
    }

}
