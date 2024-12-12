public class MedicineCabinet : PlayerInteraction
{
    private int Uses;

    private void
    Start()
    {
        Uses = 1;
    }

    public override void
    Interact( ScriptableHeldObject UsedObject )
    {
        Uses--;
        print("Picked Up Bandaid");
        if( Uses <= 0 )
        {
            Destroy( this.gameObject );
        }
    }

}
