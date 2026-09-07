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

**Soru: İki farklı modülün aynı transaction içinde veritabanına yazması nasıl sağlanır?**
Sipariş oluşturmada `Ordering` siparişi, `Inventory` rezervasyonu yazar. İkisi ayrı modül, ayrı
`DbContext`. Yarısının yazılıp yarısının yazılmaması kabul edilemez.

Cevap: transaction bağlantıya aittir, o yüzden **request başına tek bir bağlantı** açılır
(`DbConnectionAccessor`, scoped) ve o request'e hizmet eden bütün `DbContext`'ler bu tek
bağlantının üstüne kurulur. Transaction bu bağlantıda bir kez açılır, her context
`Database.UseTransaction` ile ona bağlanır, sonunda hepsi tek `Commit` ile yazılır
(`UnitOfWork`). Tek bağlantı = tek transaction; dağıtık transaction (MSDTC) gerekmez.

Bedeli: tek hat üzerinde aynı anda iki sorgu çalışamaz — bir handler'da iki veritabanı
sorgusunu `Task.WhenAll` ile birlikte başlatamazsın.

**Soru: Bir modülün migration'ları diğerininkine neden karışmıyor?**
İki ayrı mekanizma: `MigrationsAssembly` migration dosyalarının hangi projeye yazılacağını,
`MigrationsHistoryTable` de "hangileri uygulandı" kaydının hangi şemada tutulacağını belirler.
Her modül kendi şemasında kendi `__EFMigrationsHistory` tablosunu tutar; böylece bir modülü
tek başına geri almak (`database update <önceki>`) mümkün olur.

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

## EF Core ve migration

Her modülün kendi `DbContext`'i, kendi **şeması** ve kendi migration geçmişi var:

| Modül | Şema | Context |
|---|---|---|
| Inventory | `inventory` | `InventoryDbContext` |
| Ordering | `ordering` | `OrderingDbContext` |
| Integration | `integration` | `IntegrationDbContext` |

Identity'nin context'i henüz yok; task 14'te ASP.NET Identity tablolarıyla birlikte gelecek.

Bağlantı bilgisi tek yerde: `src/OrderSync.Api/appsettings.Development.json` →
`ConnectionStrings:OrderSyncDb`. Kodda hiçbir yerde connection string yazmıyor. Başka bir
makinede/ortamda `ConnectionStrings__OrderSyncDb` environment değişkeni bu satırı ezer.
Kendine özel bir ayar denemek istersen `appsettings.Development.local.json` aç — o gitignore'da.

**Development'ta migration'lar uygulamanın kendisi tarafından uygulanır.** API ayağa kalkarken
veritabanı yoksa oluşturur, sonra üç context'in bekleyen migration'larını çalıştırır ve
`InventoryDbContext is up to date.` satırlarını loglar. Yani `docker compose up -d --wait` +
Rider'da Run = çalışan sistem. Bu davranış sadece Development'ta açık.

### Yeni migration eklemek

Modül başına ayrı komut — `--project` migration'ın yazılacağı modül, `--startup-project` her
zaman API (konfigürasyonu ve DI'ı o kuruyor):

```
dotnet ef migrations add <Ad> --project src/Modules/Inventory/Inventory --startup-project src/OrderSync.Api --context InventoryDbContext --output-dir Data/Migrations
```

| İş | Komut |
|---|---|
| Son migration'ı geri al (henüz uygulanmadıysa) | `dotnet ef migrations remove --project ... --startup-project src/OrderSync.Api --context ...` |
| Elle uygula | `dotnet ef database update --project ... --startup-project src/OrderSync.Api --context ...` |
| Uygulanmışları listele | `dotnet ef migrations list --project ... --startup-project src/OrderSync.Api --context ...` |

Migration adı İngilizce ve ne yaptığını söyler: `AddStockItem`, `AddOrderLineStatus`.

### Sıfırdan başlamak

Veritabanını tamamen atmak için (`docker compose down -v` gerekmez):

```
docker exec ordersync-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'Local_Dev_P4ssw0rd!' -C -Q "ALTER DATABASE OrderSync SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE OrderSync;"
```

Sonraki çalıştırmada uygulama veritabanını ve şemaları yeniden kurar.

### Takıldığında

**`Globalization Invariant Mode is not supported`** — `Directory.Build.props` içindeki
`InvariantGlobalization` açılmış demektir. `Microsoft.Data.SqlClient` o modda hiçbir bağlantı
açamaz; `false` kalmalı.

**`No instantiatable types implementing IEntityTypeConfiguration were found`** — uyarı, hata
değil. Modülde henüz tablo yok. İlk entity configuration yazıldığında (task 06) kendiliğinden
susar.

**`Login failed for user 'sa'`** — parola yanlışsa gelir, ama veritabanı yokken de gelebilir:
uygulama bağlantı nesnesini `Database=OrderSync` ile kurar ve başarısız açılış o nesnenin
parolasını düşürür. Uygulamayı başlatmak sorunu çözer — veritabanını o oluşturur.

## Loglama ve hata yönetimi

Serilog `Program.cs`'te koda değil **konfigürasyona** bağlandı: seviyeler ve sink'ler
`appsettings.json` içindeki `Serilog` bölümünde. Yeni bir sink eklemek ya da bir namespace'i
susturmak için kod değişmez.

| Nereye | Ne için |
|---|---|
| Console | çalışırken göz ucuyla bakmak |
| Seq — http://localhost:8081 | aramak, filtrelemek |

**Structured logging.** Log satırı düz metin değil, alanlara ayrılmış bir kayıt. Şablonu
`"Unhandled exception on {Method} {Path}"` diye yazınca Seq'te `Path` ayrı bir alan olur ve
`RequestPath like '/api/inventory%'` diye filtreleyebilirsin. String interpolation
(`$"... {path}"`) kullanırsan bu alanlar kaybolur — şablon her zaman sabit kalmalı.

**Seq'te işe yarayan filtreler**

```
@Level = 'Error'
@TraceId = '9a57a47e41552dde09b1780dd30c3745'
RequestPath like '/api/inventory%' and StatusCode >= 400
Elapsed > 500
```

### Beklenmeyen hata yakalandığında ne oluyor

`GlobalExceptionHandler` (`src/OrderSync.Api/`) `IExceptionHandler`'ı uygular; .NET 8'den beri
kendi middleware'ini yazmaya gerek yok, `UseExceptionHandler()` yakaladığı exception'ı ona verir.
İki iş yapar: exception'ı stack trace'iyle **loglar**, istemciye **`ProblemDetails`** döner
(RFC 7807 — `type`/`title`/`status`, `application/problem+json`).

Gövdeye `Detail` **bilerek konmuyor**: exception mesajı connection string, SQL ya da müşteri
verisi taşıyabilir. İstemcinin aldığı tek ipucu `traceId`:

```json
{ "title": "An unexpected error occurred.", "status": 500,
  "traceId": "00-9a57a47e41552dde09b1780dd30c3745-26a5299d4011e839-00" }
```

Ortadaki uzun parça trace id'dir; Seq'te `@TraceId` alanına birebir eşittir. Yani elinde sadece
bu gövde varken hatanın tam hikâyesine tek aramayla ulaşırsın. Bir destek kaydında istenecek şey
budur.


