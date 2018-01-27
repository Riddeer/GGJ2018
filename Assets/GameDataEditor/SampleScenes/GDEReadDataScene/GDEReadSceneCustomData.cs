/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the Game Data Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//
//      This file was generated from this data file:
//      /Projects/gde_unity_4.5/Assets/GameDataEditor/SampleScenes/GDEReadDataScene/Resources/read_scene_data.json
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;

using GameDataEditor;

namespace GameDataEditor
{
    public class GDEReadSceneCustomData : IGDEData
    {
        static string descriptionKey = "description";
		string _description;
        public string description
        {
            get { return _description; }
            set {
                if (_description != value)
                {
                    _description = value;
					GDEDataManager.SetString(_key, descriptionKey, _description);
                }
            }
        }

        public GDEReadSceneCustomData(string key) : base(key)
        {
            GDEDataManager.RegisterItem(this.SchemaName(), key);
        }
        public override Dictionary<string, object> SaveToDict()
		{
			var dict = new Dictionary<string, object>();
			dict.Add(GDMConstants.SchemaKey, "ReadSceneCustom");
			
            dict.Merge(true, description.ToGDEDict(descriptionKey));
            return dict;
		}

        public override void UpdateCustomItems(bool rebuildKeyList)
        {
        }

        public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
        {
            _key = dataKey;

			if (dict == null)
				LoadFromSavedData(dataKey);
			else
			{
                dict.TryGetString(descriptionKey, out _description);
                LoadFromSavedData(dataKey);
			}
		}

        public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			
            _description = GDEDataManager.GetString(_key, descriptionKey, _description);
        }

        public GDEReadSceneCustomData ShallowClone()
		{
			string newKey = Guid.NewGuid().ToString();
			GDEReadSceneCustomData newClone = new GDEReadSceneCustomData(newKey);

            newClone.description = description;

            return newClone;
		}

        public GDEReadSceneCustomData DeepClone()
		{
			GDEReadSceneCustomData newClone = ShallowClone();
            return newClone;
		}

        public void Reset_description()
        {
            GDEDataManager.ResetToDefault(_key, descriptionKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(descriptionKey, out _description);
        }

        public void ResetAll()
        {
            GDEDataManager.ResetToDefault(_key, descriptionKey);


            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            LoadFromDict(_key, dict);
        }
    }
}
