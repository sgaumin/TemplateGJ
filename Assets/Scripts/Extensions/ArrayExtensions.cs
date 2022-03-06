using System;
using System.Linq;

public static class ArrayExtensions
{
	// The random number generator
	public static Random defaultRNG = new Random();

	/// <summary>
	/// Create a 2D jagged array from a 1D array/
	/// </summary>
	/// <typeparam name="T">the generic type</typeparam>
	/// <param name="array">The 1D array</param>
	/// <returns>The 2D jagged array</returns>
	public static T[][] AddJaggedDimension<T>(this T[] array)
	{
		T[][] extended = new T[array.Length][];
		for (int i = 0; i < array.Length; i++)
		{
			extended[i] = new T[] { array[i] };
		}

		return extended;
	}

	/// <summary>
	/// Populate an array with one value. This method mutates the array.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array">The array to populate</param>
	/// <param name="value">The value to set in each cell</param>
	public static void Populate<T>(this T[] array, T value)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = value;
		}
	}

	/// <summary>
	/// Populates randomly chosen slots of an array.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array">The array to populate.</param>
	/// <param name="count">How many slots to populate.</param>
	/// <param name="mapper">Function used to populate a particular slot.</param>
	/// <remarks>
	/// This method populates exactly <paramref name="count"/> unique slots of the array. A slot is
	/// populated at most once: <paramref name="mapper"/> will not be called more than once for a
	/// particular slot. Slots that have not been populated equal to
	/// <code>default(T)</code>
	/// .
	/// </remarks>
	public static void PopulateRandom<T>(this T[,] array, int count, Func<int, int, T> mapper, Random rng = null)
	{
		if (rng == null)
		{
			rng = defaultRNG;
		}

		// Requires that the array can even fit "count" items.
		int di = array.GetLength(0);
		int dj = array.GetLength(1);
		int maxCount = di * dj;
		if (count > maxCount)
		{
			throw new ArgumentOutOfRangeException("count", String.Format("Array cannot fit more than {0} elements.", maxCount));
		}

		// Randomly populates the array.
		bool[,] draws = new bool[di, dj];
		for (int c = 0; c < count; c++)
		{
			int i, j;

			do
			{
				// Draws a position to populate.
				i = rng.Next(0, di);
				j = rng.Next(0, dj);
			} while (draws[i, j]);

			// Populates the position.
			draws[i, j] = true;
			array[i, j] = mapper(i, j);
		}
	}

	/// <summary>
	/// Searches for the specified object and returns the coordinates of its first occurrence in a
	/// multi-dimensional array.
	/// </summary>
	/// <param name="array">The multi-dimensional array to search.</param>
	/// <param name="element">The object to locate.</param>
	/// <returns>
	/// The indices, in each dimension, of the first occurrence of <paramref name="element"/> in
	/// <paramref name="array"/>, if found; otherwise, null.
	/// </returns>
	public static int[] IndicesOf(this Array array, object element)
	{
		// https://stackoverflow.com/questions/3260935/finding-position-of-an-element-in-a-two-dimensional-array

		// Fast implementation if the array is mono-dimensional.
		if (array.Rank == 1)
		{
			int idx = Array.IndexOf(array, element);

			return idx >= 0 ? new int[] { idx } : null;
		}

		// Looks for the object in the array.
		var found = array.OfType<object>()
						  .Select((v, i) => new { v, i })
						  .FirstOrDefault(s => s.v.Equals(element));
		if (found == null)
		{
			// The object is not in the array.
			return null;
		}

		// Gets the coordinates of the object in the multi-dimensional array.
		int[] indexes = new int[array.Rank];
		int last = found.i;
		int lastLength = Enumerable.Range(0, array.Rank)
								   .Aggregate(1, (a, v) => a * array.GetLength(v));
		for (var rank = 0; rank < array.Rank; rank++)
		{
			lastLength /= array.GetLength(rank);
			int value = last / lastLength;
			last -= value * lastLength;

			int index = value + array.GetLowerBound(rank);
			if (index > array.GetUpperBound(rank))
			{
				throw new IndexOutOfRangeException();
			}
			indexes[rank] = index;
		}

		return indexes;
	}

	/// <summary>
	/// Checks whether a 2D array is symmetrical on the X or Y axis
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array">The array</param>
	/// <returns>True if the array is symmetrical on the X or Y axis</returns>
	public static bool IsSymmetrical<T>(this T[,] array)
	{
		// Check for symmetricity around column axis first, then row axis
		return array.IsSymmetrical(true) || array.IsSymmetrical(false);
	}

	/// <summary>
	/// Checks whether a 2D array is symmetrical on the given axis
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array">The array</param>
	/// <param name="onColumnAxis">Tell whether we check on the row or column axis</param>
	/// <returns>True if the array is symmetrical on the X or Y axis</returns>
	public static bool IsSymmetrical<T>(this T[,] array, bool onColumnAxis)
	{
		// #rows, then #cols
		int iN = array.GetLength(onColumnAxis ? 0 : 1);
		// #cols, then #rows
		int jN = array.GetLength(onColumnAxis ? 1 : 0);
		// Half the number of columns or rows
		int jNhalf = (int)System.Math.Floor(jN / 2f);

		for (int i = 0; i < iN; i++)
		{
			for (int j = 0; j < jNhalf; j++)
			{
				// Check if value is symmetric around current axis
				if (onColumnAxis && !array[i, j].Equals(array[i, jN - j - 1])
				|| !onColumnAxis && !array[j, i].Equals(array[jN - j - 1, i]))
				{
					return false;
				}
			}
		}

		return true;
	}
}