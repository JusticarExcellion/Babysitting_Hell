using UnityEngine;

public abstract class Interaction : MonoBehaviour
{

    public InteractableObjects InteractableData;
    public bool ConsumesInteractable;

    public abstract void
    Interact( ScriptableHeldObject UsedObject );

}
