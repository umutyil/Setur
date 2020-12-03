# Setur (.NET) Backend Assessment

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

Bu proje Setur'un hazırlamış olduğu assesmentta belirtilen gereksinimlere istinaden geliştirilmiştir.
Projede kullanılan teknolojiler ve kurulum bilgileri aşağıda ilgili başlılarda bulunabilir. Geliştirme yapılırken yapılan bazı tercihler de sebepleri ile beraber ilgili başlıkta açıklanmıştır.

# Proje Yapısı

Proje, Visual Studio Code kullanılarak geliştirilmiştir. Veritabanı olarak PostgreSQL, message broker olarak da Kafka kullanmaktadır. 
Projede istenen rehber ve rapor işlemlerine ait 2 adet REST tabanlı webapi projesi, Services klasörünün altında bulunmaktadır.
Veritabanı migration scripti Database klasörünün altında bulunmaktadır.

# Kurulum Öncesi

Uygulamanın kullanılacağı bilgisayarda aşağıdaki uygulamaların kurulu olması gerekmektedir.

  - docker
  - docker-compose
  - git

# Kurulum

Kurulum aşağıdaki gibi yapılmaktadır.

```sh
$ git clone https://github.com/umutyil/Setur.git
$ cd Setur
$ docker-compose up -d
```
**Tüm containerlar ayağa kalktıktan sonra**

Kafka içerisinde gerekli olan topiclerin oluşturulması gerekmektedir. Bunun için;
```sh
$ docker ps
```
komutu çalıştırılarak kafka containerının ID değeri alınır. Ardından,
```sh
$ docker exec -it CONTAINERID bash 
```
komutu ile container ın içine girilir. Aşağıdaki komutlar çalıştırılarak gerekli topicler oluşturulur.
```sh
$ root@kafka:/# /usr/bin/kafka-topics --create --if-not-exists --zookeeper zookeeper:2181 --partitions 1 --replication-factor 1 --topic raporrequests
$ root@kafka:/# /usr/bin/kafka-topics --create --if-not-exists --zookeeper zookeeper:2181 --partitions 1 --replication-factor 1 --topic tamamlandi
```
Bu işlem bir defa yapılacaktır.

### Yapılan Tercihler

- **Güvenlik**: Gereksinimler içerisinde herhangi bir güvenlik isteği belirtilmediği için bir güvenlik yapısı sistemde bulunmamaktadır.
- **Loglama**: Gereksinimler içerisinde herhangi bir loglama isteği belirtilmediği için bir loglama yapılmamaktadır. dotnet ile oluşturulan proje içerisinde kendiliğinden gelen ILogger yapısı muhafaza edilmiştir.
- **Unit Test**: Vakit yetersizliği sebebi ile unit test projesi ve mock-up yapısı uygulanamamıştır.

### İyileştirmeler
- Unit Test projesinin oluşturularak solution a eklenmesi
- Controller içerisinde direkt kullanılan DBContext yapısının bir interface e taşınması
- Interface e taşınan DBContext üzerinden Mock-up yapılması ve Unit Testlerin geliştirilmesi

