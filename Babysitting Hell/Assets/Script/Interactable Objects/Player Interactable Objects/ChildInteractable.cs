public class ChildInteractable : PlayerInteraction
{
    public override void
    Interact( ScriptableHeldObject UsedObject )
    {
        Character Child = this.GetComponent<Character>();
        ChildAI_test AI = this.GetComponent<ChildAI_test>();

        print("Player Interacting with Character");

        if( UsedObject )
        {
            switch( UsedObject.ObjectType )
            {
                case HeldObject.Candy:
                AI.IncreaseHappiness( 15 );
                break;
                case HeldObject.Bandage:
                AI.IncreaseHealth( 15 );
                break;
            }

        }

        Child.Moving = true;

    }

}
