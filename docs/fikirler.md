# Fikirler (kapsam dışı)

Aklına gelen ama bu projeye girmeyecek her şey buraya. Kapsam genişletme yok.

## Opsiyonel: canlıya alma (çekirdek bittikten sonra karar)

Faz değil, fikir. Karar kriteri: fiyat araştırması (Huseyin'de) + çekirdeğin bitmiş olması.
Kod bugünden hazır tutuluyor, bu yüzden kararı ertelemenin bedeli sıfır — bkz. `decisions.md`.

**A — Tek sanal makine** (Azure VM / Hetzner / DigitalOcean), aynı `docker-compose up`.
Öğretir: Linux, reverse proxy (nginx/Caddy), TLS sertifikası, DNS, sunucu güvenliği.
En ucuz, en az sürpriz, bir akşamlık iş. "Cloud-native" değil.

**B — Azure Container Apps + yönetilen servisler** (Azure SQL, Azure Cache for Redis,
Service Bus, Blob Storage). Öğretir: managed identity, secret yönetimi, scale-to-zero,
log analytics. İlanlarda karşılığı olan bilgi. Belirgin şekilde pahalı, mini bir faz kadar iş.

"Sadece çalıştığı sürece öde, sonra kapat" tercihine teknik olarak B uyuyor
(Container Apps sıfıra iner, Azure SQL serverless duraklar). Yeni hesaplara verilen
deneme kredisi "kur → demo videosunu çek → hepsini sil" senaryosuna yeter.
Kırılma noktaları: RabbitMQ → Service Bus (MassTransit soğuruyor),
MinIO → Blob Storage (`IFileStorage` soğuruyor). İkisi de zaten kapatılmış durumda.

## Kapsam dışı: abonelik / faturalama katmanı

Ürünün gerçek dünyadaki karşılığı SaaS (Sentos, Entegra aylık abonelikle satar) ve
**single-tenant SaaS** — müşteri başına ayrı kopya — geçerli bir modeldir: kod basit kalır
(tenant filtresi yok, veri sızıntısı riski yok), müşteriye özel ayar kolaydır; bedeli 50
müşteri = 50 kurulum, 50 ayrı sürüm çıkışıdır. Abonelik böyle bir modelde uygulamanın
**dışında** yaşar (ayrı faturalama servisi ya da elle), uygulama sadece lisansa bakar.

Yine de yazılmıyor — kural 3 (aynı işi iki araçla yapma): ödeme entegrasyonunun öğrettiği
şey webhook alıp idempotent işlemek, o zaten task 21'de pazaryeri entegrasyonundan geliyor.
Multi-tenancy ayrıca `CLAUDE.md`'de "kesin dışarıda" listesinde.

## Kapsam dışı: kalite durumuna göre bölünmüş stok (karantina / ıskarta)

Tam sürümde eldeki tek sayı değil, kalite kovalarına bölünürdü: *satılabilir*, *karantina*
(iade gelip muayene bekleyen), *ıskarta*, *tamirde*. İade karantinaya girer, muayene kararı
(disposition) onu satılabilire ya da ıskartaya taşır — her geçiş deftere satır.

Yapılmıyor: kalite yönetimi bu ürünün işi değil ve "bitti tanımı"na katkısı yok.
Bu projedeki karşılığı `01-domain.md` kural 13 — iade otomatik stoğa girmez, muayeneden
sonra elle stok girişi yapılır (sebep: iade girişi). Var olan stok düzeltme yolu kullanılır,
yeni kod yok.

## Kapsam dışı: paket / set ilanı

Kanalda tek ilan ("3'lü çorap paketi"), bizde 3 ayrı varyant. Satılabilir artık saklanan bir
sayı değil, bileşenlerin en darının belirlediği **türev bir hesap** olurdu; rezervasyon tek
satır yerine birden çok satırı aynı işlemde kilitlemek zorunda kalır ve kilit sırası sabitlenmezse
iki eşzamanlı sipariş birbirini kilitler (deadlock).

Yapılmıyor: bitiş tanımına katkısı yok, ama task 09 ve 10'u belirgin şekilde zorlaştırıyor.
Gerçek hayatta yaygın olduğu için README'de bedeliyle birlikte yazılır — tek depo varsayımında
olduğu gibi.

## Kapsam dışı: kanalda ilan açma

"Yeni ürün ekledim, Trendyol'da ilanını da API'den aç" yok. Her pazaryerinin ilan açma
sözleşmesi birbirinden tamamen farklıdır (zorunlu kategori öznitelikleri, görsel kuralları,
onay süreci) ve bunu simüle etmek simülatörü (task 19) öğretici olmayan bir yöne şişirir.
Push ettiğimiz şey stok ve sipariş durumudur, ilan içeriği değil. İlan kanalda elle açılır,
bize yalnızca eşleştirme girilir.

## Kapsam dışı: kısmi sevkiyat

3 satırlık siparişin 2 satırı bugün, kalan satır stok gelince sevk edilir. Gerçek hayatta
pazaryerleri buna izin verir. Bedeli tamamen muhasebe: sevkiyat artık sipariş seviyesinde tek
bir olay değil, satır grubu başına ayrı bir kayıt olur — grup başına kargo ve takip numarası,
kanala "şu satırlar gitti" bildirimi, siparişin özet durumunda kısmi hâller.

Yapılmıyor: eşzamanlılık, idempotency ve mutabakat kaslarının hiçbirine dokunmuyor.
Asıl ders kaybolmuyor — "durum makinesi satırda yaşar, sipariş durumu türetilir" kuralı
kısmi sevkiyat yüzünden değil, eşleşmemiş satır (kural 25) yüzünden zaten zorunlu.

## Kapsam dışı: push önceliklendirme

Kuyrukta 200 push beklerken hepsi eşit aceleci değil: 0'a düşen bir ilan, 50'den 49'a inenden
çok daha önemlidir — biri oversell penceresini kapatıyor, diğeri kozmetik. Öncelik kuyruğu
(kritik seviye altındaki ilanlar önce) push gecikmesinin *ortalamasını* değil, *zararlı olan
kısmını* kısaltırdı.

Yapılmıyor: kural 37'deki kanal başına kuyruk + birleştirme zaten kuyruğu ilan sayısıyla
sınırlıyor, yani beklemeler kısa. Öncelik mantığı kuyruğu karmaşıklaştırır ve öğrettiği yeni
bir şey yok.
