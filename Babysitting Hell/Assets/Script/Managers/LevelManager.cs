using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Timer")]
    public float MinutesInLevel;

    [Header("Children Spawn Points:")]
    [SerializeField] private Transform ChildSpawnPoints;

    [Space(10)]

    [Header("Babysitter Spawn Points")]
    [SerializeField] private Transform BabySitterSpawnPoints;

    [Space(10)]

    [Header("Music:")]
    [SerializeField] private AudioClip BackgroundMusic;

    [Space(20)]

    [Header("Challenges")]
    [SerializeField] private List<InteractableChallenge> InteractableChallenges;

    [SerializeField] private List<ChildChallenge> ChildChallenges;

    [Header("Activities")]
    public List<Activity> SceneActivities;

    [Space(20)]

    [Header("Children Level Parameters")]
    [SerializeField] private List<ScriptableChild> SceneChildren;

    [SerializeField] private List<ScriptablePersonality> SceneChildrenPersonalities;

    [SerializeField] private int NumberOfChildren;

    [SerializeField] private GameObject ChildPrefab;

    private List<GameObject> InteractablesInScene;
    private List<GameObject> ChildrenInScene;
    private List<Character> ChildrenCharactersInScene;
    private LevelTimer LT;
    private UIManager UIController;
    public static LevelManager Instance;
    private WorldDecomposition Worldstate;

    private void
    Awake()
    {
        if( Instance == null )
        {
            Instance = this;
        }
        else if( Instance != this )
        {
            Destroy( this );
        }
        DontDestroyOnLoad( this );
     }

    void Start()
    {
        //NOTE: Creating Background Music
        //TODO: We're going to want to load the music Asynchronously and then adjust the volume dynamically
        GameObject BGMusic = new GameObject();
        AudioSource BackgroundSource = BGMusic.AddComponent<AudioSource>();
        BackgroundSource.clip = BackgroundMusic;
        BackgroundSource.loop = true;
        BackgroundSource.volume = .05f;
        BackgroundSource.Play();

        LT = FindObjectOfType< LevelTimer >();
        LT.SetTimer( MinutesInLevel );

        UIController = FindObjectOfType<UIManager>();

        InteractablesInScene = new List<GameObject>();
        ChildrenInScene = new List<GameObject>();

        if( !FindAllChildInteractables() )
        {
            Debug.LogError("ERROR: NO INTERACTABLES IN SCENE");
            Debug.Break();
        }

        SpawnCharacters( in ChildSpawnPoints, in SceneChildren, in  SceneChildrenPersonalities);
        UIController.InitializeUI( in ChildrenInScene, in InteractableChallenges, in ChildChallenges );
        Worldstate = FindObjectOfType< WorldDecomposition >();
    }

    public List<GameObject>
    GetCharacterInteractables()
    {
        return InteractablesInScene;
    }

    public void
    EndLevel()
    {
        //TODO: Bring up Scoring screen, cleanup all of the children, the player and all of the scene objects
        UIController.FadeOut();
        if( UIController.Score( in ChildrenInScene ) )
        {
            CleanupLevel();
        }
    }

    private bool
    SpawnCharacters( in Transform SpawnPoints, in List<ScriptableChild> CharacterPool, in List<ScriptablePersonality> PersonalityPool )
    {
        print("Spawning Characters...");
        //TODO: Spawn the player here as well using the Player Spawn point

        //Error Checking
        if( NumberOfChildren <= 0 )
        {
            return false;
        }

        //Setup
        int SpawnPointCount = SpawnPoints.childCount;
        int ChildrenCount = CharacterPool.Count;
        int PersonalityCount = PersonalityPool.Count;

        if(SpawnPointCount == 0 || ChildrenCount == 0 || PersonalityCount == 0 )
        {
            return false;
        }

        List<Transform> TempSpawns = new List<Transform>();
        List<ScriptableChild> TempChildren = new List<ScriptableChild>();
        List<ScriptablePersonality> TempPersonalities = new List<ScriptablePersonality>();

        //NOTE: Filling Temporary lists which will be used to randomly select from until we run out then we will randomly select from the intial list
        foreach( Transform point in SpawnPoints )
        {
            TempSpawns.Add( point );
        }

        foreach( ScriptableChild child in CharacterPool )
        {
            TempChildren.Add( child );
        }

        foreach( ScriptablePersonality personality in PersonalityPool )
        {
            TempPersonalities.Add( personality );
        }

        //NOTE: Instantiating the children in the scene
        for( int i = 0; i < NumberOfChildren; i++ )
        {

            Transform selectedTransform;
            ScriptableChild selectedChild;
            ScriptablePersonality selectedPersonality;

            //NOTE: Selecting necessary components for the character
            if( TempSpawns.Count >= 1 ) // If our temporary lists are not empty then we will continue to pick from them
            {
                selectedTransform = TempSpawns[ Random.Range( 0, TempSpawns.Count - 1) ];
                TempSpawns.Remove( selectedTransform );
            }
            else
            {
                selectedTransform = SpawnPoints.GetChild( Random.Range( 0, SpawnPointCount - 1) );
            }

            if( TempChildren.Count >= 1 )
            {
                selectedChild = TempChildren[ Random.Range( 0, TempChildren.Count - 1) ];
                TempChildren.Remove( selectedChild );
            }
            else
            {
                selectedChild = CharacterPool[ Random.Range( 0, ChildrenCount - 1) ];
            }

            if( TempPersonalities.Count >= 1 )
            {
                selectedPersonality = TempPersonalities[ Random.Range( 0, TempPersonalities.Count - 1) ];
                TempPersonalities.Remove( selectedPersonality );
            }
            else
            {
                selectedPersonality = PersonalityPool[ Random.Range( 0, PersonalityCount - 1) ];
            }

            GameObject go = Instantiate( ChildPrefab, selectedTransform.position, new Quaternion(0,0,0,0) );

            ChildAI_test ChildAI = go.GetComponent<ChildAI_test>();
            ChildAI.GenerateChild( in selectedChild,  selectedPersonality );
            ChildAI.ChildNumber = i;
            ChildrenInScene.Add( go );
        }

        return true;
    }

    private bool
    FindAllChildInteractables()
    {
        CharacterInteraction[] Interactables = FindObjectsOfType<CharacterInteraction>();
        if( Interactables.GetLength(0) == 0 ) return false;
        foreach( CharacterInteraction Interactable in Interactables )
        {
            InteractablesInScene.Add( Interactable.gameObject );
        }

        return true;
    }

    public List<Activity>
    GetAvailableActivities()
    {
        //Populating Temporary List
        List<Activity> AvailableActivites = new List<Activity>();
        foreach( Activity A in SceneActivities )
        {
            AvailableActivites.Add( A );
        }

        // Running through each child to make sure we remove all activites currently being performed

            /*
             //TODO: The scene children are running this while other children are being instantiated and added to the list meaning it's illegal for a child to touch the list while it is already being accessed by someone else. There needs to be a go command after everything has been set up, and all of the assets challenges, UI, etc. has been loaded in and the level is actually ready to start then children can access this list without fear of others accessing it
        foreach( GameObject child in ChildrenInScene )
        {

            ChildAI_test AI = child.GetComponent<ChildAI_test>();
            foreach( Activity A in AvailableActivites )
            {
                if( A == AI.SelectedActivity )
                {
                    AvailableActivites.Remove( A );
                }
            }
        }
        */

        return AvailableActivites;
    }

    public List< GameObject >
    GetChildrenInScene()
    {
        return ChildrenInScene;
    }

    public void
    CleanupLevel()
    {
        //TODO: Destroy the children cleanup the lists and cleanup all of the interactables and destroy the controlling character
        UIController.ClearUI();
    }

    public void
    ReloadScene()
    {
        Destroy( Worldstate.gameObject );
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

}
