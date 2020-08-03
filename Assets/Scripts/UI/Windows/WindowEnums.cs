namespace SMGCore.Windows {
	public enum WindowType {
		Unknown = 0,
		SettingsWindow = 1,
		MapWindow = 2,
		PauseWindow = 3,
		LoseWindow = 4,
		WinWindow = 5,
		AboutWindow = 6,
	}

	public enum WindowState {
		Hidden,
		Appearing,
		Active,
		Hiding,
		Suspended
	}
	public enum ShowAnimType {
		None,
		SlideUp,
		SlideDown,
		SlideLeft,
		SlideRight,
		ScaleUp
	}

	public enum HideAnimType {
		None,
		MoveDown,
		MoveUp,
		MoveLeft,
		MoveRight,
		ScaleDown
	}
}