using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class GameObjectExtensions
{
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
	}

	public static List<T> GetAllComponents<T>(this GameObject gameObject) where T : Component
	{
		List<T> allColliders = new List<T>();

		gameObject.GetComponents<T>()?.ForEach(x => allColliders.Add(x));
		gameObject.GetComponentsInChildren<T>()?.ForEach(x => allColliders.Add(x));

		return allColliders;
	}

	public static bool TryGetComponent<T>(this GameObject gameObject, out T val) where T : Component
	{
		val = gameObject.GetComponent<T>();
		return val == null;
	}

	public static bool TryGetComponent<T>(this Component component, out T val) where T : Component
	{
		val = component.GetComponent<T>();
		return val == null;
	}

	public static void EnableColliders(this GameObject gameObject)
	{
		gameObject.GetAllComponents<Collider>()?.ForEach(x => x.enabled = true);
		gameObject.GetAllComponents<Collider2D>()?.ForEach(x => x.enabled = true);
	}

	public static void DisableColliders(this GameObject gameObject)
	{
		gameObject.GetAllComponents<Collider>()?.ForEach(x => x.enabled = false);
		gameObject.GetAllComponents<Collider2D>()?.ForEach(x => x.enabled = false);
	}

	public static void FadIn(this GameObject gameObject, float duration = 1f)
	{
		gameObject.GetAllComponents<SpriteRenderer>()?.ForEach(x => x.WithAlpha(0f));
		gameObject.GetAllComponents<Image>()?.ForEach(x => x.WithAlpha(0f));
		gameObject.GetAllComponents<TextMeshPro>()?.ForEach(x => x.WithAlpha(0f));
		gameObject.GetAllComponents<TextMeshProUGUI>()?.ForEach(x => x.WithAlpha(0f));

		gameObject.GetAllComponents<SpriteRenderer>()?.ForEach(x => x.DOFade(1f, duration));
		gameObject.GetAllComponents<Image>()?.ForEach(x => x.DOFade(1f, duration));
		gameObject.GetAllComponents<TextMeshPro>()?.ForEach(x => x.DOFade(1f, duration));
		gameObject.GetAllComponents<TextMeshProUGUI>()?.ForEach(x => x.DOFade(1f, duration));
	}

	public static void FadOut(this GameObject gameObject, float duration = 1f)
	{
		gameObject.GetAllComponents<SpriteRenderer>()?.ForEach(x => x.DOFade(0f, duration));
		gameObject.GetAllComponents<Image>()?.ForEach(x => x.DOFade(0f, duration));
		gameObject.GetAllComponents<TextMeshPro>()?.ForEach(x => x.DOFade(0f, duration));
		gameObject.GetAllComponents<TextMeshProUGUI>()?.ForEach(x => x.DOFade(0f, duration));
	}
}