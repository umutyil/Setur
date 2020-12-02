using System;
using System.Collections.Generic;

#nullable disable

namespace Rapor.Api.Models
{
    public partial class Iletisimbilgileri
    {
        public Guid IletisimbilgiId { get; set; }
        public Guid? KisiId { get; set; }
        public int? BilgiTipi { get; set; }
        public string Icerik { get; set; }

        public virtual Kisiler Kisi { get; set; }
    }
}
