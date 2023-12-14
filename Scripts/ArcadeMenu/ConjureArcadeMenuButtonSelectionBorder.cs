using System.Collections;
using UnityEngine;

namespace ConjureOS.ArcadeMenu
{
    public class ConjureArcadeMenuButtonSelectionBorder : MonoBehaviour
    {
        [SerializeField] 
        private AnimationCurve scaleCurve;

        [SerializeField] 
        private float scaleSpeed = 1.0f;
        
        [SerializeField] 
        private float scaleFactor = 1.0f;
        
        private bool isSelected;
        private CanvasGroup canvasGroup;

        private float scaleTimer;
        private Coroutine scaleCoroutine;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value && !isSelected)
                {
                    scaleTimer = 0;
                    scaleCoroutine = StartCoroutine(ScaleRoutine());
                }
                else if (!value)
                {
                    StopScaleRoutine();
                }
                
                if (canvasGroup)
                {
                    canvasGroup.alpha = value ? 1f : 0f;
                }
                isSelected = value;
            }
        }
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            StopScaleRoutine();
        }

        private IEnumerator ScaleRoutine()
        {
            while (true)
            {
                float currentScale = scaleCurve.Evaluate(scaleTimer);
                transform.localScale = Vector3.one + new Vector3(currentScale, currentScale, currentScale) * scaleFactor;

                yield return null;
                
                scaleTimer += Time.unscaledDeltaTime * scaleSpeed;
                if (scaleTimer > 1.0f)
                {
                    scaleTimer -= 1.0f;
                }
            }
            
            // ReSharper disable once IteratorNeverReturns
            // Reason: This coroutine ends when the border is no longer connected.
        }

        private void StopScaleRoutine()
        {
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
                scaleCoroutine = null;
            }
        }
    }
}