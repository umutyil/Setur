CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE TABLE IF NOT EXISTS kisiler (
  kisi_id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  ad VARCHAR NOT NULL,
  soyad VARCHAR NOT NULL,
  firma VARCHAR NOT NULL
);
CREATE TABLE IF NOT EXISTS iletisimbilgileri (
  iletisimbilgi_id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  kisi_id uuid references kisiler(kisi_id),
  bilgi_tipi int,
  icerik VARCHAR NOT NULL
);
CREATE TABLE IF NOT EXISTS raporlar (
  rapor_id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  durum int,
  tarih timestamp DEFAULT now(), 
  icerik VARCHAR NOT NULL
);
