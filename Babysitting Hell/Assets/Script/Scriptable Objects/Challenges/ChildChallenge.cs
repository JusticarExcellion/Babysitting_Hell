using UnityEngine;

[CreateAssetMenu(fileName= "Data", menuName = "ScriptableObjects/Challenges/ChildChallenge", order = 1)]
public class ChildChallenge : ScriptableObject
{
    public string Description;
    public int Threshold;
    public bool HappinessOrHealth;
    public int ChallengeScore;
}
