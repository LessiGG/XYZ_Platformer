using System.Collections;
using PixelCrew.Model.Data;
using PixelCrew.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PixelCrew.UI.Hud.Dialogs
{
    public class DialogBoxController : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private GameObject _container;
        [SerializeField] private Animator _animator;

        [Space] [SerializeField] private float _textSpeed = 0.09f;
        
        [Header("Sounds")]
        [SerializeField] private AudioClip _typingSound;
        [SerializeField] private AudioClip _openSound;
        [SerializeField] private AudioClip _closeSound;

        private int _currentSentance;
        private AudioSource _sfxSource;
        private DialogData _data;
        private Coroutine _typingRoutine;

        private static readonly int IsOpenKey = Animator.StringToHash("is-open");

        private void Start()
        {
            _sfxSource = AudioUtils.FindSfxSource();
        }

        public void ShowDialog(DialogData data)
        {
            _data = data;
            _currentSentance = 0;
            _text.text = string.Empty;
            
            _container.SetActive(true);
            _sfxSource.PlayOneShot(_openSound);
            _animator.SetBool(IsOpenKey, true);
        }

        private IEnumerator TypeDialogText()
        {
            _text.text = string.Empty;
            var sentance = _data.Sentences[_currentSentance];
            foreach (var letter in sentance)
            {
                _text.text += letter;
                _sfxSource.PlayOneShot(_typingSound);
                yield return new WaitForSeconds(_textSpeed);
            }

            _typingRoutine = null;
        }

        public void OnSkip()
        {
            if (_typingRoutine == null) return;
            
            StopTypeAnimation();
            _text.text = _data.Sentences[_currentSentance];
        }

        public void OnContinue()
        {
            StopTypeAnimation();
            _currentSentance++;

            var isDialogCompleted = _currentSentance >= _data.Sentences.Length;
            if (isDialogCompleted)
            {
                HideDialogBox();
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
        }

        private void StopTypeAnimation()
        {
            if (_typingRoutine != null)
                StopCoroutine(_typingRoutine);
            _typingRoutine = null;
        }

        private void OnStartDialogAnimation()
        {
            _typingRoutine = StartCoroutine(TypeDialogText());
        }

        private void OnCloseAnimationComplete()
        {
            
        }
    }
}