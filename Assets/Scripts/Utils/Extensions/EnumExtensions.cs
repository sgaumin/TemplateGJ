using System;

public static class EnumExtensions
{
	/// <summary>
	/// Parse enum in a simple way
	///
	/// Example: StatusEnum MyStatus = "Active".ToEnum<StatusEnum>();
	/// </summary>
	/// <typeparam name="T">Type</typeparam>
	/// <param name="value">string to parse</param>
	/// <returns></returns>
	public static T ToEnum<T>(this string value)
	{
		return (T)Enum.Parse(typeof(T), value, true);
	}
}