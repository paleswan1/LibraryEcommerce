using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryEcom.Domain.Entities.Identity;

public class Role : IdentityRole<Guid>;