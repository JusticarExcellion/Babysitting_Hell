public class WatchTV : CharacterInteraction
{
    public override void
    Interact( ScriptableHeldObject UsedObject )
    {}

    public override void
    Interact( ScriptableHeldObject UsedObject, Character character )
    {
        print("Watching TV");
        ChildAI_test AI = character.GetComponent<ChildAI_test>();
        AI.DecreaseHealth( 5 );
    }
}
