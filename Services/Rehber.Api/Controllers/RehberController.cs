using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rehber.Api.Models;

namespace Rehber.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RehberController : ControllerBase
    {
        
        private readonly ILogger<RehberController> _logger;

        public RehberController(ILogger<RehberController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Rehberdeki kisileri listeler
        /// </summary>
        /// <returns>Rehberdeki kisiler listesini doner</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get()
        {
            var res = new List<Kisiler>();
            using(var db  = new setur_databaseContext()){
                //rehberde kayitli kisiler alinir
                res = db.Kisilers.OrderBy(a => a.Soyad).ThenBy(a => a.Ad).ToList();
            }
            res.Add(new Kisiler(){
                Ad="Umut",
                Soyad="Yildirim"
            });
            // eger liste bos ise bulunamadi donulur
            if(res.Count == 0){
                return NotFound();
            }
            return Ok(res);
        }

        /// <summary>
        /// Rehberdeki bir kisinin iletisim bilgileri ile beraber getirir
        /// ID ile eslesen kisi bulunamaz ise 404 hatasi dondurur
        /// </summary>
        /// <param name="id">Kisi ID bilgisi. GUID string</param>
        /// <returns>Iletisim bilgileri olan kisi nesnesi</returns>
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetKisiById(string id){
            using(var db  = new setur_databaseContext()){
                // Kisi bilgisi vt den primary key ile sorgulanir
                var res = db.Kisilers.Find(id);
                // eslesme bulunamadi
                if(res == null){
                    return NotFound();
                } else { // eslesme bulundu
                    //detay bilgileri var mi?
                    if(res.Iletisimbilgileris.Count == 0){
                        // yoksa ekle
                        res.Iletisimbilgileris = db.Iletisimbilgileris.Where(a => a.KisiId == res.KisiId).ToList();
                    }
                }

                return Ok(res);
            }
            
        }


        /// <summary>
        /// Rehbere yeni kisi ekler
        /// Kisi bilgileri hatali ise hatali alanlari doner
        /// Islemde hata olusur ise hata bilgilerini doner
        /// </summary>
        /// <param name="kisi">Rehbere eklenecek kisi bilgileri</param>
        /// <returns>Basarili ise eklenen kisi bilgisini doner</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(Kisiler kisi)
        {
            if(ModelState.IsValid){
                try {
                    using(var db = new setur_databaseContext())
                    {
                        await db.Kisilers.AddAsync(kisi);
                        await db.SaveChangesAsync();
                        return Ok(kisi);
                    }
                }
                catch (Exception ex){
                    return BadRequest(ex);   
                }
            }
            
            return BadRequest(ModelState);            
        }

        /// <summary>
        /// Ilgili kisi bilgisini kaldirir
        /// </summary>
        /// <param name="id">kisi ID bilgisi</param>
        /// <returns>Basarili ise OK doner</returns>
        [HttpDelete]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Delete(string id)
        {
            try {
                    using(var db = new setur_databaseContext())
                    {
                        var kisi = db.Kisilers.Find(id);
                        // eger kisi var ise
                        if(kisi != null){
                            //iletisim bilgileri alinir ve silinir
                            var iletisimBilgileri = db.Iletisimbilgileris.Where(a => a.KisiId == kisi.KisiId).ToList();
                            db.Iletisimbilgileris.RemoveRange(iletisimBilgileri);
                            db.Kisilers.Remove(kisi);
                            db.SaveChanges();
                        }
                        
                        return Ok();
                    }
                }
                catch (Exception ex){
                    return BadRequest(ex);   
                }     
        }

        
        /// <summary>
        /// Varolan kisiye iletisim bilgisi ekler.
        /// Bilgi Tipi olarak 
        /// 1 - Telefon Numarasi
        /// 2 - E-mail Adresi
        /// 3 - Konum
        /// seklinde kullanilir. Bunlarin disindaki degerlerde hata doner
        /// </summary>
        /// <param name="bilgi">Eklenecek Iletisim Bilgisi</param>
        /// <returns>Islem sonucunu Ok ya da Hata olarak doner</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateIletisimBilgisi(Iletisimbilgileri bilgi)
        {
            if(ModelState.IsValid){
                try {
                    if(bilgi.BilgiTipi <1 || bilgi.BilgiTipi > 3){
                        throw new ArgumentOutOfRangeException("Bilgi Tipi 1 ile 3 arasinda olmalidir");
                    }
                    using(var db = new setur_databaseContext())
                    {
                        await db.Iletisimbilgileris.AddAsync(bilgi);
                        await db.SaveChangesAsync();
                        return Ok(bilgi);
                    }
                }
                catch (Exception ex){
                    return BadRequest(ex);   
                }
            }
            
            return BadRequest(ModelState);            
        }


        /// <summary>
        /// Iletisim bilgisini siler
        /// </summary>
        /// <param name="id">Iletisim bilgisi ID</param>
        /// <returns>Islem sonucunu Ok ya da Hata olarak doner</returns>
        [HttpDelete]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteIletisimBilgisi(string id)
        {
            try {
                    using(var db = new setur_databaseContext())
                    {
                        var iletisimBilgisi = db.Iletisimbilgileris.Find(id);
                        // eger iletisim bilgisi var ise
                        if(iletisimBilgisi != null){
                            //iletisim bilgileri alinir ve silinir                            
                            db.Iletisimbilgileris.Remove(iletisimBilgisi);                            
                            db.SaveChanges();
                        }
                        
                        return Ok();
                    }
                }
                catch (Exception ex){
                    return BadRequest(ex);   
                }     
        }
    }
}
