using UnityEngine;

public class DeactivateForWebGL : MonoBehaviour
{
	private void Awake()
	{
#if !UNITY_EDITOR && UNITY_WEBGL
			gameObject.SetActive(false);
#endif
	}
}