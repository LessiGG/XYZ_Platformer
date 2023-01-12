using System.Collections;
using PixelCrew.Model.Data;
using PixelCrew.Model.Definitions;
using PixelCrew.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.UI.Hud.Dialogs
{
    public class DialogBoxController : MonoBehaviour
    {
        [SerializeField] private GameObject _container;
        [SerializeField] private Animator _animator;

        [Space] [SerializeField] private float _textSpeed = 0.09f;
        
        [Header("Sounds")]
        [SerializeField] private AudioClip _typingSound;
        [SerializeField] private AudioClip _openSound;
        [SerializeField] private AudioClip _closeSound;
        
        [Space] [SerializeField] protected DialogContent _content;

        private int _currentSentance;
        private AudioSource _sfxSource;
        private DialogData _data;
        private Coroutine _typingRoutine;

        private UnityEvent _onComplete;
        protected Sentence CurrentSentence => _data.Sentences[_currentSentance];

        private static readonly int IsOpenKey = Animator.StringToHash("is-open");

        protected virtual DialogContent CurrentContent => _content;

        private void Start()
        {
            _sfxSource = AudioUtils.FindSfxSource();
        }

        public void ShowDialog(DialogData data, UnityEvent onComplete)
        {
            _onComplete = onComplete;
            _data = data;
            _currentSentance = 0;
            CurrentContent.Text.text = string.Empty;
            
            _container.SetActive(true);
            _sfxSource.PlayOneShot(_openSound);
            _animator.SetBool(IsOpenKey, true);
            DisableInput.SetInput(false);
        }

        private IEnumerator TypeDialogText()
        {
            var def = DefsFacade.I.Characters.Get(CurrentSentence.Character);
            var icon = def.Icon;
            
            CurrentContent.Text.text = string.Empty;
            var sentance = CurrentSentence;
            CurrentContent.TrySetCharacterIcon(icon);

            var localizedSentence = sentance.Value.Localize();
            
            foreach (var letter in localizedSentence)
            {
                CurrentContent.Text.text += letter;
                _sfxSource.PlayOneShot(_typingSound);
                yield return new WaitForSeconds(_textSpeed);
            }

            _typingRoutine = null;
        }

        public void OnSkip()
        {
            if (_typingRoutine == null) return;
            
            StopTypeAnimation();
            var sentence = _data.Sentences[_currentSentance].Value;
            CurrentContent.Text.text = sentence.Localize();
        }

        public void OnContinue()
        {
            StopTypeAnimation();
            _currentSentance++;

            var isDialogCompleted = _currentSentance >= _data.Sentences.Length;
            if (isDialogCompleted)
            {
                HideDialogBox();
                _onComplete?.Invoke();
            }
            else
            {
                OnStartDialogAnimation();
            }
        }

        private void HideDialogBox()
        {
            _animator.SetBool(IsOpenKey, false);
            _sfxSource.PlayOneShot(_closeSound);
            DisableInput.SetInput(true);
        }

        private void StopTypeAnimation()
        {
            if (_typingRoutine != null)
                StopCoroutine(_typingRoutine);
            _typingRoutine = null;
        }

        protected virtual void OnStartDialogAnimation()
        {
            _typingRoutine = StartCoroutine(TypeDialogText());
        }

        private void OnCloseAnimationComplete()
        {
        }
    }
}