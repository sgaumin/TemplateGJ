using Tools;
using UnityEngine;

public class LoadBankAndScene : MonoBehaviour
{
	[SerializeField, FMODUnity.BankRef] private string bankName;

	private void Awake()
	{
		FMODUnity.RuntimeManager.LoadBank(bankName, true);
	}

	void Update()
	{
		if (FMODUnity.RuntimeManager.HasBankLoaded(bankName))
		{
			LevelLoader.LoadNextLevel();
		}
	}
}
