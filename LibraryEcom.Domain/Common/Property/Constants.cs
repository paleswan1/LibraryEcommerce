namespace LibraryEcom.Domain.Common.Property;

public abstract class Constants
{
    public abstract class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Staff = "Staff";
    }
    
    public abstract class DbProviderKeys
    {
        public const string Npgsql = "postgresql";
    }
    
    public abstract class Admin
    {
        public const string Identifier = "66cd3c59-b2e9-4bd9-8e39-65f550c59c1c";

        public abstract class Development
        {
            public const string Name = "Affinity";
            public const string EmailAddress = "affinity@affinity.io";
            public const string DecryptedPassword = "radi0V!oleta";
        }
    }
    
    public abstract class Staff
    {
        public const string Identifier = "88b6d6e1-3e19-4ae6-a7a1-5cb1584b0123"; 

        public abstract class Development
        {
            public const string Name = "Library Staff";
            public const string EmailAddress = "mochimmy6@gmail.com";
            public const string DecryptedPassword = "St@ff123"; // Use a strong password
        }
    }

    public abstract class Cors
    {
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    }
    
    public abstract class Authentication
    {
        public const string PasswordCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}|;:,.<>?/";
    }
    
    public abstract class Encryption
    {
        public const string Key = "@ff!N1ty";
    }

    public abstract class Provider
    {
        public const string Api = "API";
        public const string Wasm = "WASM";
    }
    
    private abstract class FolderPath
    {
        public const string Images = "images";
        public const string Resources = "resources";
        public const string EmailTemplates = "email-templates";
    }
    
    public abstract class FilePath
    {
        public const string UsersImagesFilePath = $"{FolderPath.Images}/user-images/";
        public const string EmailTemplateFilePath = $"{FolderPath.EmailTemplates}/";

        
    }
    
}