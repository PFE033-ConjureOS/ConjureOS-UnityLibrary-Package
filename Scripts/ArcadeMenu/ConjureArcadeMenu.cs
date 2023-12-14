using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConjureOS.ArcadeMenu
{
    [RequireComponent(typeof(Animator))]
    public class ConjureArcadeMenu : MonoBehaviour
    {
        private static readonly int UnscaledTime = Shader.PropertyToID("_UnscaledTime");
        
        public static event Action OnOpen;
        public static event Action OnClose;
        public static event Action<bool> OnStatusChanged;
        
        private static ConjureArcadeMenu Instance { get; set; }
        public static bool HasInstance => (bool) Instance;

        private bool isOpened;
        
        public static bool IsOpened => HasInstance && Instance.IsOpenedOnInstance;

        private bool IsOpenedOnInstance
        {
            get => isOpened;
            set
            {
                if (value != isOpened)
                {
                    if (value)
                    {
                        OnOpen?.Invoke();
                    }
                    else
                    {
                        OnClose?.Invoke();
                    }
                    
                    OnStatusChanged?.Invoke(value);
                }

                isOpened = value;
            }
        }

        private ConjureArcadeMenuButton SelectedButton
        {
            get
            {
                if (currentSelectedButtonIndex < 0 || currentSelectedButtonIndex >= menuButtons.Count)
                {
                    return null;
                }

                return menuButtons[currentSelectedButtonIndex];
            }
        }

        [Header("Setup")]
        [SerializeField] 
        private List<ConjureArcadeMenuButton> menuButtons;

        [SerializeField] 
        private ConjureArcadeMenuButton firstMenuButton;

        [SerializeField] 
        private Image backgroundImage;

        private bool IsAnimatingOpen => openAnimationCoroutine != null;
        private bool IsAnimatingClose => closeAnimationCoroutine != null;
        private Coroutine openAnimationCoroutine;
        private Coroutine closeAnimationCoroutine;

        private Animator animator;

        private int currentSelectedButtonIndex = -1;

        public static void Open()
        {
            if (HasInstance)
            {
                Instance.OpenOnInstance();
            }
        }
        
        public static void Close()
        {
            if (HasInstance)
            {
                Instance.CloseOnInstance();
            }
        }

        public static void MovePrevious()
        {
            if (HasInstance)
            {
                Instance.MovePreviousOnInstance();
            }
        }

        public static void MoveNext()
        {
            if (HasInstance)
            {
                Instance.MoveNextOnInstance();
            }
        }

        public static void Select()
        {
            if (HasInstance)
            {
                Instance.SelectOnInstance();
            }
        }
        
        private void OpenOnInstance()
        {
            if (IsOpenedOnInstance)
            {
                return;
            }

            int indexOnOpen = menuButtons.IndexOf(firstMenuButton);
            if (indexOnOpen == -1)
            {
                indexOnOpen = 0;
            }
            UpdateSelectedButtonIndex(indexOnOpen);
            
            IsOpenedOnInstance = true;
            openAnimationCoroutine = StartCoroutine(AnimateOpenRoutine());
        }

        private void CloseOnInstance()
        {
            if (!IsOpenedOnInstance)
            {
                return;
            }

            IsOpenedOnInstance = false;
            closeAnimationCoroutine = StartCoroutine(AnimateCloseRoutine());
        }

        private void MovePreviousOnInstance()
        {
            if (!IsOpenedOnInstance || menuButtons.Count == 0)
            {
                return;
            }
            
            UpdateSelectedButtonIndex(currentSelectedButtonIndex - 1 < 0 ? menuButtons.Count - 1 : currentSelectedButtonIndex - 1);
        }

        private void MoveNextOnInstance()
        {
            if (!IsOpenedOnInstance || menuButtons.Count == 0)
            {
                return;
            }

            UpdateSelectedButtonIndex((currentSelectedButtonIndex + 1) % menuButtons.Count);
        }

        private void UpdateSelectedButtonIndex(int newButtonIndex)
        {
            if (SelectedButton != null)
            {
                SelectedButton.IsSelected = false;
            }
            currentSelectedButtonIndex = newButtonIndex;
            if (SelectedButton != null)
            {
                SelectedButton.IsSelected = true;
            }
        }

        private void SelectOnInstance()
        {
            if (!IsOpenedOnInstance)
            {
                return;
            }

            if (currentSelectedButtonIndex < 0 || currentSelectedButtonIndex >= menuButtons.Count)
            {
                return;
            }

            ConjureArcadeMenuButton buttonToSelect = menuButtons[currentSelectedButtonIndex];
            if (!buttonToSelect)
            {
                return;
            }
            
            buttonToSelect.Execute();
        }
        
        private IEnumerator AnimateOpenRoutine()
        {
            if (animator && !IsAnimatingOpen)
            {
                float normalizedTimeToStart = 0.0f;
                if (IsAnimatingClose)
                {
                    normalizedTimeToStart =  1.0f - animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    StopCoroutine(AnimateCloseRoutine());
                    closeAnimationCoroutine = null;
                }
                
                animator.Play("Open", 0, normalizedTimeToStart);
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            }

            openAnimationCoroutine = null;
        }
        
        private IEnumerator AnimateCloseRoutine()
        {
            if (animator && !IsAnimatingClose)
            {
                float normalizedTimeToStart = 0.0f;
                if (IsAnimatingOpen)
                {
                    normalizedTimeToStart =  1.0f - animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    StopCoroutine(AnimateOpenRoutine());
                    openAnimationCoroutine = null;
                }
                
                animator.Play("Close", 0, normalizedTimeToStart);
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            }

            closeAnimationCoroutine = null;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (HasInstance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            // To adjust the layout groups at the start when the window hasn't had time to resize yet.
            // If this is not called, the layout might not work correctly.
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void Update()
        {
            if (backgroundImage)
            {
                Material backgroundMaterial = backgroundImage.material;
                if (backgroundMaterial)
                {
                    backgroundMaterial.SetFloat(UnscaledTime, Time.unscaledTime);
                }
            }
        }
    }
}