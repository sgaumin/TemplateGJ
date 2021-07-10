using Tools.Utils;
using UnityEngine;
using UnityEngine.UI;

public class PlayAudio : MonoBehaviour
{
	[SerializeField] private AudioExpress sound;

	[Header("Options")]
	[SerializeField] private bool autoAssignButton = false;

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