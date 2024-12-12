public class TestInteractable : CharacterInteraction
{
    public override void
    Interact( ScriptableHeldObject UsedObject )
    {

        if(!UsedObject)
        {
            print("You've got nothing!!!");
            return;
        }

        if( UsedObject.ObjectType == HeldObject.Crayon )
        {
            print("Using Crayons on Object");
        }

    }

    public override void
    Interact( ScriptableHeldObject UsedObject, Character character )
    {
    }
}
