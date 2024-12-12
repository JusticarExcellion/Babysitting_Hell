using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChallengeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text ChallengeText;
    public static ChallengeManager Instance;
    [Header("Child Challenges:")]
    private List<InteractableChallenge> CurrentInteractableChallenges;
    private List<ChildChallenge> CurrentChildChallenges;
    private List< ChildListener > childListeners;
    private List< InteractableListener > interacatbleListeners;

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
        childListeners = new List< ChildListener >();
    }

    public void
    InitializeChallenges( in List<InteractableChallenge> InteractableChallenges, in List<ChildChallenge> ChildChallenges )
    {
        CurrentInteractableChallenges = InteractableChallenges;
        CurrentChildChallenges = ChildChallenges;

        foreach( ChildChallenge challenge in ChildChallenges )
        {
            ChildListener challengeListener = this.gameObject.AddComponent< ChildListener >();
            challengeListener.attachedChallenge = challenge;
            childListeners.Add( challengeListener );
            challengeListener.StartChallenge();
        }

        foreach( InteractableChallenge challenge in InteractableChallenges )
        {
            InteractableListener challengeListener = this.gameObject.AddComponent< InteractableListener >();
            challengeListener.attachedChallenge = challenge;
            interacatbleListeners.Add( challengeListener );
        }

        RefreshChallengePanel();
    }

    private void
    RefreshChallengePanel()
    {
        if( ChallengeText )
        {

            string text = "Challenges: \n";
            foreach( ChildChallenge challenge in CurrentChildChallenges )
            {
                text += challenge.Description;
                text += "\n";
            }

            ChallengeText.text = text;
        }
    }

    public int
    ScoreChallenges()
    {
        int result = 0;
        foreach( ChildListener listener in childListeners )
        {
            result += listener.Score;
        }
        return result;
    }

    public void
    ChallengeFailed( in ChildListener Listener )
    {
        if( childListeners.Remove( Listener ) )
        {
            Debug.Log( "Child Challenge Failed!!!" );
        }
    }

    public void
    ChallengeFailed( in InteractableListener Listener )
    {
        if( interacatbleListeners.Remove( Listener ) )
        {
            Debug.Log( "Player Challenge Failed!!!" );
        }

        RefreshChallengePanel();
    }

}
