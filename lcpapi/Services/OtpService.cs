namespace lcpapi.Services;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public interface IOtpService
{
    string GenerateSecret(int length = 20);
    string GetProvisioningUri(string account, string issuer, string secret);
    bool ValidateTotp(string secret, string code, int step = 30, int digits = 6, int window = 1);
}

public class OtpService : IOtpService
{
    // Generates a random base32 secret
    public string GenerateSecret(int length = 20)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"; // base32 alphabet
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        var sb = new StringBuilder(length);
        foreach (var b in bytes)
            sb.Append(alphabet[b % alphabet.Length]);
        return sb.ToString();
    }

    // Returns the otpauth URI for QR code generation
    public string GetProvisioningUri(string account, string issuer, string secret)
    {
        // URL-encode account and issuer
        var acct = Uri.EscapeDataString(account);
        var iss = Uri.EscapeDataString(issuer);
        return $"otpauth://totp/{iss}:{acct}?secret={secret}&issuer={iss}&algorithm=SHA1&digits=6&period=30";
    }

    // Validate TOTP using HMAC-SHA1
    public bool ValidateTotp(string secret, string code, int step = 30, int digits = 6, int window = 1)
    {
        if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(code)) return false;

        // decode base32 secret
        byte[] key;
        try { key = Base32Decode(secret); } catch { return false; }

        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var counter = unixTime / step;

        for (long i = -window; i <= window; i++)
        {
            var hash = HmacSha1(key, BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((long)(counter + i))));
            int offset = hash[hash.Length - 1] & 0x0F;
            int binary = ((hash[offset] & 0x7f) << 24)
                        | ((hash[offset + 1] & 0xff) << 16)
                        | ((hash[offset + 2] & 0xff) << 8)
                        | (hash[offset + 3] & 0xff);
            int otp = binary % (int)Math.Pow(10, digits);
            if (otp.ToString(new string('0', digits)) == code) return true;
        }
        return false;
    }

    private static byte[] HmacSha1(byte[] key, byte[] data)
    {
        using var hmac = new HMACSHA1(key);
        return hmac.ComputeHash(data);
    }

    // Minimal base32 decoder
    private static byte[] Base32Decode(string base32)
    {
        base32 = base32.TrimEnd('=');
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var bytes = new List<byte>();
        int cur = 0, bits = 0;
        foreach (char c in base32.ToUpperInvariant())
        {
            int val = alphabet.IndexOf(c);
            if (val < 0) throw new FormatException("Invalid base32 character");
            cur = (cur << 5) | val;
            bits += 5;
            if (bits >= 8)
            {
                bits -= 8;
                bytes.Add((byte)((cur >> bits) & 0xFF));
            }
        }
        return bytes.ToArray();
    }
}
