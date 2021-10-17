using Tools.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickPlayAudio : MonoBehaviour
{
	[SerializeField] private bool autoAssignButton;
	[SerializeField] private AudioExpress sound;

	private Button button;

	protected void Awake()
	{
		if (autoAssignButton)
		{
			button = GetComponent<Button>();
			if (button != null)
			{
				button.onClick.AddListener(Play);
			}
		}
	}

	public void Play()
	{
		sound.Play();
	}
}