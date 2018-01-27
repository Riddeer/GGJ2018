/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using GameDataEditor;

#if GDE_ICODE_SUPPORT

namespace ICode.Actions
{
    [Category(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.InitActionTooltip)]
    public class GDEManagerInit : StateAction
    {
		[Tooltip(GDMConstants.GDEDataFilenameTooltip)]
        public FsmString GDEDataFileName;

		[Tooltip(GDMConstants.EncryptedCheckboxTooltip)]
		public FsmBool Encrypted;

        public override void OnEnter()
        {
			try
			{
				if (!GDEDataManager.Init(GDEDataFileName.Value, Encrypted.Value))
					Debug.LogError(GDMConstants.ErrorNotInitialized + " " + GDEDataFileName.Value);
			}
			catch(UnityException ex)
			{
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
