public class CandyJar : PlayerInteraction
{

    private int Uses;

    private void
    Start()
    {
        Uses = 3;
    }

    public override void
    Interact( ScriptableHeldObject UsedObject )
    {
        Uses--;
        print("Picked up some candy");
        if( Uses <= 0 )
        {
            Destroy( this.gameObject );
        }
    }
}
