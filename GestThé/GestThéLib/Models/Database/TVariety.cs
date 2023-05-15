using System;
using System.Collections.Generic;

namespace GestThéLib.Models.Database;

public partial class TVariety
{
    public long IdVariety { get; set; }

    public string? VarietyName { get; set; }

    public virtual ICollection<TTea> TTeas { get; set; } = new List<TTea>();
}
