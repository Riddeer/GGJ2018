/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameDataEditor
{
    public abstract partial class IGDEData
    {
		public IGDEData()
		{
			_key = string.Empty;
		}

		public IGDEData(string key)
		{
			Dictionary<string, object> dict;
			if (GDEDataManager.Get(key, out dict))
				LoadFromDict(key, dict);
			else
				LoadFromSavedData(key);
		}

		protected string _key;
		public string Key
		{
			get { return _key; }
			private set { _key = value; }
		}

        public abstract void LoadFromDict(string key, Dictionary<string, object> dict);
		public abstract void LoadFromSavedData(string key);

        public virtual void UpdateCustomItems(bool rebuildKeyList) {}
        public virtual Dictionary<string, object> SaveToDict() { return null; }
    }
}

