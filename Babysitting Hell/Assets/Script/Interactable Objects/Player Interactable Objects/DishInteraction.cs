public class DishInteraction : PlayerInteraction
{

    private int Uses;

    private void
    Start()
    {
        Uses = 5;
    }

    public override void
    Interact( ScriptableHeldObject UsedObject )
    {
        Uses--;
        print("Picked Up Dish");
        if( Uses <= 0 )
        {
            print("Dishes Gone");
            Destroy( this.gameObject );
        }
    }

}
