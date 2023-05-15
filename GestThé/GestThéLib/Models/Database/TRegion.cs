using System;
using System.Collections.Generic;

namespace GestThéLib.Models.Database;

public partial class TRegion
{
    public long IdRegion { get; set; }

    public string? RegionName { get; set; }

    public long IdCountry { get; set; }

    public virtual TCountry IdCountryNavigation { get; set; } = null!;

    public virtual ICollection<TTea> TTeas { get; set; } = new List<TTea>();
}
