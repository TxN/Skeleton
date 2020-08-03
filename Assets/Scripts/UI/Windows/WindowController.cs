using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using SMGCore.EventSys;
using SMGCore.Windows.Events;

using DG.Tweening;

namespace SMGCore.Windows {
	[RequireComponent(typeof(CanvasGroup))]
	public partial class WindowController : MonoBehaviour {
		[Header("Main Settings")]
		public WindowType   Type            = WindowType.Unknown;
		public ShowAnimType ShowAnim        = ShowAnimType.SlideRight;
		public HideAnimType HideAnim        = HideAnimType.MoveRight;
		public bool         HideByMisclick  = false;

		[Header("Optional Settings")]
		public bool         WithBackground  = true;
		public Canvas       OverrideCanvas  = null;
		public int          MaxOrderInLayer = 0;
		public Button       CloseButton     = null;
		public Color        BgColor         = new Color(0, 0, 0.07f, 0.6f);
		public string       ShowSound       = null;
		public string       HideSound       = null;

		bool                          _inited          = false;
		RectTransform                 _transform       = null;
		CanvasGroup                   _canvasGroup     = null;
		Sequence                      _seq             = null;
		UnityAction<WindowController> _hidingCallback  = null;
		WindowState                   _state           = WindowState.Hidden;

		void Awake() {
			if ( !_inited ) {
				Init();
			}
		}

		void OnEnable() {
			WindowManager.Instance.AddActiveWindow(this);
		}

		void OnDisable() {
			if ( WindowManager.IsAlive ) {
				WindowManager.Instance.RemoveActiveWindow(this);
			}
		}

		void Init() {
			_transform   = GetComponent<RectTransform>();
			_canvasGroup = GetComponent<CanvasGroup>();
			if ( CloseButton ) {
				CloseButton.onClick.AddListener(Hide);
			}
			_inited = true;
		}

		public bool IsVisible {
			get {
				return _state == WindowState.Active;
			}
		}

		public bool IsSuspended {
			get {
				return _state == WindowState.Suspended;
			}
		}

		public bool IsInteractable {
			get {
				return IsVisible || _state == WindowState.Appearing;
			}
		}

		public RectTransform CachedTransform {
			get {
				return _transform;
			}
		}

		public void Show() {
			Show(false);
		}

		public void Hide() {
			Hide(false);
		}

		public void WakeUp() {
			ChangeState(WindowState.Active);
			ResetSequence();
			_seq = CreateAppearSequence();
		}

		public void Suspend() {
			ChangeState(WindowState.Suspended);
			ResetSequence();
			_seq = CreateDisappearSequence();
		}

		public void Kill() {
			_seq = TweenHelper.ResetSequence(_seq, false);
			ChangeState(WindowState.Hidden);
		}

		public void KillSilently() {
			_seq   = TweenHelper.ResetSequence(_seq, false);
			_state = WindowState.Hidden;
			ProcessHide();
		}

		bool CanHide() {
			return (_state == WindowState.Appearing) || (_state == WindowState.Active);
		}

		void Show(bool force) {
			if ( !CanShow() && !force ) {
				return;
			}
			if ( !_inited ) {
				Init();
			}
			ChangeState(WindowState.Appearing, force);
			ResetSequence();
			_seq = CreateAppearSequence();
			if ( force ) {
				_seq.Complete();
				OnShown();
			} else {
				_seq.InsertCallback(GetCallbackTime(), OnShown);
			}
		}

		void Hide(bool force) {
			if ( !CanHide() && !force ) {
				return;
			}
			ChangeState(WindowState.Hiding);
			ResetSequence();
			if ( force ) {
				OnHidden();
			} else {
				_seq = CreateDisappearSequence();
				_seq.AppendCallback(OnHidden);
			}
		}

		void ResetSequence() {
			_seq = TweenHelper.ResetSequence(_seq);
		}

		void OnShown() {
			if ( _state == WindowState.Appearing ) {
				ChangeState(WindowState.Active);
			}
		}

		void OnHidden() {
			ChangeState(WindowState.Hidden);
		}

		void ChangeState(WindowState newState, bool force = false) {
			if ( _state == newState ) {
				return;
			}
			_state = newState;
			if ( IsInteractable ) {
				EnableControls();
			} else {
				DisableControls();
			}

			switch ( newState ) {
				case WindowState.Appearing: {
						if ( !gameObject.activeSelf ) {
							gameObject.SetActive(true);
						}
						if ( !string.IsNullOrEmpty(ShowSound) ) {
							SoundManager.Instance.PlaySound(ShowSound);
						}
						EventManager.Fire(new Event_WindowAppearing(this));
					}
					break;
				case WindowState.Active: {
						EventManager.Fire(new Event_WindowShown(this));
					}
					break;
				case WindowState.Hiding: {
						if ( !string.IsNullOrEmpty(HideSound) ) {
							SoundManager.Instance.PlaySound(HideSound);
						}						
						EventManager.Fire(new Event_WindowHiding(this));
					}
					break;
				case WindowState.Hidden: {
						EventManager.Fire(new Event_WindowHidden(this));
					}
					break;
			}

			if ( _state == newState ) {
				EventManager.Fire(new Event_WindowStateChanged(this, newState, force));
			}
			ProcessHide();
		}

		void ProcessHide() {
			if ( _state != WindowState.Hidden ) {
				return;
			}

			OnHide();
			Destroy(gameObject);
		}

		void OnHide() {
			_hidingCallback?.Invoke(this);
		}

		bool CanShow() {
			return _state == WindowState.Hidden || _state == WindowState.Suspended;
		}

		void EnableControls() {
			SetControlState(true);
		}

		void DisableControls() {
			SetControlState(false);
		}

		void SetControlState(bool enable) {
			if ( _canvasGroup ) {
				_canvasGroup.interactable = enable;
			}
		}


	}
}