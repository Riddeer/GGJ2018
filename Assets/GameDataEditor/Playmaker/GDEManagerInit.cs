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

#if GDE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.InitActionTooltip)]
    public class GDEManagerInit : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.FsmString)]
        [Tooltip(GDMConstants.GDEDataFilenameTooltip)]
        public FsmString GDEDataFileName;

		[UIHint(UIHint.FsmBool)]
		[Tooltip(GDMConstants.EncryptedCheckboxTooltip)]
		public FsmBool Encrypted;

        public override void Reset()
        {
            GDEDataFileName = null;
        }
        
        public override void OnEnter()
        {
            try
            {
                if (!GDEDataManager.Init(GDEDataFileName.Value, Encrypted.Value))
                    LogError(GDMConstants.ErrorNotInitialized + " " + GDEDataFileName.Value);
            }
            catch(UnityException ex)
            {
                LogError(ex.ToString());
            }
            finally
            {
                Finish();       
            }
        }
    }
}

#endif
