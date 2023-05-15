using System;
using System.Collections.Generic;

namespace GestThéLib.Models.Database;

public partial class TField
{
    public long IdField { get; set; }

    public string? FieldName { get; set; }

    public virtual ICollection<TList> IdLists { get; set; } = new List<TList>();
}
