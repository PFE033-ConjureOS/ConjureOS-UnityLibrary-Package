namespace ConjureOS.ArcadeMenu
{
    public class ConjureArcadeResumeButton : ConjureArcadeMenuButton
    {
        public override void Execute()
        {
            ConjureArcadeMenu.Close();
        }
    }
}