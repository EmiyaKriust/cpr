using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class RSAKeyGenerator:MonoBehaviour
{
    public static string ExportPublicKeyToXml(RSACryptoServiceProvider rsa)
    {
        return rsa.ToXmlString(false); // 仅导出公钥
    }

    public static string ExportPrivateKeyToXml(RSACryptoServiceProvider rsa)
    {
        return rsa.ToXmlString(true); // 导出私钥
    }

    public static void GenerateAndPrintKeys()
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048)) // 2048 是密钥长度
        {
            string publicKey = ExportPublicKeyToXml(rsa);
            string privateKey = ExportPrivateKeyToXml(rsa);

            Debug.Log("Public Key:");
            Debug.Log(publicKey);

            Debug.Log("Private Key:");
            Debug.Log(privateKey);
        }
    }
}