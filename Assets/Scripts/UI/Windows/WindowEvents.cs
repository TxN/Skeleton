namespace SMGCore.Windows.Events {
	public struct Event_WindowStateChanged {
		public WindowController Window;
		public WindowState State;
		public bool Force;

		public Event_WindowStateChanged(WindowController window, WindowState state, bool force) {
			Window = window;
			State = state;
			Force = force;
		}
	}

	public struct Event_WindowAppearing {
		public WindowController Window;

		public Event_WindowAppearing(WindowController window) {
			Window = window;
		}
	}

	public struct Event_WindowShown {
		public WindowController Window;

		public Event_WindowShown(WindowController window) {
			Window = window;
		}
	}

	public struct Event_WindowHiding {
		public WindowController Window;

		public Event_WindowHiding(WindowController window) {
			Window = window;
		}
	}

	public struct Event_WindowHidden {
		public WindowController Window;

		public Event_WindowHidden(WindowController window) {
			Window = window;
		}
	}

	public struct Event_WindowTopWindowChange {
		public WindowController Window;

		public Event_WindowTopWindowChange(WindowController window) {
			Window = window;
		}
	}

}
