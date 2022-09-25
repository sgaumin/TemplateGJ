using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace AnimExpress
{
	public static class AnimationExpressCreatorTool
	{
		private const string SUBFODLER = "Animations";

		[MenuItem("CONTEXT/TextureImporter/Create Animations", priority = 1000)]
		private static void CopySettings(MenuCommand command)
		{
			TextureImporter textureImporter = (TextureImporter)command.context;
			string path = AssetDatabase.GetAssetPath(textureImporter);
			string folderPath = Path.GetDirectoryName(path);
			string fileName = Path.GetFileName(path).Replace(".png", "");
			Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

			List<Frame> frames = new List<Frame>();
			for (int i = 0; i < sprites.Length; i++)
			{
				Frame frame = new Frame(sprites[i]);
				frames.Add(frame);
			}
			AnimationExpress animClip = new AnimationExpress(frames);

			string newFodlerPath = Path.Combine(folderPath, SUBFODLER);
			if (!Directory.Exists(newFodlerPath))
			{
				Directory.CreateDirectory(newFodlerPath);
			}

			string newAssetName = Path.Combine(newFodlerPath, $"{fileName}.asset");
			var existingAsset = AssetDatabase.LoadAssetAtPath<AnimationExpress>(newAssetName);
			if (existingAsset is null)
			{
				AssetDatabase.CreateAsset(animClip, newAssetName);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}
	}
}

#endif