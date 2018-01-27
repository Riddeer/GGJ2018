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

#if GDE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    public abstract class GDEActionBase : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.FsmString)]
        [Tooltip(GDMConstants.ItemNameTooltip)]
        public FsmString ItemName;
        
        [RequiredField]
        [UIHint(UIHint.FsmString)]
        [Tooltip(GDMConstants.FieldNameTooltip)]
        public FsmString FieldName;

        public override void Reset()
        {
            ItemName = null;
            FieldName = null;
        }
        
        public abstract override void OnEnter();
    }
}

#endif
