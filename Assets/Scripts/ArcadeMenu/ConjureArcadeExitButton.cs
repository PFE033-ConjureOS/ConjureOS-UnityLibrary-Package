using UnityEngine;

namespace ConjureOS.ArcadeMenu
{
    public class ConjureArcadeExitButton : ConjureArcadeMenuButton
    {
        public override void Execute()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}