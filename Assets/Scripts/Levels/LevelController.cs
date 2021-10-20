using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tools;
using Tools.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using static Facade;

public class LevelController : SceneBase
{
	public static LevelController Instance { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
	}

	protected void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}