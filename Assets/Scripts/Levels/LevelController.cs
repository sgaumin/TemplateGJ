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

public class LevelController : SceneBase, ISingleton
{
	public static LevelController Instance { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		// Setup all singletons present in the scene
		FindObjectsOfType<MonoBehaviour>().OfType<ISingleton>().OrderByDescending(x => x.GetSingletonPriority()).ForEach(x => x.OnSingletonSetup());
	}

	public int GetSingletonPriority()
	{
		return 100;
	}

	public void OnSingletonSetup()
	{
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