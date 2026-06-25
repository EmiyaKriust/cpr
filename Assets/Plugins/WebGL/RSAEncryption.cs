using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

public class RSAEncryption
{
    public static RSAParameters publicKey;
    public static RSAParameters privateKey;

    public static void ImportKeys(string publicKeyBase, string privateKeyBase)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(publicKeyBase);
            publicKey = rsa.ExportParameters(false);
            rsa.FromXmlString(privateKeyBase);
            privateKey = rsa.ExportParameters(true);
        }
    }

    public static string Encrypt(string data, RSAParameters publicKey)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(publicKey);
            byte[] encryptedData = rsa.Encrypt(dataBytes, false);
            return Convert.ToBase64String(encryptedData);
        }
    }

    public static string Decrypt(string encryptedData, RSAParameters privateKey)
    {
        byte[] encryptedDataBytes = Convert.FromBase64String(encryptedData);

        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            byte[] decryptedData = rsa.Decrypt(encryptedDataBytes, false);
            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}
