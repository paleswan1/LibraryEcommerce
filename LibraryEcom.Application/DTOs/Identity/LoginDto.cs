﻿namespace LibraryEcom.Application.DTOs.Identity;

public class LoginDto
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string Provider { get; set; }
}
