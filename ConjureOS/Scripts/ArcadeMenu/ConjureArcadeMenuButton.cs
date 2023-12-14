using UnityEngine;

namespace ConjureOS.ArcadeMenu
{
    public abstract class ConjureArcadeMenuButton : MonoBehaviour
    {
        private bool isSelected;
        private ConjureArcadeMenuButtonSelectionBorder selectionBorder;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (selectionBorder)
                {
                    selectionBorder.IsSelected = value;
                }
                isSelected = value;
            }
        }
        
        public abstract void Execute();

        private void Awake()
        {
            selectionBorder = GetComponentInChildren<ConjureArcadeMenuButtonSelectionBorder>();
        }
    }
}