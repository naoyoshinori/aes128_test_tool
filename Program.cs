// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using Tomlyn;

var blockSize = 128;
var keySize = 128;
var padding = PaddingMode.Zeros;

var testcase_file = "testcase.toml";

void test_aes128(TestCase testcase)
{
    Console.Write("Name: " + testcase.name + "  ");
    Console.Write("Type: " + testcase.type + "  ");
    Console.Write("Mode: " + testcase.mode);
    Console.WriteLine();

    switch (testcase.mode)
    {
        case "encrypt":
            test_aes128_encrypt(testcase);
            break;
        case "decrypt":
            test_aes128_decrypt(testcase);
            break;
        default:
            Console.WriteLine("mode encrypt or decrypt :" + testcase.mode);
            break;
    }
}

void test_aes128_encrypt(TestCase testcase)
{
    using var aes = Aes.Create();
    aes.BlockSize = blockSize;
    aes.KeySize = keySize;
    aes.Padding = padding;
    aes.Key = [.. testcase.key];

    switch (testcase.type)
    {
        case "ECB":
            aes.Mode = CipherMode.ECB;
            break;
        case "CBC":
            aes.Mode = CipherMode.CBC;
            aes.IV = [.. testcase.ini_vector];
            break;
        default:
            Console.WriteLine("type ECB or CBC: " + testcase.type);
            return;
    }

    var encrypt = aes.CreateEncryptor();
    var memoryStream = new MemoryStream();
    var cryptStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);
    cryptStream.Write([.. testcase.data], 0, testcase.data.Count);
    cryptStream.FlushFinalBlock();
    byte[] encrypted = memoryStream.ToArray();

    Console.Write("Result: ");
    foreach (var b in encrypted)
    {
        Console.Write(b.ToString("X2") + "  ");
    }
    Console.WriteLine();
}

void test_aes128_decrypt(TestCase testcase)
{
    using var aes = Aes.Create();
    aes.BlockSize = blockSize;
    aes.KeySize = keySize;
    aes.Padding = padding;
    aes.Key = [.. testcase.key];

    switch (testcase.type)
    {
        case "ECB":
            aes.Mode = CipherMode.ECB;
            break;
        case "CBC":
            aes.IV = [.. testcase.ini_vector];
            aes.Mode = CipherMode.CBC;
            break;
        default:
            Console.WriteLine("type ECB or CBC: " + testcase.type);
            return;
    }

    byte[] decrypted = new byte[testcase.data.Count];

    var decryptor = aes.CreateDecryptor();
    var memoryStream = new MemoryStream([.. testcase.data]);
    var cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
    cryptStream.Read(decrypted, 0, decrypted.Length);

    Console.Write("Result: ");
    foreach (var b in decrypted)
    {
        Console.Write(b.ToString("X2") + "  ");
    }
    Console.WriteLine();
}

void main()
{
    if (!File.Exists(testcase_file))
    {
        Console.WriteLine("File not found. :" + testcase_file);
        return;
    }

    var toml_text = File.ReadAllText(testcase_file);

    var testcase_array = Toml.ToModel<TestCaseArray>(toml_text);

    foreach (var testcase in testcase_array.testcase)
    {
        test_aes128(testcase);
        Console.WriteLine();
    }
}

main();
