using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickPlayAudio : MonoBehaviour
{
	[SerializeField] private bool autoAssignButton;
	[SerializeField] private AudioExpress sound;

	protected void Start()
	{
		if (autoAssignButton)
		{
			var button = GetComponent<Button>();
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