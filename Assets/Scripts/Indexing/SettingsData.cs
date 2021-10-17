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

public class SettingsData : BaseIndex
{
	private static SettingsData _instance;
	public static SettingsData Instance => GetOrLoad(ref _instance);

	// Set up your references below!
	public bool autoAssignScene = true;

	[Header("Animations")]
	public float sceneFadeDuration = 0.5f;

	[Header("Audio")]
	public float audioFadeDuration = 0.2f;
}
