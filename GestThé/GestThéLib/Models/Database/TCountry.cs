using System;
using System.Collections.Generic;

namespace GestThéLib.Models.Database;

public partial class TCountry
{
    public long IdCountry { get; set; }

    public string? CountryName { get; set; }

    public virtual ICollection<TRegion> TRegions { get; set; } = new List<TRegion>();
}
