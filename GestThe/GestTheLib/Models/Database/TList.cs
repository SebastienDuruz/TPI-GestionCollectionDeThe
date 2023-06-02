using System;
using System.Collections.Generic;

namespace GestTheLib.Models.Database;

public partial class TList
{
    public long IdList { get; set; }

    public string? ListName { get; set; }

    public string? ListDescription { get; set; }

    public DateTime? ListAddDate { get; set; }

    public DateTime? ListModificationDate { get; set; }

    public virtual ICollection<TField> IdFields { get; set; } = new List<TField>();

    public virtual ICollection<TTea> IdTeas { get; set; } = new List<TTea>();
}
