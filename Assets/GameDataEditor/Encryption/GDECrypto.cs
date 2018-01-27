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
using System.IO;
using System.Text;
using System.Security.Cryptography;

using GameDataEditor;

namespace GameDataEditor
{
    /// <summary>
    /// AES is a symmetric 256-bit encryption algorthm.
    /// Read more: http://en.wikipedia.org/wiki/Advanced_Encryption_Standard
    /// </summary>
    [Serializable]
	public class GDECrypto
    {
		public const int KEY_LENGTH = 256;
	
		public byte[] Salt;
		public byte[] IV;
		public string Pass;

        /// <summary>
        /// Decrypts an AES-encrypted string.
        /// </summary>
        /// <param name="cipherText">Text to be decrypted</param>
        /// <returns>A decrypted string</returns>
        public string Decrypt(byte[] cipherTextBytes)
        {
			string content = string.Empty;

			try
			{
				byte[] keyBytes = new Rfc2898DeriveBytes(Pass, Salt).GetBytes(KEY_LENGTH / 8);
	            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

	            using (RijndaelManaged symmetricKey = new RijndaelManaged())
	            {
	                symmetricKey.Mode = CipherMode.CBC;

					using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, IV))
					{
	                    using (MemoryStream memStream = new MemoryStream(cipherTextBytes))
	                    {
	                        using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
	                        {
	                            int byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
	                            content = Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
	                        }
	                    }
	                }
	            }
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}

			return content;
        }
    }
}
