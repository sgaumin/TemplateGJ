using DG.Tweening;
using System;
using UnityEngine;
using Utils;

public class SceneBase : Singleton<SceneBase>
{
	public event Action OnStart;

	public event Action OnEnd;

	public event Action OnPause;

	[SerializeField] private LevelLoader levelLoader;
	[SerializeField] private MusicPlayer music;
	[SerializeField] private VisualEffectsHandler effectHandler;

	private SceneState state;

	public SceneState State
	{
		get => state;
		set
		{
			state = value;

			switch (value)
			{
				case SceneState.Start:
					OnStart?.Invoke();
					break;

				case SceneState.End:
					OnEnd?.Invoke();
					break;

				case SceneState.Pause:
					OnPause?.Invoke();
					break;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		Application.targetFrameRate = 60;

		DOTween.Init();
		DOTween.defaultAutoPlay = AutoPlay.All;

		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	#region Level Loading

	public void ReloadScene() => levelLoader.Reload();

	public void LoadNextScene() => levelLoader.LoadNextScene();

	public void LoadPrevious() => levelLoader.LoadPrevious();

	public void LoadMenu() => levelLoader.LoadMenu();

	public void LoadSceneTransition(SceneLoading loading) => levelLoader.LoadSceneTransition(loading);

	public void Quit() => levelLoader.Quit();

	#endregion Level Loading

	#region Post-Processing and Effects

	public void GenerateImpulse()
	{
		effectHandler.GenerateImpulse();
	}

	#endregion Post-Processing and Effects

	#region Audio

	public bool IsMuted => music.IsMuted;

	public void Mute()
	{
		music.Mute();
	}

	#endregion Audio
}