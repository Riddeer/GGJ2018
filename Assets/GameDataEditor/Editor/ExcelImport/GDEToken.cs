/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;

namespace GameDataEditor
{
    public class GDEToken
    {
        public GDEToken(string type, string value, GDETokenPosition position)
        {
            Type = type;
            Value = value;
            Position = position;
        }

        public GDETokenPosition Position { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("Token: {{ Type: \"{0}\", Value: \"{1}\", Position: {{ Index: \"{2}\", Line: \"{3}\", Column: \"{4}\" }} }}", Type, Value, Position.Index, Position.Line, Position.Column);
        }
    }
}
