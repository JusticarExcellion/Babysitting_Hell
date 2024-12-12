public class Bed : CharacterInteraction
{
    //TODO: This is an interactable that should occur over time

    public override void
    Interact( ScriptableHeldObject UsedObject )
    {
    }

    public override void
    Interact( ScriptableHeldObject UsedObject, Character character )
    {
        ChildAI_test AI = character.GetComponent<ChildAI_test>();
        AI.DecreaseHealth( 15 );
        AI.IncreaseHappiness( 15 );
    }

}
