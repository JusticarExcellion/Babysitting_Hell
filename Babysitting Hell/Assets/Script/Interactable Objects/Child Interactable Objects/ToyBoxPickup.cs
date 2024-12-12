public class ToyBoxPickup : CharacterInteraction
{
    public override void
    Interact( ScriptableHeldObject UsedObject )
    {

        if(!UsedObject)
        {
            print("You've got nothing!!!");
            return;
        }

        if( UsedObject.ObjectType == HeldObject.Toy )
        {
            print("Playing with toys");
        }

    }

    public override void
    Interact( ScriptableHeldObject UsedObject, Character character )
    {
        print("Picking up toy");
        ChildAI_test AI = character.GetComponent<ChildAI_test>();
    }
}
