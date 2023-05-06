using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
	public static class UIExtensions
	{
		/// <summary>
		/// Sets a new alpha value on a given TextMeshPro.
		/// </summary>
		/// <param name="t">TextMeshPro reference</param>
		/// <param name="a">The new alpha value</param>
		/// <returns>A new color {t.rgb, a}</returns>
		public static TextMeshPro SetAlpha(this TextMeshPro t, float a)
		{
			t.color = t.color.WithAlpha(a);
			return t;
		}

		/// <summary>
		/// Sets a new alpha value on a given TextMeshProUGUI.
		/// </summary>
		/// <param name="t">TextMeshProUGUI reference</param>
		/// <param name="a">The new alpha value</param>
		/// <returns>A new color {t.rgb, a}</returns>
		public static TextMeshProUGUI SetAlpha(this TextMeshProUGUI t, float a)
		{
			t.color = t.color.WithAlpha(a);
			return t;
		}

		/// <summary>
		/// Sets a new alpha value on a given SpriteRenderer.
		/// </summary>
		/// <param name="s">SpriteRenderer reference</param>
		/// <param name="a">The new alpha value</param>
		/// <returns>A new color {s.rgb, a}</returns>
		public static SpriteRenderer SetAlpha(this SpriteRenderer s, float a)
		{
			s.color = s.color.WithAlpha(a);
			return s;
		}

		/// <summary>
		/// Sets a new alpha value on a given Image.
		/// </summary>
		/// <param name="i">Image reference</param>
		/// <param name="a">The new alpha value</param>
		/// <returns>A new color {i.rgb, a}</returns>
		public static Image SetAlpha(this Image i, float a)
		{
			i.color = i.color.WithAlpha(a);
			return i;
		}

		/// <summary>
		/// Create a new color from the given color and a new value for its alpha.
		/// </summary>
		/// <param name="c">The source color</param>
		/// <param name="a">The new alpha value</param>
		/// <returns>A new color {c.rgb, a}</returns>
		public static Color WithAlpha(this Color c, float a)
		{
			c.a = a;
			return c;
		}
	}
}