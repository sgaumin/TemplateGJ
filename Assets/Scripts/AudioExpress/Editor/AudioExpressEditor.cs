﻿using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[CustomPropertyDrawer(typeof(AudioExpress))]
public class AudioExpressEditor : PropertyDrawer
{
	private float offset = EditorGUIUtility.singleLineHeight * 0.3f;
	private float offsetClipsArray;
	private float offsetPitch;
	private float offsetLoop;
	private float offsetAutoDestroy;
	private bool isExpanded;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return isExpanded ? base.GetPropertyHeight(property, label) * 8.2f + offsetClipsArray + offsetPitch + offsetLoop + offsetAutoDestroy + EditorGUIUtility.singleLineHeight * 1f : EditorGUIUtility.singleLineHeight * 1f;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, GUIContent.none, property);

		// References
		SerializedProperty isUsingClips = property.FindPropertyRelative("isUsingClips");
		SerializedProperty clip = property.FindPropertyRelative("clip");
		SerializedProperty clips = property.FindPropertyRelative("clips");
		SerializedProperty mixerGroup = property.FindPropertyRelative("mixerGroup");
		SerializedProperty loopType = property.FindPropertyRelative("loopType");
		SerializedProperty timeBetweenLoop = property.FindPropertyRelative("timeBetweenLoop");
		SerializedProperty pitchVariation = property.FindPropertyRelative("pitchVariation");
		SerializedProperty autoDestroy = property.FindPropertyRelative("autoDestroy");
		SerializedProperty multiplier = property.FindPropertyRelative("multiplier");

		// Calculate rects
		Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		Rect isUsingClipsRect = new Rect(position.x, position.y + offset + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
		Rect clipRect = new Rect(position.x, position.y + offset + EditorGUIUtility.singleLineHeight * 2.1f, position.width, EditorGUIUtility.singleLineHeight);
		Rect mixerGroupRect = new Rect(position.x, position.y + offset + offsetClipsArray + EditorGUIUtility.singleLineHeight * 3.2f, position.width, EditorGUIUtility.singleLineHeight);
		Rect isPitchModifiedRect = new Rect(position.x, position.y + offset + offsetClipsArray + EditorGUIUtility.singleLineHeight * 5f, position.width, EditorGUIUtility.singleLineHeight);
		Rect pitchMaxVariationRect = new Rect(position.x, position.y + offset + offsetClipsArray + EditorGUIUtility.singleLineHeight * 6.2f, position.width, EditorGUIUtility.singleLineHeight * 1.85f);
		Rect loopTypeRect = new Rect(position.x, position.y + offset + offsetClipsArray + offsetPitch + EditorGUIUtility.singleLineHeight * 6.2f, position.width, EditorGUIUtility.singleLineHeight);
		Rect timeBetweenLoopRect = new Rect(position.x, position.y + offset + offsetClipsArray + offsetPitch + EditorGUIUtility.singleLineHeight * 7.4f, position.width, EditorGUIUtility.singleLineHeight * 1.85f);
		Rect autoDestroyRect = new Rect(position.x, position.y + offset + offsetClipsArray + offsetPitch + offsetLoop + EditorGUIUtility.singleLineHeight * 7.4f, position.width, EditorGUIUtility.singleLineHeight);
		Rect multiplierRect = new Rect(position.x, position.y + offset + offsetClipsArray + offsetPitch + offsetLoop + EditorGUIUtility.singleLineHeight * 8.6f, position.width, EditorGUIUtility.singleLineHeight * 1.15f);

		// Draw label
		property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label);
		isExpanded = property.isExpanded;

		if (isExpanded)
		{
			EditorGUI.indentLevel++;

			EditorGUI.PropertyField(isUsingClipsRect, isUsingClips, new GUIContent("Multiple Clips"));
			if (isUsingClips.boolValue)
			{
				EditorGUI.PropertyField(clipRect, clips, true);
			}
			else
			{
				EditorGUI.PropertyField(clipRect, clip, GUIContent.none);
			}

			// Expand field
			if (isUsingClips.boolValue && clips.isExpanded)
			{
				offsetClipsArray = EditorGUIUtility.singleLineHeight * (1.3f + clips.arraySize * 0.115f + clips.arraySize);
			}
			else
			{
				offsetClipsArray = 0f;
			}

			EditorGUI.PropertyField(mixerGroupRect, mixerGroup, GUIContent.none);
			if (!isUsingClips.boolValue && clip.objectReferenceValue != null)
			{
				AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Sounds/Audio Mixer.mixer");
				AudioMixerGroup[] groups = mixer.FindMatchingGroups(clip.objectReferenceValue.name);
				if (groups.Length > 0)
				{
					mixerGroup.objectReferenceValue = groups[0];
				}
			}

			EditorGUI.PropertyField(isPitchModifiedRect, pitchVariation, new GUIContent("Pitch Variation"));

			EditorGUI.PropertyField(loopTypeRect, loopType, new GUIContent("Loop"));
			if (loopType.enumValueIndex == (int)AudioLoopType.Manuel)
			{
				EditorGUI.PropertyField(timeBetweenLoopRect, timeBetweenLoop);
				offsetLoop = EditorGUIUtility.singleLineHeight * 2.3f;
			}
			else
			{
				offsetLoop = 0f;
			}

			EditorGUI.PropertyField(autoDestroyRect, autoDestroy);
			if (autoDestroy.enumValueIndex != (int)AudioStopType.No)
			{
				EditorGUI.PropertyField(
				  multiplierRect, multiplier, new GUIContent(autoDestroy.enumValueIndex == (int)AudioStopType.StopAfterDuration ?
				  "Seconds" : "Play Count"));
				offsetAutoDestroy = EditorGUIUtility.singleLineHeight * 1.3f;
			}
			else
			{
				offsetAutoDestroy = 0f;
			}

			EditorGUI.indentLevel--;
		}

		EditorGUI.EndProperty();
	}
}