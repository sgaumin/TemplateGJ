using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : GameSystem
{
	public static Game Instance { get; private set; }

	public GameStates GameState { get; private set; } = GameStates.Play;

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}

	protected override void Update()
	{
		base.Update();
	}
}
