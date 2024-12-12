using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildListener : MonoBehaviour
{
    private List< ChildAI_test > ChildrenInScene;
    public ChildChallenge attachedChallenge;
    private int Threshold;
    private bool HappinessOrHealth;
    public int Score;
    private ChildListener Self;
    private bool activated;

    void Start()
    {
    }

    public void
    StartChallenge()
    {
        ChildrenInScene = new List< ChildAI_test >();
        List< GameObject > children = LevelManager.Instance.GetChildrenInScene();
        foreach( GameObject child in children )
        {
            ChildAI_test childAI = child.GetComponent< ChildAI_test >();
            if( childAI )
            {
                ChildrenInScene.Add( childAI );
            }
        }

        Threshold = attachedChallenge.Threshold;
        HappinessOrHealth = attachedChallenge.HappinessOrHealth;
        Score = attachedChallenge.ChallengeScore;
        Self = this.GetComponent< ChildListener >();
        activated = true;
    }

    void FixedUpdate()
    {
        if( activated )
        {
            foreach( ChildAI_test childAI in ChildrenInScene )
            {
                if( !HappinessOrHealth )
                {
                    if( childAI.Health < Threshold )
                    {
                        ChallengeManager.Instance.ChallengeFailed( in Self );
                    }
                }
                else
                {
                    if( childAI.Happiness < Threshold )
                    {
                        ChallengeManager.Instance.ChallengeFailed( in Self );
                        Destroy( this );
                    }
                }
            }
        }
    }
}
