using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public struct ScoreStats
{
    public ChildStats[] ChildStatistics;
    public int ChallengeScore;
}

public struct ChildStats
{
    public int HealthScore;
    public int HappinessScore;
}

public class UIManager : MonoBehaviour
{
    [HideInInspector]public List<ChildUI> UIChildren;
    [HideInInspector]public List<ProgressBar> UIProgressBars;

    [SerializeField] private Transform UICanvas;

    [Header("Children Status Bar")]
    [SerializeField] private GameObject StatusBar;
    [SerializeField] private int Offset;

    [Header("Progress Bar")]
    [SerializeField] private GameObject LinearProgressBar;

    [Header("Challenges")]
    [SerializeField] private ChallengeManager CM;

    [Header("Fade Out")]
    [SerializeField] private Image FadeOutSquare;
    private bool FadeOutImage;

    [Header("Scoring Screen")]
    [SerializeField] private ScoringSystem ScoringScreen;

    void Start()
    {
        UIChildren = new List< ChildUI >();
        UIProgressBars = new List<ProgressBar>();
        FadeOutImage = false;
    }

    void Update()
    {
        if( FadeOutImage )
        {
            FadeOutSquare.CrossFadeAlpha( 1f, 5f, true );
        }
    }

    public bool
    InitializeUI( in List<GameObject> ChildrenInScene, in List<InteractableChallenge> InteractableChallenges, in List<ChildChallenge> ChildChallenges )
    {

        print(" Initialzing UI... ");
        CreateStatusBars( in ChildrenInScene );
        PositionAndResizeStatusBars( in UIChildren );
        CreateChallenges( in InteractableChallenges, in ChildChallenges );

        return true;
    }

    public void
    CreateProgressBar( Transform TargetTransform, float ProgressTime )
    {
        Debug.Log("Progress Bar Timer: " + ProgressTime );
        GameObject go = Instantiate( LinearProgressBar, UICanvas );
        ProgressBar pB = go.GetComponent< ProgressBar >();
        pB.CanvasTransform = UICanvas.GetComponent< RectTransform >();
        pB.ProgressTime = ProgressTime;
        pB.TargetTransform = TargetTransform;

        UIProgressBars.Add( pB );
    }

    public void
    DestroyProgressBar( ProgressBar PB )
    {
        UIProgressBars.Remove( PB );
    }


    private bool
    CreateStatusBars( in List<GameObject> ChildrenInScene )
    {
        //TODO: Do Error Checking to see if each Status bar was instantiated correctly
        foreach( GameObject Child in ChildrenInScene )
        {
            ChildAI_test ChildAI = Child.GetComponent< ChildAI_test >();
            GameObject go = Instantiate( StatusBar, UICanvas );
            ChildUI goChildUI = go.GetComponent<ChildUI>();
            goChildUI.AttachedAI = ChildAI;
            ChildAI.UI = goChildUI;
            goChildUI.ChildHeadSprite.sprite = ChildAI.Sprites.ChildHeadSprite;
            UIChildren.Add( goChildUI );
        }
        return true;
    }

    private bool
    PositionAndResizeStatusBars( in List<ChildUI> ChildStatusBars )
    {
        //TODO: If we need to resize the bars because of how many there are we should be able to do that here
        for( int i = 0; i < ChildStatusBars.Count; i++ )
        {
            ChildUI CurrentChildUI = ChildStatusBars[ i ];
            RectTransform ChildUITransfom = CurrentChildUI.GetComponent<RectTransform>();
            Vector2 NewPosition = ChildUITransfom.anchoredPosition;
            //print("Current Status Bar Position on Screen: " + NewPosition );
            NewPosition.y -= Offset * i;
            //print("New Status Bar Position on Screen: " + NewPosition );
            ChildUITransfom.anchoredPosition = NewPosition;
        }

        return true;
    }

    public void
    CreateChallenges( in List<InteractableChallenge> InteractableChallenges, in List<ChildChallenge> ChildChallenges )
    {
        //TODO: Get Challenge Manager and initialize
        ChallengeManager.Instance.InitializeChallenges( in InteractableChallenges, in  ChildChallenges );
    }

    public void
    FadeOut()
    {
        //NOTE: Activate the fadeout
        FadeOutSquare.gameObject.SetActive( true );
        FadeOutImage = true;
    }

    public bool
    Score( in List<GameObject> Children )
    {
        ScoreStats FinalScore = new ScoreStats();
        //TODO: Look through each of the children and display the player's score for the level based on the challenges completed, the children's health profile, etc.
        FinalScore.ChildStatistics = new ChildStats[ Children.Count ];
        int currentChildCount = 0;

        foreach( GameObject child in Children )
        {
            ChildStats childProfile = new ChildStats();
            ChildAI_test childAI = child.GetComponent< ChildAI_test >();
            childProfile.HealthScore = childAI.Health;
            childProfile.HappinessScore = childAI.Happiness;

            FinalScore.ChildStatistics[ currentChildCount ] = childProfile;
            currentChildCount++;
        }

        FinalScore.ChallengeScore = ChallengeManager.Instance.ScoreChallenges();

        ScoringScreen.gameObject.SetActive( true );
        ScoringScreen.DisplayText( in FinalScore );
        return true;
    }

    public void
    ClearUI()
    {
        while( UIChildren.Count > 0)
        {
            ChildUI element = UIChildren[0];
            UIChildren.Remove(element);
            Destroy( element.gameObject );
        }

        while( UIProgressBars.Count > 0 )
        {
            ProgressBar element = UIProgressBars[0];
            UIProgressBars.Remove(element);
            Destroy( element.gameObject );
        }
    }

}
