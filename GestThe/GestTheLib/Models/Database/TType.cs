using System;
using System.Collections.Generic;

namespace GestTheLib.Models.Database;

public partial class TType
{
    public long IdType { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<TTea> TTeas { get; set; } = new List<TTea>();
}
