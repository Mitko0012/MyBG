using Microsoft.EntityFrameworkCore;
using MyBG.Models;
using NuGet.Protocol.Plugins;

namespace MyBG.Models;

public class AdminRequest
{ 
    public int Id {get; set;}
    public string Message {get; set;}
    public PFPModel UserCreated {get; set;}
    public bool Processed {get; set;}
}