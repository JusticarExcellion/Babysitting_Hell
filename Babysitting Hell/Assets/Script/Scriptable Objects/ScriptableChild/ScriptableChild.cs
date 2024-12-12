using UnityEngine;

[CreateAssetMenu(fileName="Data", menuName = "ScriptableObjects/Children/ScriptableChild", order = 2)]
public class ScriptableChild : ScriptableObject
{
    public Sprite ChildSprite;
    public Sprite ChildHeadSprite;
    //NOTE: Maybe we add different modifiers in here as well
}
