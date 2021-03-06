using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rapor.Api.Kafka;
using Rapor.Api.Models;

namespace Rapor.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RaporController : ControllerBase
    {
        
        private readonly ILogger<RaporController> _logger;

        private readonly ProducerConfig config;

        public RaporController(ILogger<RaporController> logger, ProducerConfig config)
        {
            _logger = logger;
            this.config = config;
        }

        /// <summary>
        /// Tanimli raporlari listeler kisileri listeler
        /// </summary>
        /// <returns>Tanimli Rapor listesini doner</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get()
        {
            var res = new List<Raporlar>();
            using(var db  = new setur_databaseContext()){
                //rehberde kayitli kisiler alinir
                res = db.Raporlars.OrderByDescending(a => a.Tarih).ToList();
            }
            res.Add(new Raporlar(){
                Tarih=DateTime.Now,
                Durum=1,
                Icerik="Ankara"
            });
            // eger liste bos ise bulunamadi donulur
            if(res.Count == 0){
                return NotFound();
            }
            return Ok(res);
        }

        /// <summary>
        /// Tanimli bir raporun icerigini doner
        /// ID ile eslesen rapor bulunamaz ise 404 hatasi dondurur
        /// Rapor durumu hazirlandi degil ise 404 hatasi dondurur
        /// </summary>
        /// <param name="id">Rapor ID bilgisi. GUID string</param>
        /// <returns>Rapor icerigi</returns>
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRaporById(string id){
            using(var db  = new setur_databaseContext()){
                // Rapor bilgisi vt den primary key ile sorgulanir
                var res = db.Raporlars.Find(id);
                var sonuc= new RaporIcerik();
                // eslesme bulunamadi
                if(res == null){
                    return NotFound();
                } else { // eslesme bulundu
                    //Rapor durumu Tamamlandi mi?
                    if(res.Durum == 1){
                        // yoksa ekle
                        return BadRequest("Rapor su an hazirlanmaktadir");
                    }

                    // Oncelikle konum bilgisi girilen deger olan tum iletisim bilgileri alinir
                    var iletisimler = db.Iletisimbilgileris.Where(a => a.BilgiTipi == 3 && a.Icerik == res.Icerik).ToList();
                    sonuc.Konum = res.Icerik;
                    // Tekil kisi sayisi hesaplanir
                    var kisilerTekil =iletisimler.Select(a => a.KisiId).Distinct().ToList();
                    var kisiSayisi = kisilerTekil.Count();
                    sonuc.KisiSayisi = kisiSayisi;
                    // Bu konumdaki kisilere ait telefon numaralari sayilir
                    var telefonSayisi = iletisimler.Where(a => kisilerTekil.Contains(a.KisiId) && a.BilgiTipi == 1).Count();
                    sonuc.TelefonNumarasiSayisi = telefonSayisi;
                }

                return Ok(sonuc);
            }
            
        }


        /// <summary>
        /// Yeni Rapor talebi olusturur
        /// Rapor bilgileri hatali ise hatali alanlari doner
        /// Islemde hata olusur ise hata bilgilerini doner
        /// </summary>
        /// <param name="rapor">Hazirlanacak rapor bilgileri</param>
        /// <returns>Basarili ise eklenen rapor bilgisini doner</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(Raporlar rapor)
        {
            if(ModelState.IsValid){
                try {
                    using(var db = new setur_databaseContext())
                    {
                        await db.Raporlars.AddAsync(rapor);
                        await db.SaveChangesAsync();

                        // Kafkaya rapor hazirlama islemini bildir
                        string serializedOrder = JsonConvert.SerializeObject(rapor);           

                        var producer = new ProducerWrapper(this.config,"raporrequests");
                        await producer.writeMessage(serializedOrder);
                        return Ok(rapor);
                    }
                }
                catch (Exception ex){
                    return BadRequest(ex);   
                }
            }
            
            return BadRequest(ModelState);            
        }

        
    }
}
