using UnityEngine;

public enum InteractableType : int
{
    Child,
    Crayons,
    DishWasher,
    DishRack,
    Bed,
    CandyJar,
    MedicineCabinet,
    Test,
    Test2,
    Toybox,
    Skateboard,
    TV,
    Total
}

[CreateAssetMenu(fileName= "Data", menuName = "ScriptableObjects/Objects/InteractableObject", order = 1)]
public class InteractableObjects : ScriptableObject
{
    public string InteractableName;
    public InteractableType ObjectType;
    public bool PlayerInteractable;
    public float InteractionTime;
    public ScriptableHeldObject HeldObjectData;
}
