using UnityEngine;

public class HideCursor : MonoBehaviour
{
	private void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}