/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if UNITY_WEBPLAYER
/* 
 * ArrayPrefs2 v 1.4
 * http://wiki.unity3d.com/index.php/ArrayPrefs2
 *
 * Added functionality to save/load Vector4 type
 * Added functionality to support 2 Dimensional Lists
 * Split up into multiple files
 * Changed type from Array to List
 *
 * This File and its Content is available under Creative Commons Attribution Share Alike (http://creativecommons.org/licenses/by-sa/3.0/).
 * 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameDataEditor 
{
	public partial class GDEPPX
	{
		static int endianDiff1;
		static int endianDiff2;
		static int idx;
		static byte [] byteBlock;
		
		enum ListType {
			Float, 
			Int32, 
			Bool, 
			String, 
			Vector2, 
			Vector3, 
			Quaternion, 
			Color, 
			Vector4,
			Float_2D,
			Int32_2D,
			Bool_2D,
			String_2D,
			Vector2_2D,
			Vector3_2D,
			Vector4_2D,
			Quaternion_2D,
			Color_2D
		}

		static void Initialize (bool is2DList = false)
		{
			if (System.BitConverter.IsLittleEndian)
			{
				endianDiff1 = 0;
				endianDiff2 = 0;
			}
			else
			{
				endianDiff1 = 3;
				endianDiff2 = 1;
			}
			if (byteBlock == null)
			{
				byteBlock = new byte[4];
			}

			if (is2DList)
				idx = 0;
			else
				idx = 1;
		}
		
		static void SplitLong(long input, out int lowBits, out int highBits)
		{
			// unsigned everything, to prevent loss of sign bit.
			lowBits = (int)(uint)(ulong)input;
			highBits = (int)(uint)(input >> 32);
		}
		
		public static void ShowArrayType (String key)
		{
			var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
			if (bytes.Length > 0)
			{
				ListType arrayType = (ListType)bytes[0];
				Debug.Log (string.Format(GDMConstants.ErrorCorruptPrefFormat, key, arrayType.ToString()));
			}
		}

		static bool SaveBytes (String key, byte[] bytes)
		{
			try
			{
				PlayerPrefs.SetString (key, System.Convert.ToBase64String (bytes));
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
#endif
