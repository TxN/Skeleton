using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMGCore {
	public abstract class BaseStateController  {
		protected GameManager Owner { get; }

		protected BaseStateController(GameManager owner) {
			Owner = owner;
		}

		public abstract string DebugName { get; }

		public virtual void Init()       { }
		public virtual void PostInit()   { }

		public virtual void Load()       { }
		public virtual void PostLoad()   { }

		public virtual void Save()       { }

		public virtual void Update()     { }
		public virtual void LateUpdate() { }
		public virtual void RareUpdate() { }
		public virtual void Reset()      { }
	}

	public class StateController<T> : BaseStateController where T : StateController<T> {
		public static T Instance { get; private set; }

		string _debugName = string.Empty;

		public override string DebugName {
			get {
				if ( string.IsNullOrEmpty(_debugName) ) {
					_debugName = typeof(T).Name;
				}
				return _debugName;
			}
		}

		protected StateController(GameManager owner) : base(owner) {
			if ( Instance != null ) {
				var text = string.Format(
					"StateController<{0}>. Instance already created!",
					typeof(T).Name);
				throw new UnityException(text);
			}
			Instance = this as T;
		}

		public override void Reset() {
			Instance = null;
		}
	}
}

