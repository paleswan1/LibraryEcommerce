using System.Reflection;
using System.ComponentModel;
using LibraryEcom.Domain.Common;
using Microsoft.AspNetCore.Http;
using LibraryEcom.Domain.Common.Enum;
using System.Security.Cryptography;
using LibraryEcom.Domain.Common.Property;

namespace LibraryEcom.Helper;

public static class ExtensionMethod
{
    public static string SetUniqueFileName(this string fileExtension)
    {
        var dateTimeNow = GetUtcDate();
        
        var renamedFileName = dateTimeNow.Year.ToString() +
                              dateTimeNow.Month.ToString() +
                              dateTimeNow.Day.ToString() +
                              dateTimeNow.Hour.ToString() +
                              dateTimeNow.Minute.ToString() +
                              dateTimeNow.Millisecond.ToString();

        return renamedFileName + fileExtension;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        var random = new Random();

        var n = list.Count;

        while (n > 1) {
            n--;
            var k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static DateTime GetUtcDate()
    {
        return DateTime.UtcNow;
    }
    
    public static string ToFormattedDate(this DateOnly dateOnly)
    {
        return dateOnly.ToString("dd.MM.yyyy");
    }
    
    public static string ToFormattedTime(this TimeSpan timeSpan)
    {
        var dateTime = DateTime.Today.Add(timeSpan);

        return dateTime.ToString("hh:mm tt");
    }
    
    public static string ToFormattedDateTime(this DateTime dateTime, bool isLocalDateFormat = true)
    {
        if (isLocalDateFormat)
        {
            dateTime = dateTime.AddHours(5).AddMinutes(45);
        }

        return dateTime.ToString("dd.MM.yyyy hh:mm tt");
    }
    
    public static string GeneratePassword(int length = 8)
    {
        const string chars = Constants.Authentication.PasswordCharacters;

        var random = new Random();
        var password = new char[length];

        for (var i = 0; i < length; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }

        return new string(password);
    }
    
    public static string HashPassword(this string password)
    {
        byte[] salt;
        
        byte[] buffer2;
        
        ArgumentNullException.ThrowIfNull(password);
        
        using (var bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
        {
            salt = bytes.Salt;
            buffer2 = bytes.GetBytes(0x20);
        }
        
        var dst = new byte[0x31];
        
        Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
        
        Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
        
        return Convert.ToBase64String(dst);
    }
    
    

    public static (string Pronoun, string PossessivePronoun, string ReflexivePronoun) GetPronouns(this GenderType gender)
    {
        return gender switch
        {
            GenderType.Male => ("he", "his", "himself"),
            GenderType.Female => ("she", "her", "herself"),
            _ => ("they", "their", "themselves")
        };
    }
    
    
    public static DateTime GetDateTimeInLocalTimeZone()
    {
        return DateTime.UtcNow.AddHours(5).AddMinutes(45);
    }
}