/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using GameDataEditor;

using Object = UnityEngine.Object;

namespace GameDataEditor {
	public class GDEEncryptionWindow : EditorWindow {

		GUIStyle headerStyle;
		Texture warningIcon;

		float x;
		float y;

		void Reset()
		{
			x = 5;
			y = 5;
		}

		void SetStyles()
		{
			if (headerStyle.IsNullOrEmpty())
			{
				headerStyle = new GUIStyle(GUI.skin.label);
				headerStyle.fontStyle = FontStyle.Bold;
				headerStyle.fontSize = 16;
			}
			
			if (warningIcon == null)
			{
				warningIcon = (Texture)AssetDatabase.LoadAssetAtPath(GDESettings.RelativeRootDir + Path.DirectorySeparatorChar +
					GDEConstants.WarningIconTexturePath, typeof(Texture));
			}
		}

		void OnGUI()
		{
			SetStyles();

			Reset();

			GUIContent content = new GUIContent(GDEConstants.EncryptionTitle);
			Vector2 size = headerStyle.CalcSize(content);
			EditorGUI.LabelField(new Rect(x, y, size.x, size.y), GDEConstants.EncryptionTitle, headerStyle);
			y += size.y + 20;

			GUI.Box(new Rect(x, y, warningIcon.width/2, warningIcon.height/2), warningIcon);
			x += warningIcon.width/2 + 6;

			content.text = GDEConstants.EncryptionWarning;
			size = GUI.skin.label.CalcSize(content);
			EditorGUI.LabelField(new Rect(x, y, size.x, size.y), content);
			y += size.y;

			content.text = GDEItemManager.DataFilePath;
			size = EditorStyles.objectField.CalcSize(content);
			if (GUI.Button(new Rect(x, y, size.x, size.y), content, EditorStyles.objectField))
			{
				string relativePath = GDEItemManager.DataFilePath.Replace(Environment.CurrentDirectory, string.Empty).TrimLeadingDirChars();
				Object gdeDataAsset = AssetDatabase.LoadAssetAtPath(relativePath, typeof(TextAsset));
				EditorApplication.ExecuteMenuItem("Window/Project");
				EditorGUIUtility.PingObject(gdeDataAsset);
			}
			y += size.y + 5;

			content.text = GDEConstants.EncryptionFileLabel;
			size = GUI.skin.label.CalcSize(content);
			EditorGUI.LabelField(new Rect(x, y, size.x, size.y), content);
			y += size.y;

			content.text = GDEItemManager.EncryptedFilePath;
			size = EditorStyles.objectField.CalcSize(content);
			if (GUI.Button(new Rect(x, y, size.x, size.y), content, EditorStyles.objectField))
			{
				string relativePath = GDEItemManager.EncryptedFilePath.Replace(Environment.CurrentDirectory, string.Empty).TrimLeadingDirChars();
				Object gdeDataAsset = AssetDatabase.LoadAssetAtPath(relativePath, typeof(TextAsset));
				EditorApplication.ExecuteMenuItem("Window/Project");
				EditorGUIUtility.PingObject(gdeDataAsset);
			}
			y += size.y + 5;

			content.text = GDEConstants.OkLbl;
			size = headerStyle.CalcSize(content);
			if (GUI.Button(new Rect(position.width-size.x-10, position.height-size.y-10, size.x, size.y), content))
				this.Close();
		}
	}
}
