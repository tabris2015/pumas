using UnityEngine;
using System.Collections;

public static class GameData{

	private static int nPumas, nGordos, nFlacos, coins;

	public static int NPumas 
	{
		get
		{
			return nPumas;
		}
		set
		{
			nPumas = value;
		}
	}

	public static int NGordos 
	{
		get
		{
			return nGordos;
		}
		set
		{
			nGordos = value;
		}
	}
	public static int NFlacos 
	{
		get
		{
			return nFlacos;
		}
		set
		{
			nFlacos = value;
		}
	}
	public static int Coins 
	{
		get
		{
			return coins;
		}
		set
		{
			coins = value;
		}
	}
}
