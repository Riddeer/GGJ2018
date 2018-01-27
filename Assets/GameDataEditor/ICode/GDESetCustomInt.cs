/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections.Generic;
using GameDataEditor;

#if GDE_ICODE_SUPPORT

namespace ICode.Actions
{
	[Category(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.SetCustomIntActionTooltip)]
	public class GDESetCustomInt : GDEActionBase
	{   
		[Tooltip(GDMConstants.IntCustomFieldTooltip)]
		public FsmString CustomField;
		
		public FsmInt IntValue;
		
		public override void OnEnter()
		{
			try
			{	
				Dictionary<string, object> data;
				string customKey;
				
				if (GDEDataManager.DataDictionary.ContainsKey(ItemName.Value))
				{
					GDEDataManager.Get(ItemName.Value, out data);
					data.TryGetString(FieldName.Value, out customKey);
					customKey = GDEDataManager.GetString(ItemName.Value, FieldName.Value, customKey);
				}
				else
				{
					// New Item Case
					customKey = GDEDataManager.GetString(ItemName.Value, FieldName.Value, string.Empty);
				}
				
				GDEDataManager.SetInt(customKey, CustomField.Value, IntValue.Value);
			}
			catch(UnityException ex)
			{
				Debug.LogError(string.Format(GDMConstants.ErrorSettingCustomValue, GDMConstants.IntType, ItemName.Value, FieldName.Value, CustomField.Value));
				Debug.LogError(ex.ToString());
			}
			finally
			{
				Finish();
			}
		}
	}
}

#endif

