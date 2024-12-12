using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoringSystem : MonoBehaviour
{
    [Header("Content Text")]
    [SerializeField] private TMP_Text ChallengeText;
    [SerializeField] private TMP_Text ChildrenText;
    [SerializeField] private TMP_Text FinalScoreText;

    private readonly string ChallengeSetupText = "Challenge: \n";
    private readonly string ChildrenSetupText = "Children: \n";
    private readonly string FinalSetupText = "Final Score: \n";

    public void
    DisplayText( in ScoreStats finalScore )
    {
        int TotalScore = 0;
        //TODO: Give the information and numbers required to display on the score screen
        string ChallengePanelText = ChallengeSetupText;
        string ChildPanelText = ChildrenSetupText;
        string FinalPanelText = "";

        //NOTE: Challenge Text
        TotalScore += finalScore.ChallengeScore;
        ChallengePanelText += "Challenges Compeleted Bonus: " + finalScore.ChallengeScore + "\n";
        int childNo = 1;

        //NOTE: Child Text
        foreach( ChildStats childScore in finalScore.ChildStatistics )
        {
            ChildPanelText += "Child " + childNo + ": \n";
            ChildPanelText += "\t Health Score: " + childScore.HealthScore + "\n";
            ChildPanelText += "\t Happiness Score: " + childScore.HappinessScore + "\n";
            TotalScore += childScore.HealthScore;
            TotalScore += childScore.HappinessScore;
            childNo++;
        }

        //NOTE: Final Score Text
        FinalPanelText += "Final Score: " + TotalScore;

        ChallengeText.text =  ChallengePanelText;
        ChildrenText.text =  ChildPanelText;
        FinalScoreText.text =  FinalPanelText;
    }
}
