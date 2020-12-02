using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using Rapor.Api.Models;
using Newtonsoft.Json;
using Confluent.Kafka;

namespace Rapor.Api.Kafka{


    public class ProcessRaporService: BackgroundService
    {
        private readonly ConsumerConfig consumerConfig;
        private readonly ProducerConfig producerConfig;
        public ProcessRaporService(ConsumerConfig consumerConfig, ProducerConfig producerConfig)
        {
            this.producerConfig = producerConfig;
            this.consumerConfig = consumerConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumerHelper = new ConsumerWrapper(consumerConfig, "raporrequests");
                string raporRequest = consumerHelper.readMessage();

                //Rapor objesi mesajdan alinir 
                Raporlar rapor = JsonConvert.DeserializeObject<Raporlar>(raporRequest);

                
                Console.WriteLine($"Info: RaporHandler => Rapor isleniyor {rapor.RaporId}");
                try{

                
                using(var db  = new setur_databaseContext()){
                    //raporun dsorgulanir
                    var raporObj = db.Raporlars.Find(rapor.RaporId);
                    //rapor bulundu ise
                    if(raporObj != null){
                        raporObj.Durum = 2;
                        await db.SaveChangesAsync();
                    }
                    rapor.Durum = 2;
                }     
                }
                catch (Exception ex){
                    Console.WriteLine($"Hata: RaporHandler => Raporda hata olustu {ex.Message}");
                }               

                var producerWrapper = new ProducerWrapper(producerConfig,"tamamlandi");
                await producerWrapper.writeMessage(JsonConvert.SerializeObject(rapor));
            }
        }
    }
}