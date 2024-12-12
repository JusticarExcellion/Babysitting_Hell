public class SkateboardPickup : CharacterInteraction
{
    public override void
    Interact( ScriptableHeldObject UsedObject )
    {
        if(!UsedObject)
        {
            print("You've got nothing!!!");
            return;
        }

        if( UsedObject.ObjectType == HeldObject.Skateboard )
        {
            print("using skateboard");
        }
    }

    public override void
    Interact( ScriptableHeldObject UsedObject, Character character )
    {
        print("Picking Up Skateboard");
        ChildAI_test AI = character.GetComponent<ChildAI_test>();
        AI.DecreaseHealth( 20 );
        AI.IncreaseHappiness( 20 );
    }
}
