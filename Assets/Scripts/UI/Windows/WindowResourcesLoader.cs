using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SMGCore.Windows {
	public class WindowResourcesLoader {

		Dictionary<Type, WindowInfo> _windows = new Dictionary<Type, WindowInfo>();

		public void Initialize() {
			RegisterAllWindows();
		}

		public GameObject GetWindowResources<T>() {
			_windows.TryGetValue(typeof(T), out var info);
			if ( info == null ) {
				Debug.LogErrorFormat("WindowResourcesLoader.GetWindow: window {0} isn't registered.", typeof(T).ToString());
				return null;
			}
			if ( !info.IsResourcesLoaded ) {
				info.LoadFromResources();
			}
			return info.LoadedResources;
		}

		void RegisterAllWindows() {
			Register<WindowBackground>("UI/Windows/WindowBackground", true);
			//Add new windows here
		}

		void Register<T>(string path, bool preload) where T: MonoBehaviour {
			var info = new WindowInfo {
				Type = typeof(T),
				ResourcesPath = path
			};
			_windows.Add(typeof(T), info);
		}

		public IEnumerator LoadResourcesCoro() {
			foreach ( var w in _windows ) {
				if ( w.Value.IsResourcesLoaded ) {
					continue;
				} else {
					w.Value.LoadFromResources();
					if ( w.Value.IsResourcesLoaded ) {
						Debug.LogFormat("WindowResourcesLoader: window {0} from {1} loaded.", w.Key.ToString(), w.Value.ResourcesPath);
						yield return new WaitForEndOfFrame();
					} else {
						Debug.LogErrorFormat("WindowResourcesLoader: window {0} from {1} could not be loaded.", w.Key.ToString(), w.Value.ResourcesPath);
					}									
				}
			}

			yield return null;
		}
	}

	public class WindowInfo {
		public Type       Type            = null;
		public string     ResourcesPath   = string.Empty;
		public GameObject LoadedResources = null;

		public bool IsResourcesLoaded { get { return LoadedResources; } }

		public void LoadFromResources() {
			LoadedResources = Resources.Load<GameObject>(ResourcesPath);
		}
	}

}

