using UnityEngine;

public enum HeldObject
{
    Crayon,
    Bandage,
    Candy,
    Dishes,
    Toy,
    Skateboard,
    Nothing
}

[CreateAssetMenu(fileName= "Data", menuName = "ScriptableObjects/Objects/HeldObject", order = 1)]
public class ScriptableHeldObject : ScriptableObject
{
    public string Name;
    public HeldObject ObjectType;
    public Sprite HeldObjectSprite;
}
