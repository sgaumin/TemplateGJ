using System;
using Utils;

public class LeaderBoardEntry
{
	private string name;
	private float value;
	private long utx;

	public string Name => name;
	public float Value => value;
	public long UTX => utx;

	public LeaderBoardEntry(string name, float value, long utx)
	{
		this.name = name;
		this.value = value;
		this.utx = utx;
	}

	public DateTime GetDateTime()
	{
		return TimeManager.UnixTimeStampToDateTime(utx);
	}
}