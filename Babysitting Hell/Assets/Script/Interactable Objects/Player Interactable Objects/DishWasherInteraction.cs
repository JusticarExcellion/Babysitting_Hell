public class DishWasherInteraction : PlayerInteraction
{
    private int Uses;
    void Start()
    {
        Uses = 5;
    }

    public override void
    Interact( ScriptableHeldObject UsedObject )
    {
        if(!UsedObject) return;

        if( UsedObject.ObjectType == HeldObject.Dishes )
        {
            Uses--;
            print( "Put up dish No. " + Uses );
            if( Uses <= 0 )
            {
                Destroy( this.gameObject );
            }
        }
    }
}
