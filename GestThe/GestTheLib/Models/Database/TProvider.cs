using System;
using System.Collections.Generic;

namespace GestTheLib.Models.Database;

public partial class TProvider
{
    public long IdProvider { get; set; }

    public string? ProviderName { get; set; }

    public virtual ICollection<TTea> TTeas { get; set; } = new List<TTea>();
}
