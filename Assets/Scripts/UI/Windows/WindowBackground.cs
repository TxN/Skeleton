using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace SMGCore.Windows {
	public class WindowBackground : MonoBehaviour {
		public float  WaitTime       = 0.15f;
		public float  FadeTime       = 0.5f;
		public Canvas OverrideCanvas = null;

		bool    _shown   = false;
		bool    _inShow  = false;
		bool    _inHide  = false;		
		Tweener _tweener = null;

		Image _image = null;
		public Image CachedImage {
			get {
				if ( !_image ) {
					_image = GetComponent<Image>();
				}
				return _image;
			}
		}

		RectTransform _transform = null;
		public RectTransform CachedTransform {
			get {
				if ( !_transform ) {
					_transform = GetComponent<RectTransform>();
				}
				return _transform;
			}
		}

		CanvasGroup _group = null;
		public CanvasGroup CachedCanvasGroup {
			get {
				if ( !_group ) {
					_group = GetComponent<CanvasGroup>();
				}
				return _group;
			}
		}

		Button _button = null;
		public Button CachedButton {
			get {
				if ( !_button ) {
					_button = GetComponent<Button>();
				}
				return _button;
			}
		}

		public void Init() {
			CachedCanvasGroup.alpha = 0;
			CachedButton.onClick.AddListener(OnClick);
		}

		public void Resetup(bool withState = false) {
			ResetTweener();
			gameObject.SetActive(false);
			if ( withState ) {
				_shown  = false;
				_inShow = false;
				_inHide = false;
			}
		}

		public void Show(bool force, Color fadeColor) {
			ResetTweener();
			if ( !_shown ) {
				CachedImage.color = fadeColor;
				gameObject.SetActive(true);
				if ( force ) {
					CachedCanvasGroup.alpha = 1;
					_inShow = true;
					OnShowComplete();
				} else {
					_tweener = CachedCanvasGroup.DOFade(1, FadeTime).SetDelay(WaitTime).OnComplete(OnShowComplete);
					_inShow = true;
				}
			}
		}

		public void Hide(bool force) {
			ResetTweener();
			if ( _shown ) {
				_tweener = CachedCanvasGroup.DOFade(0, FadeTime).SetDelay(WaitTime).OnComplete(OnHideComplete);
				_shown = false;
				_inHide = true;
			} else if ( force ) {
				gameObject.SetActive(false);
			}
		}

		public void OnClick() {
			WindowManager.Instance.TryToHideByBackgroundClick();
		}

		void OnShowComplete() {
			if ( _inShow ) {
				_shown  = true;
				_inShow = false;
			}
		}

		void OnHideComplete() {
			if ( _inHide ) {
				gameObject.SetActive(false);
			}
		}

		void ResetTweener() {
			if ( _tweener == null ) {
				return;
			}

			_tweener.SetAutoKill(false);
			_tweener.Complete();
			_tweener.Kill();
			_tweener = null;
			if ( _inShow ) {
				OnHideComplete();
			}
			if ( _inHide ) {
				OnShowComplete();
			}
		}
	}
}
