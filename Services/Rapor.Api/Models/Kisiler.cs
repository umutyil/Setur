using System;
using System.Collections.Generic;

#nullable disable

namespace Rapor.Api.Models
{
    public partial class Kisiler
    {
        public Kisiler()
        {
            Iletisimbilgileris = new HashSet<Iletisimbilgileri>();
        }

        public Guid KisiId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Firma { get; set; }

        public virtual ICollection<Iletisimbilgileri> Iletisimbilgileris { get; set; }
    }
}
