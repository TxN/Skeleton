using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using SMGCore.EventSys;
using SMGCore.Windows.Events;

namespace SMGCore.Windows {
	public class WindowManager : MonoSingleton<WindowManager> {

		List<WindowController>  _activeWindows = new List<WindowController>();
		Stack<WindowController> _windows       = new Stack<WindowController>();
		WindowResourcesLoader   _loader        = new WindowResourcesLoader();
		WindowBackground        _background    = null;
		UICanvas                _canvas        = null;

		public WindowController CurrentWindow {
			get {
				return (_windows.Count > 0) ? _windows.Peek() : null;
			}
		}

		public bool HasWindows {
			get {
				return CurrentWindow;
			}
		}

		public bool HasAnyActiveWindows {
			get { return (_activeWindows.Count > 0); }
		}

		WindowBackground Background {
			get {
				return _background;
			}
		}

		bool HasWindowsWithBackground {
			get {
				foreach ( var curWindow in _windows ) {
					if ( curWindow && curWindow.WithBackground ) {
						return true;
					}
				}
				return false;
			}
		}

		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);
			InitializeCache();
			InitializeManager();
			EventManager.Subscribe<Event_WindowStateChanged>(this, OnWindowStateChanged);
		}

		void Start() {
			CommonRefresh();
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		void OnDestroy() {
			EventManager.Unsubscribe<Event_WindowStateChanged>(OnWindowStateChanged);
		}

		void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
			CommonRefresh();
		}

		public void ShowWindow<T>(Action<T> init = null) where T : MonoBehaviour {
			var type = typeof(T);
			if ( _canvas.HasActiveWindow<T>() ) {
				return;
			}
			LoadWindow(init);
		}

		public void AddActiveWindow(WindowController controller) {
			if ( !controller ) {
				Debug.LogError("WindowManager.AddActiveWindow: Controller is null.");
				return;
			}
			_activeWindows.Add(controller);
		}

		public void RemoveActiveWindow(WindowController controller) {
			if ( !controller ) {
				Debug.LogError("WindowManager.RemoveActiveWindow: Controller is null.");
				return;
			}
			_activeWindows.Remove(controller);
		}

		public void TryToHideByBackgroundClick() {
			var window = CurrentWindow;
			if ( window && window.HideByMisclick ) {
				window.Hide();
			}
		}

		public void TryToHideByBackButton() {
			var window = CurrentWindow;
			if ( window && IsClosableByBackButton(window) ) {
				window.Hide();
			}
		}

		public void TryToHideAllWindows(Action callback) {
			while ( _windows.Count > 0 ) {
				var window = _windows.Peek();
				if ( window.HideByMisclick ) {
					window.Hide();
				}
				else {
					break;
				}
			}
			if ( callback != null ) {
				callback.Invoke();
			}
		}

		void InitializeManager() {
			_canvas = new GameObject("[UICanvas]").AddComponent<UICanvas>().Init();

			if ( !_background ) {
				GameObject itemRes = _loader.GetWindowResources<WindowBackground>();
				var item = _canvas.InstantiateNew(itemRes);
				item.SetActive(false);
				if ( item != null ) {
					var background = item.GetComponent<WindowBackground>();
					if ( background ) {
						_background = background;
						_background.Init();
					}
				}

			}
		}

		void InitializeCache() {
			_loader.Initialize();
			StartCoroutine(_loader.LoadResourcesCoro());
		}

		void LoadWindow<T>(Action<T> init) where T : MonoBehaviour {
			var type = typeof(T);
			var cachedItem = _loader.GetWindowResources<T>();
			InitWindow<T>(cachedItem, init);
		}

		void InitWindow<T>(GameObject prefab, Action<T> init) where T : MonoBehaviour {
			var type = typeof(T);
			var instance = _canvas.InstantiateNew(prefab);
			if ( !instance ) {
				Debug.LogErrorFormat("WindowManager.InitWindow({0}): no instance", type.Name);
				return;
			}
			var window = instance.GetComponent<WindowController>();
			var component = instance.GetComponent<T>();
			OpenWindow(window, component, init);
		}

		void OpenWindow<T>(WindowController window, object component, Action<T> init) where T : MonoBehaviour {
			var type = typeof(T);
			if ( !window ) {
				Debug.LogErrorFormat("WindowManager.OpenWindow({0}): no WindowController.", type.Name);
				return;
			}
			if ( component == null ) {
				Debug.LogErrorFormat("WindowManager.OpenWindow({0}): no component.", type.Name);
				return;
			}
			_canvas.Attach(window);

			var tComponent = component as T;
			if ( init != null ) {
				init(tComponent);
			} else {
				window.Show();
			}
		}

		bool IsClosableByBackButton(WindowController window) {
			if ( !window  ) {
				return false;
			}
			return window.HideByMisclick;
		}

		void CommonRefresh() {
			_canvas.TrySetCanvasLayer();
			_canvas.UpdateCanvasCamera();
		}

		void OnWindowStateChanged(Event_WindowStateChanged e) {
			var window = e.Window;
			var state = e.State;
			switch ( state ) {
				case WindowState.Appearing:
					OnShow(window, e.Force);
					break;

				case WindowState.Hiding:
					OnHide(window);
					break;

				case WindowState.Hidden:
					OnTrackingWindowHidden(window);
					break;
			}
		}

		void OnShow(WindowController window, bool force) {
			_canvas.CheckLayers(window, CurrentWindow);
			_windows.Push(window);
			_canvas.SetupLayers(window, Background, HasWindowsWithBackground, force);
			OnTopWindowChanged(window);
		}

		void OnHide(WindowController window) {
			if ( _windows.Count < 1 ) {
				return;
			}
			var currentWindow = CurrentWindow;
			_windows.Pop();

			var prevWindow = CurrentWindow;
			if ( prevWindow ) {
				_canvas.SetupLayers(prevWindow, Background, HasWindowsWithBackground, false);
				OnTopWindowChanged(prevWindow);
				_canvas.MoveToFirstPlane(window);
				if ( prevWindow.IsSuspended ) {
					prevWindow.WakeUp();
				}
				return;
			}

			if ( Background ) {
				Background.Hide(!window.WithBackground);
			}
		}

		void OnTrackingWindowHidden(WindowController window) {
			if ( _windows.Contains(window) ) {
				OnHide(window);
			}
		}

		void OnTopWindowChanged(WindowController newWindow) {
			EventManager.Fire(new Event_WindowTopWindowChange(newWindow));
		}

	}


}
