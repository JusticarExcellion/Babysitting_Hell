using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableListener : MonoBehaviour
{
    private List< GameObject > InteractablesInScene;
    public InteractableChallenge attachedChallenge;
    private InteractableListener Self;

    private void
    Start()
    {
        PlayerInteraction[]  InteractionScripts = FindObjectsOfType< PlayerInteraction >();
        foreach( PlayerInteraction script in InteractionScripts )
        {
            InteractablesInScene.Add( script.gameObject );
        }

        Self = this.GetComponent< InteractableListener >();
    }

    private void
    FixedUpdate()
    {
        foreach( GameObject interactable in InteractablesInScene )
        {
            if( !interactable )
            {
                ChallengeManager.Instance.ChallengeFailed( Self );
                Destroy( this );
            }
        }
    }

}
