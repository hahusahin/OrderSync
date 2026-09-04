# Geliştirme Notları

## Kısayollar — Visual Studio keymap

| İşlem | Kısayol |
|---|---|
| Her şeyi ara | `Shift Shift` · `Ctrl+T` |
| Dosyaya git | `Ctrl+Shift+T` |
| Komut / ayar ara | `Ctrl+Q` |
| Hızlı düzeltme, bağlam eylemleri | `Alt+Enter` |
| Tanıma git | `F12` |
| Yeniden adlandır | `Ctrl+R, R` |
| Build | `Ctrl+Shift+B` |
| Debug | `Alt+F5` |

Bilmediğin her şey için: `Shift Shift` → adını yaz.

## Ekran görüntüsü

`Ctrl+V` ile yapıştırma Windows'ta güvenilir değil. Görüntüyü `docs/img/` içine kaydet,
mesajda yolunu ver.

## Öğrendiklerim

**`.csproj` açmak.** Solution görünümünde ayrı dosya olarak listelenmez; proje düğümünün kendisi
o dosyadır. Sağ tık → Edit → `Edit 'X.csproj'`, ya da `Ctrl+Shift+T` ile adını yaz.
`Directory.Build.props` gibi projeye ait olmayan dosyalar için de `Ctrl+Shift+T`.

## Docker — lokal altyapı

Beş servis `docker-compose.yml` ile geliyor: SQL Server, Redis, RabbitMQ, MinIO, Seq.
API şimdilik container'da değil; Rider'dan çalışıp bu portlara bağlanıyor.

**Docker Desktop açık olmalı.** Kapalıyken her `docker` komutu
`failed to connect to the docker API` der. Başlat menüsünden aç, balina simgesi
sabitlenene kadar bekle.

### Günlük komutlar

| İş | Komut |
|---|---|
| Hepsini başlat | `docker compose up -d` |
| Başlat **ve hepsi healthy olana kadar bekle** | `docker compose up -d --wait` |
| Durum + sağlık | `docker compose ps` |
| Durdur (veri kalır) | `docker compose stop` |
| Durdur + container'ları sil (veri kalır) | `docker compose down` |
| **Veriyi de sil** (sıfırdan başla) | `docker compose down -v` |
| Tek servisin logu, canlı | `docker compose logs -f sqlserver` |
| Tek servisi yeniden başlat | `docker compose restart rabbitmq` |

`down` ile `down -v` arasındaki fark tek harf ama geri dönüşü yok: `-v` named volume'ları
siler, yani veritabanı da gider. Migration'ları tekrar çalıştırman gerekir.

### Arayüzler ve bağlantı bilgileri

| Servis | Adres | Kullanıcı / parola |
|---|---|---|
| SQL Server | `localhost,1433` | `sa` / `Local_Dev_P4ssw0rd!` |
| Redis | `localhost:6379` | yok |
| RabbitMQ yönetim UI | http://localhost:15672 | `ordersync` / `Local_Dev_P4ssw0rd!` |
| MinIO konsol | http://localhost:9001 | `ordersync` / `Local_Dev_P4ssw0rd!` |
| Seq UI | http://localhost:8081 | yok |

Parolalar `docker-compose.yml` içinde default olarak gömülü; değiştirmek istersen kökte
`.env` aç (`.env.example`'ı kopyala), `docker compose up -d` ile tekrar kaldır.

### Sağlık durumu — `healthy` ne demek

`docker compose ps` çıktısındaki `(healthy)`, container'ın **çalıştığını** değil, içine yazdığımız
kontrol komutunun geçtiğini söyler — SQL Server için gerçekten `SELECT 1` çalıştırılır.
Fark önemli: SQL Server ilk açılışta ~30 saniye bağlantı kabul etmez, ama container o an
"running" görünür. Migration çalıştırmadan önce `healthy` bekle — `docker compose up -d --wait`
bunu senin yerine yapar: beşi de healthy olana kadar geri dönmez, olmazsa hata koduyla çıkar.

Servisler arasında başlama sırası **yok**, çünkü birbirlerine bağlı değiller. Sıra, API compose'a
girdiğinde (task 41) `depends_on: condition: service_healthy` ile gelecek.

### Rider'dan bağlanmak

Database penceresi (`Alt+3` ya da View → Tool Windows → Database) → `+` → Data Source →
Microsoft SQL Server. Host `localhost`, Port `1433`, User `sa`, parola yukarıdaki.
İlk açılışta driver'ı indirmek isteyecek, indir. Bağlantı hatası verirse
`Advanced` sekmesinde `trustServerCertificate` değerini `true` yap — container self-signed
sertifika kullanıyor.

### Takıldığında

**"port is already allocated"** — o portu başka bir şey tutuyor. Genelde makinede kurulu
gerçek SQL Server servisi (1433) olur. `netstat -ano | findstr :1433` ile PID'i bul,
Görev Yöneticisi → Ayrıntılar'da kimliğini gör.

**SQL Server container'ı sürekli yeniden başlıyor** — `docker compose logs sqlserver`.
En sık sebebi parolanın SQL Server'ın karmaşıklık kuralına uymaması (8+ karakter, büyük/küçük
harf, rakam, sembol). Konteyner sessizce ölür, log söyler.
