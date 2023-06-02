using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PuzzleMania.Models;

namespace PuzzleMania.Areas.Identity.Data;

public class PuzzleManiaUser : IdentityUser
{
    /* public int? TeamId { get; set; }
     public Team Team { get; set; }*/
    public string? Image { get; set; }

}

