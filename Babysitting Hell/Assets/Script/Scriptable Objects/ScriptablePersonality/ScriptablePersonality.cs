using UnityEngine;

[CreateAssetMenu(fileName= "Data", menuName = "ScriptableObjects/Child/ScriptablePersonality", order = 3)]
public class ScriptablePersonality : ScriptableObject
{
    [Range(1,100)]
    public int Annonyingness;

    [Range(1,100)]
    public int TroubleMaker;

    [Range(1,100)]
    public int Nice;

    [Range(1,100)]
    public int Hungry;
}
