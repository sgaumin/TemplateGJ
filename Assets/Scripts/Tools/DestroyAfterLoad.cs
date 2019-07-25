using UnityEngine;

public class DestroyAfterLoad : MonoBehaviour
{
	public void Initialize(float duration) => Destroy(gameObject, duration);
}
