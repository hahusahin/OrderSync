# Domain Notları

> Koda dönüşecek kurallar. Anlatı ve gerekçe burada değil — README'ye (task 40) gider.

## Ortak dil (Ubiquitous Language)

**Stok**

| Terim | Karşılığı |
|---|---|
| **Eldeki** (on-hand) | Fiziksel olarak var kabul edilen miktar. Rafın aynası. |
| **Rezerve** (allocated) | Siparişe ayrılmış, henüz sevk edilmemiş miktar |
| **Satılabilir** (ATP) | Eldeki − Rezerve − Tampon. **Kanallara bildirilen sayı budur.** |
| **Tampon** (buffer) | Bilerek bildirmediğimiz pay. Ürün bazında ayarlanır. |
| **Stok devir hızı** | Ürünün satılıp yenilenme sıklığı. Tampon kararının girdisi. |
| **Sayaç** | Tek bir sayı tutan sütun (`OnHand = 12`). Üstüne yazılır, eski değer kaybolur. |
| **Defter** (ledger) | Her değişimin ayrı satır olarak yazıldığı tablo. Silinmez, yalnız eklenir. |

**Katalog ve kanal**

| Terim | Karşılığı |
|---|---|
| **Kanal** | Satış yeri — pazaryeri, kendi site, mağaza |
| **Ürün** (product) | Varyantları gruplar, **stoğu yoktur** |
| **Varyant** (variant) | Sipariş edilebilen, rafta ayrı yeri olan şey. **Stok buradadır.** |
| **SKU** | Bir varyantın bizdeki kodu. İnsan yazar, insan okur. |
| **Barkod** (GTIN/EAN) | Üreticinin küresel kodu. Tüm satıcılarda aynıdır, bizim değildir. |
| **Kanal kodu** | Kanalın o ilana verdiği kimlik (ASIN, stockCode). Kanala aittir. |
| **İlan** (listing) | Bir varyantın bir kanaldaki tek bir satış kaydı. Kanal kodunu varyantımıza bağlayan eşleştirme satırı budur; aynı varyantın aynı kanalda birden çok ilanı olabilir (kural 24). |
| **System of record** | Anlaşmazlıkta sayısı doğru kabul edilen sistem |

**Sipariş**

| Terim | Karşılığı |
|---|---|
| **Sipariş satırı** | Siparişteki tek bir ürün satırı |
| **Karşılama** (fulfillment) | Siparişin toplanıp paketlenip müşteriye ulaştırılması |
| **Alan** (field) | Sipariş kaydındaki tek bir bilgi. Otorite alan bazında belirlenir. |
| **Durum makinesi** | Sonlu durum listesi **ve** izinli geçişlerin listesi. İkisi birlikte; yalnız liste tutmak etikettir. |
| **Terminal durum** | Kendisinden çıkış geçişi olmayan durum (`Cancelled`) |

**Eşzamanlılık**

| Terim | Karşılığı |
|---|---|
| **Oversell** | Elde olmayan adedin satılması |
| **Satır kilidi** | Veritabanının bir satırı güncelleme boyunca kapatması. İkinci işlem bekler. |
| **Tekil indeks** | Veritabanının bir sütun birleşiminde ikinci kaydı reddetmesi. Var olmayan satır kilitlenemediği için oluşturma yarışının tek çaresi. |
| **Deadlock** | İki işlemin karşılıklı olarak diğerinin tuttuğu kilidi beklemesi |
| **Eşzamanlılık** (concurrency) | Aynı anda kaç mesajın işlendiği. Kuyruk sırayı korur, eşzamanlılık bozar. |
| **Idempotency** | Aynı mesajın tekrarının sonucu değiştirmemesi |

**Entegrasyon**

| Terim | Karşılığı |
|---|---|
| **Polling** | Kanala aralıklarla "yeni bir şey var mı" diye sormak. İstek bizden çıkar. |
| **Webhook** | Kanalın bizim endpoint'imizi çağırıp "şu değişti" demesi. İstek kanaldan çıkar. |
| **Rate limit** | Birim zamanda kabul edilen istek sayısı. Aşılınca `429`; `Retry-After` ne zaman devam edileceğini söyler, ne hızla gidileceğini değil. |
| **At-least-once** | Mesaj kaybolmaz ama birden fazla gelebilir. Gönderen "gitmedi" ile "onayı dönmedi"yi ayırt edemez. |
| **Birleştirme** (coalescing) | Aynı hedef için bekleyen birden çok işi tek işe indirmek; son değer kazanır. |
| **Backoff + jitter** | Bekleme süresini her denemede büyütmek + üstüne rastgele sapma. Jitter olmazsa bekleyenler duvara topluca çarpar (thundering herd). |
| **Devre kesici** (circuit breaker) | Art arda hata veren kanala denemeyi bir süre durdurmak. Ölü kanala atılan istek bağlantı tutar, birikince diğerlerini de bloke eder. |
| **Consumer** | Kuyruktan mesajı alıp işi yapan kod. Ayrı program değil, uygulamanın arka plan parçası. |
| **Bayat** (stale) | Eski bir anın verisi; artık doğru değil |

**Tasarım**

| Terim | Karşılığı |
|---|---|
| **Invariant** | Her yazma işleminin sonunda sağlanması gereken kural. Bu projede tek tane: rezerve ≤ eldeki. |
| **Aggregate** | Tek işlemde birlikte yüklenip birlikte yazılan nesne kümesi. Sınırını ilişkiler değil, korunacak kural belirler. |
| **Aggregate kökü** (root) | Aggregate'e dışarıdan erişilen tek nesne. Dışarısı içerideki çocuğu değil, kökün **id**'sini tutar. |
| **Bounded context** | Bir terimin tek bir anlamı olduğu sınır. Aynı kelime iki sınırda iki farklı şeydir; sınırda çevrilir. |
| **Anti-corruption layer** (ACL) | Sınırdaki çeviri katmanı. Şekil çevirmekten fazlasıdır, karar içerir. |
| **Domain event** | Bir modülün "şu oldu" diye yayınladığı, dinleyenini tanımadığı olay |
| **Stratejik / taktik DDD** | Stratejik: sınır çizmek — her yerde. Taktik: aggregate, value object gibi desenler — yalnız invariant olan yerde. |

**Mutabakat**

| Terim | Karşılığı |
|---|---|
| **Mutabakat** | Bizdeki ve kanaldaki sayıyı karşılaştırıp sapmayı düzeltme |
| **Sapma** (drift) | Bizdeki ve kanaldaki sayının ayrışması. İki ekseni var: iletişim ve fiziksel. |
| **Uçuştaki sipariş** | Kanalın sattığı ama bize henüz ulaşmamış sipariş. Uyuşmazlık üretir, arıza değildir. |
| **Sessiz pencere** | Uyuşmazlığı hemen düzeltmeyip N dakika sonra tekrar bakmak |

## Vaat

> **Stok ve sipariş karşılaması için** tek panel, tek stok gerçeği, otomatik senkron —
> ve oversell'i sıfırlamak yerine **ölçülebilir şekilde küçültmek.**

Manuel senkron ve dağınık sipariş tamamen çözülür; oversell azaltılır, bitirilmez.
Kanalın kendi paneli ortadan kalkmaz: replika olduğumuz her alanda (müşteri mesajı,
iade onayı, hakediş/fatura, fiyat-kampanya, satıcı puanı) panel hâlâ gereklidir.

## Aktörler

**Sistemi kullananlar — hepsi satıcı şirketin içinde.** Müşteriye bakan yüz yoktur
(task 16'daki rol modeli budur):

| Rol | Sistemde yaptığı |
|---|---|
| Operasyon sorumlusu | Siparişleri izler, sorunluyu çözer. Panelin asıl kullanıcısı. |
| Depo personeli | Toplama/paketleme, sevk işaretleme |
| Stok sorumlusu | Stok girişi ve düzeltmesi, tampon ayarı |
| Yönetici | Yalnız okur (rapor) |

**Kullanmayan ama akışı belirleyenler:** müşteri (bizde hesabı yok, tüm eylemleri bize
kanal üzerinden **gecikmeli ve dolaylı** ulaşır), kanal (hem sipariş kaynağı hem stok
tüketicisi — iki ayrı rol), kargo firması (kapsam dışı; yalnız takip no tutulur),
tedarikçi (kapsam dışı; stok girişi elle/Excel — task 32).

## Bir siparişin hayatı

Kanalda doğar → ödeme kanalda alınır → bize ulaşır (webhook + polling, kural 34) →
**rezervasyon** (eldeki dokunulmaz) → satılabilir düştüğü için **tüm kanallara push** →
toplama/paketleme → **sevkiyat** (eldeki ve rezerve birlikte düşer, deftere çıkış, kanala
takip no) → teslim.

Sapmalar: sevkiyat **öncesi** iptal → rezerve serbest, defterde iz yok.
Sevkiyat **sonrası** iade → önce çıkış vardı; malın fiziksel dönüşünde ayrı bir giriş
satırı (kural 13).

---

# Kurallar

## Otorite ve akış

1. **Otorite bizdedir.** Aynı raf için N kopya sayı var (kanallar + biz); kanallar
   **replikadır**. Kendi sitemiz de bir kanaldır — 5. kanal eklemek yeni kod değil
   konfigürasyon olmalı. Otorite ≠ fiziksel gerçek: raftaki sayıyı garanti edemeyiz,
   yalnızca anlaşmazlıkta bizim sayımız esas alınır.

2. **Akış tek yönlü.** Stok: biz → kanallar (push). Sipariş: kanallar → biz (kural 34).
   Hiçbir sayı iki yöne gitmez — gitseydi sayı kendi kuyruğunu kovalardı. İstisna:
   mutabakatta kanalın sayısını okuruz, **düzeltmek için — inanmak için değil.**

## Oversell ve tampon

3. **Oversell yapısal olarak engellenemez, azaltılır.** Push ile varış arasında pencere
   var; pencereyi rezervasyon + hızlı push + mutabakat daraltır. Tampon bunu yapmaz:
   12 adet 3 kanala 10'ar bildirilince dışarıda 30 görünür. Tampon o uçurumu değil, tek
   kanalın pencere içindeki taşmasını karşılar.

4. **Tampon ürün bazındadır.** Maliyeti satılmayan maldır (800 üründe tampon 2 = 1600 adet
   kilitli). Hızlı dönen + ucuz → yüksek; yavaş dönen + pahalı → 0; tek kanallı ürün → 0.
   Kaydı bozmaz, yalnızca ilan edilen sayıyı kısar (eldeki hâlâ 12'dir).

## Stok hareketi ve invariant

5. **Sipariş eldeki'ye dokunmaz.** Dokunsaydı envanter farkı yorumlanamazdı ("sistem 12,
   rafta 11" hırsızlık mı bekleyen sipariş mi belirsiz olurdu) ve iptal deftere sahte
   çıkış + sahte giriş yazdırırdı. Rezervenin sahibi ve yaşı vardır, eldeki anonimdir.

6. **Eldeki sevkiyat anında düşer.** Tek işlemde: eldeki 12→11, rezerve 1→0, hareket
   defterine (task 08) çıkış satırı. Satılabilir değişmez.

7. **Invariant:** rezerve ≤ eldeki.

## Siparişin stoğa etkisi

8. **Otorite alan bazındadır, sistem bazında değil.** Tek sipariş nesnesi, alanları farklı
   sahiplerin: siparişin **ticari** yaşam döngüsünde kanal otoritedir, **karşılama**
   döngüsünde biz. İki döngü aynı sipariş üstünde paralel akar.

   | Alan | Sahibi |
   |---|---|
   | Müşteri, adres, sipariş satırları, fiyat | Kanal — okuruz, değiştirmeyiz |
   | Ödeme alındı mı | Kanal |
   | Müşteri iptal/iade talep etti mi | Kanal |
   | Rezervasyon | **Biz** |
   | Toplama / paketleme | **Biz** |
   | Sevkiyat + kargo takip no | **Biz** — kanala push edilir |

9. **Stok fiziksel olayla değişir, ticari olayla değil.** Kanal "iptal edildi" dediğinde
   mal kargodaysa stoğa hiçbir şey yapılmaz; sipariş "iade bekleniyor" işaretlenir. Eldeki
   ancak mal fiziksel olarak geri girdiğinde artar ve deftere o an giriş satırı yazılır.

10. **Sipariş durumu, sipariş satırlarının türevidir.** Rezervasyon ve sevkiyat satır
    düzeyinde yaşar; "kısmen sevk edildi" bir durum değil, satırların özetidir. Kanalın
    gönderdiği durum ayrı bir alanda saklanır, bizimkini **ezmez**; uyuşmazlık gizlenecek
    bir hata değil, operatöre çıkan bir iş kaydıdır (task 29). Ekranda bizim durumumuz
    baskındır — eylemi o belirler; kanal durumu bağlam olarak ikincil gösterilir.

11. **Satılabilir değişince tüm kanallara push edilir**, yalnız siparişin geldiği kanala
    değil. Satılabilir tek bir hesaptır; bir kanalı bilerek bayat bırakmak oversell üretir.

12. **Raf sayımı farkı ayrı bir olaydır.** "Sistem 1, rafta 0" bir sipariş sorunu değil bir
    **stok düzeltmesi**dir: deftere sebebi "sayım farkı" olan ayrı bir satır yazılır. Aynı
    anda oluşsalar bile sipariş kaydı ve düzeltme kaydı karışmaz.

13. **İade otomatik olarak stoğa girmez.** Malın geri geldiği haberi eldeki'yi artırmaz;
    depoda muayene edilir, satılabilir bulunursa stok sorumlusu **elle stok girişi** yapar
    (sebep: iade girişi). Muayene sistemin dışındadır. Fiziksel olarak doğrulanmamış bir
    sayıyı otoriter kaynağa yazmak sistemin tüm vaadini bozar.

14. **Malın kalite durumu siparişe değil stoğa aittir.** Muayene bekleyen kutu, sipariş
    kapandıktan sonra da depoda durur — bu yüzden sipariş durum makinesinde temsil edilmez.
    Stok gerçeği siparişten uzun yaşar. (Kalite kovaları kapsam dışı — `fikirler.md`.)

## Sayı nerede tutulur

15. **Eldeki hem sayaçta hem defterde tutulur — bilinçli tekrar.** Sayaç
    (`StockItem.OnHand`) karar verir: kilitlenebilir, tek okumada gelir. Defter (task 08)
    kanıt verir: sayının nasıl oluştuğunu geriye dönük açıklar. İkisi **tek transaction'da**
    yazılır; ayrı yazılırsa defter güvenilmez olur.
    Yalnız defter tutulsaydı **kilitlenecek satır kalmazdı** — toplam bir sorgu sonucudur,
    satır değil; iki eşzamanlı sipariş ikisi de "yeter" der, ikisi de satar.

16. **Rezerve için de aynı ikilik.** `Reservation` kayıtları (hangi sipariş satırı, kaç adet,
    ne zaman) + `StockItem.Reserved` sayacı; sayaç aynı sebeple var: karar tek satır
    kilitlenerek verilsin.
    **Rezervasyonun süresi yoktur.** Sipariş bize ödemesi alınmış gelir (kural 8), sepet
    rezervasyonu yok. Rezervasyon yalnız iptalle ya da sevkiyatla ölür, zamanla değil.

17. **Satılabilir saklanmaz, hesaplanır.** Buna karşılık **kanal başına "son bildirilen
    sayı"** saklanır — satılabilir'in kopyası değil, kanalla aramızdaki iletişimin kaydı.
    Gereksiz push'u önler ve sapmanın yerini ayırır:

    | Uyuşmazlık | Arıza nerede |
    |---|---|
    | Satılabilir ≠ son bildirilen | **Bizde** — push yapılmamış ya da başarısız |
    | Son bildirilen ≠ kanaldaki | **Kanalda** — push kabul edilmiş ama uygulanmamış |

    Bu sütun olmadan "kanalda 8, bizde 6" bilgisinin teşhis değeri yoktur.

18. **Stok kaydı varyant başınadır — tek depo varsayılır.** Bedeli tablo değil **karar**:
    çok depoda anahtar `(Varyant, Depo)` olur ve arkasından "bu sipariş hangi depodan
    karşılanacak" sorusu gelir. Ayrı ve büyük bir problem, bitiş tanımına katkısı yok.
    README'ye yazılır.

19. **Invariant'ın iki farklı hâli var.** *Rezervasyon yapılırken* rezerve ≤ eldeki
    pazarlıksızdır. *Fiziksel düzeltme sonrası* geçici olarak kırılabilir: eldeki 12 /
    rezerve 11 iken sayım 9 derse rezerve eldeki'yi aşar — rafın gerçeği bizim kuralımıza
    uymak zorunda değildir.
    Sistem bunu **kendiliğinden düzeltmez**: hangi rezervasyonun iptal edileceği teknik
    değil ticari bir karardır. Yapılan: satılabilir eksiye düşer, push'ta **0'a kırpılır**
    ve **operatöre iş kaydı çıkar** (task 29). 0 push etmek sorunu çözmez, büyümesini
    durdurur. Eldeki asla negatif olamaz (fiziksel sayı); satılabilir olabilir (hesap).

20. **Karar yalnız kaynaktan verilir.** Aynı sayının içeride de kopyaları var: veritabanı
    satırı (**kaynak**), Redis cache (hız kopyası), son bildirilen sayı (iletişim kaydı),
    defter toplamı (kanıt). Rezervasyon kararı **yalnız veritabanı satırından** verilir.
    Sebep bayatlık değil: **kontrol ile yazma aynı işlemin içinde, aynı kilitli satırda**
    olmak zorundadır; cache kilitlenebilecek bir şey olmadığı için bunu yapısal olarak
    sağlayamaz. Redis okuma yükünü ve ekranı hızlandırır.

## Katalog: ürün, varyant, ilan

21. **Stok varyantta yaşar, üründe değil.** Ürün bir gruplamadır; başlık ve görsel taşır,
    sayı taşımaz — "bu üründen 40 var" eyleme çevrilemez, sipariş 42 numara gelir. Her
    ürünün **en az bir varyantı** olur; tek çeşit üründe de otomatik tek varyant yaratılır,
    böylece "varyantlı mı" diye ayrılan ikinci bir kod yolu doğmaz. Panelde ürün seviyesinde
    gösterilecek sayı toplam değil, **kritik seviyenin altındaki varyant sayısıdır**.

22. **Üç ayrı kod vardır ve karıştırılmaz.** SKU bizimdir, barkod üreticinindir (bizde
    opsiyonel alan), kanal kodu kanalındır.
    **SKU birincil anahtar değildir:** iş kodudur, yazım hatası düzeltilir; anahtar olsaydı
    her düzeltme hareket defteri dahil tüm referansları güncellemeyi, yani geçmişi yeniden
    yazmayı gerektirirdi. Anahtar yapay `Id`'dir, SKU **tekil indeksli** iş kodudur.
    **SKU asla yeniden kullanılmaz.** Kapanan varyant silinmez, pasife alınır; satır durduğu
    sürece tekil indeks aynı kodun yeniden verilmesini zaten reddeder. Silinseydi defter
    "bu koddan mart'ta 40 çıkmış" derken iki farklı malı toplardı.

23. **İlan kaydı köprüdür: (Kanal, Varyant, Kanal kodu).** İki zıt yönlü işi birden yapar —
    dışarı: push'un hangi kanala hangi kodla gideceğini söyler; içeri: gelen sipariş
    satırındaki yabancı kodu bizim varyantımıza çevirir. Bu çeviri olmadan rezervasyon
    yapılamaz. Kural 17'deki "son bildirilen sayı" ayrı bir tablo değil, **bu satırın
    sütunudur**.

24. **Push kanal başına değil ilan başına yapılır.** Bir varyantın aynı kanalda birden çok
    ilanı olabilir (kapanmamış eski ilan, kampanya ilanı). Hepsi **aynı stoğu** tüketir ve
    hepsine **aynı sayı** gider — satılabilir ilanlar arasında **bölüştürülmez**;
    bölüştürmek oversell'i azaltmaz, satış kaybettirir.
    Ters yön tekildir: bir ilan tek bir varyanta bağlanır. Paket/set ilanı kapsam dışıdır.

25. **Eşleşmeyen sipariş satırı kaydedilir, rezerve edilmez.** Kanal koduyla otomatik ürün
    **yaratılmaz** — yaratmak katalog otoritesini tersine çevirir. Sipariş reddedilemez de
    (kanalda satılmış, para alınmış). Yapılan: satır "eşleşmemiş" işaretlenir, operatöre iş
    kaydı çıkar (task 29). Bu sırada satılabilir hiç değişmediği için kanallara olduğundan
    fazlası bildirilmeye devam eder — delinen şey doğrudan bitiş tanımıdır.
    Operatör eşleştirmeyi kurunca rezervasyon **denenir**; stok yetmiyorsa satır "stok
    yetersiz" iş kaydına düşer. Kimin mağdur edileceği ticari karardır, sistem seçmez.

26. **Katalogda otorite bizdedir, fiyat ve kampanya kanalda kalır.** İlan **açma** kapsam
    dışıdır: ilan kanalda elle açılır, bize yalnızca eşleştirme girilir (elle ya da Excel —
    task 32).

## Sipariş durum makinesi

27. **Sipariş kaydında iki ayrı durum alanı vardır.** `ChannelStatus` kanaldan gelen ham
    durumdur — **replikadır, karar vermez**, yalnız tetikleyici ve kayıttır.
    `FulfillmentStatus` bizimdir; stok hareketini yalnız o tetikler. Tek alana birleştirmek
    iki şeyi birden kırar: kanalın durum sözlüğü bizim iş kuralımız olur (yeni kanal artık
    konfigürasyon değil, makine değişikliğidir — kural 1) ve kanalın gecikmeli mesajı bizim
    karşılama gerçeğimizin üstüne yazar. Örnek: sevk sonrası "müşteri iptal etti" gelirse
    `ChannelStatus = Cancelled` yazılır, `FulfillmentStatus = Shipped` **kalır**, stok
    dokunulmaz, operatöre iş kaydı çıkar.

28. **Durum makinesi sipariş satırındadır; siparişin durumu türetilir.**
    Satır durumları: `Received` → `Unmatched` / `OutOfStock` / `Reserved` →
    `Shipped` / `Cancelled` → `Returned`.
    Depo adımları (`Picking`, `Packed`) **yoktur**: `Reserved` ile `Shipped` arasında hiçbir
    stok değişmez, makinede yer tutmazlar.
    Siparişin durumu = **iptal edilmiş satırlar hariç, en geride kalan satırın durumu**;
    hepsi iptalse sipariş iptaldir. Sıralama: `Unmatched`/`OutOfStock` < `Received` <
    `Reserved` < `Shipped` < `Returned`.
    Satır bazlı olmasının sebebi kural 25'tir: eşleşmemiş tek satır varken diğer satırlar
    rezerve edilebilmelidir. Tek sütun olsaydı iki yalandan biri seçilirdi — siparişi
    "sorunlu" saymak (sağlam satırlar rezerve edilmez, oversell penceresi açık kalır) ya da
    "rezerve" saymak (eşleşmemiş satır gizlenir).

29. **Geçişlerin stok etkisi ve yasak geçişler.**

    | Geçiş | Stok etkisi |
    |---|---|
    | `Received → Reserved` | rezerve **+** adet |
    | `Received → Unmatched` / `Received → OutOfStock` | yok |
    | `Unmatched → Reserved` / `OutOfStock → Reserved` | rezerve **+** adet |
    | `Reserved → Shipped` | eldeki **−**, rezerve **−**, deftere **çıkış** |
    | `Reserved → Cancelled` | rezerve **−**, defterde iz yok |
    | `Unmatched` / `OutOfStock → Cancelled` | yok |
    | `Shipped → Returned` | **yok** (kural 13) |

    Yasak ve sebebi: `Shipped → Cancelled` (çıkış deftere yazıldı, geri alınmaz — iade yolu
    vardır), `Cancelled → *` (terminal), `Received → Shipped` (rezerve edilmemiş mal sevk
    edilemez), `Reserved → Reserved` (ikinci rezervasyon stok sızıntısıdır).
    Defter yalnız **fiziksel** hareketi yazar; rezervasyon sayaç üzerindeki ticari bir
    iddiadır ve kendi kaydı vardır (kural 16), iptalde o kayıt kapanır.

30. **Rezervasyon sipariş bize ulaştığı anda yapılır**, kanaldan "onaylandı" beklenmeden.
    Para kanalda zaten alınmıştır (kural 8); ikinci bir sinyali beklemek oversell penceresini
    bizim kontrolümüz dışında büyütür. Bedeli kabul edilmiştir: iptal olacak siparişler bir
    süre stok tutar. Yan fayda: rezervasyon **giriş yoluna** oturur, böylece eşzamanlılık
    (son 1 adet için iki sipariş) ve tekrarlanan mesaj aynı kod yolunda karşılanır.

31. **Durum makinesi tekrarlanan *geçişi* karşılar, tekrarlanan *oluşturmayı* karşılamaz.**
    `Shipped → Shipped` tanımsız olduğu için ikinci "kargolandı" mesajı zararsızdır. Ama aynı
    "yeni sipariş" mesajı iki kez düşerse ortada henüz durum yoktur: iki sipariş yaratılır,
    stok iki kez rezerve edilir. Oluşturma yolu bu yüzden ayrı bir **idempotency anahtarına**
    ihtiyaç duyar — `(Kanal, KanalSiparişNo)` üzerinde tekil indeks (task 21).
    Her iki mekanizma da ancak kontrol ile yazma **aynı işlemde, aynı kilitli satırda** ise
    çalışır (kural 20).

32. **`Returned` durumunun stok etkisi yoktur** — makinede yalnız kayıt olan bir durumdur;
    mal fiziksel olarak dönse bile eldeki artmaz (kural 13). Yine de ayrı bir durum olarak
    durur: `Shipped`'i terminal bırakmak, iadeyi `Cancelled`'a benzetme baskısını ortadan
    kaldırır.

33. **Kısmi sevkiyat kapsam dışıdır.** Sevkiyat **sipariş seviyesinde** bir eylemdir ve tüm
    satırları birlikte `Shipped` yapar; doğrudan sonucu: bir satır `Unmatched` veya
    `OutOfStock` iken sipariş sevk edilemez. (`fikirler.md`)

## Entegrasyon

Sipariş girişi: 34, 35, 39, 40, 41, 43. Stok push: 36, 37, 38, 42.

34. **Sipariş iki yoldan girer: webhook taşır, polling garanti eder.** Webhook hızlıdır ama
    kaybolabilir ve **kaybın sinyali yoktur** — gelmeyen mesaj "0 hata" olarak görünür. Bu
    yüzden webhook varken polling kapatılmaz; uzun aralıklı **güvenlik ağına** dönüşür ve
    penceresi geniş tutulur (kesinti/deploy sonrası boşluğu geriye dönük tarar). Aralık "en
    küçük sayı" değildir: kota sipariş çekme ile stok push arasında paylaşılır.

35. **Webhook gövdesine güvenilmez, yalnız kimliğine güvenilir** (thin payload). Mesajdan
    sipariş numarası alınır, sipariş kanaldan yeniden çekilir. Üç sebep: otorite kanaldadır
    ve gövde eski bir fotoğraftır; endpoint internete açıktır, URL'yi bilen sahte gövde
    uydurabilir (imza doğrulaması ayrıca yapılır — task 21); çekilen veri sıra
    bozukluğundan etkilenmez (kural 43).

36. **Push mutlak sayı taşır, fark değil.** "Stok 7 olsun", "3 azalt" değil. Fark
    gönderilseydi kaybolan veya iki kez işlenen tek bir mesaj **kalıcı** sapma bırakırdı;
    mutlak sayıda bir sonraki başarılı push zararı siler. Push bu sayede doğası gereği
    idempotent'tir ve birleştirilebilir.

37. **Push kuyruğu kanal başına ayrılır ve kanalın limitinin altında akar.** Ortak kuyrukta
    ölü bir kanal diğerlerini bekletir (head-of-line blocking); devre kesici de kanal
    başınadır. Aynı ilan için bekleyen push'lar tek push'a indirilir — kuyruk boyu değişiklik
    sayısıyla değil **ilan sayısıyla** sınırlanır. `429`'a tepki vermek yetmez: sürekli
    çalışan bir kendi hız sınırımız (client-side throttle) olur, yoksa sistem patlama → ceza
    → patlama döngüsüne girer. `Retry-After` gelirse süresine uyulur; yeniden denemeler
    backoff + jitter ile yapılır.

38. **Push en iyi çabadır; doğruluğun garantisi mutabakattır.** Kanal erişilemezken sipariş
    alınır ve rezervasyon yapılır — satılabilir bizde doğrudur, kanalda bayattır. "Son
    bildirilen sayı" (kural 17) **yalnız başarılı push'ta** güncellenir; sapmanın ölçüsü
    odur. Erişilemeyen kanal operatöre görünür (task 29), stoğa elle dokunulmaz. Kanal
    dönünce kuyruk boşalır, artakalanı gece mutabakatı kapatır.

39. **Webhook alınır ve hemen onaylanır; işleme ayrı yürür.** Endpoint ham mesajı yazar,
    kuyruğa atar, `200` döner (kanal zaman aşımı kısadır, kural 35'teki çekim yavaş
    olabilir). Kanal için `2xx` "teslim edildi", `2xx` dışı "başarısız, yeniden dene"
    demektir; sürekli hata dönen endpoint bazı kanallarda devre dışı bırakılır. Bu yüzden
    **tekrarlanan mesaja da `200` dönülür** — istenen durum zaten sağlanmıştır. `5xx` yalnız
    mesajı gerçekten kaydedemediğimizde dönülür.

40. **Tekrarı engelleyen şey kilit değil, tekil indekstir.** Sipariş oluşturmada kilitlenecek
    satır yoktur; "önce bak, yoksa yarat" iki eşzamanlı kopyanın **ikisine de** boş görünür
    (check-then-act race). Koruma `(Kanal, KanalSiparişNo)` üzerindeki tekil indekstir:
    ikinci yazmayı veritabanı reddeder, kod bunu "zaten var" olarak işler.
    Anahtar **işin kimliğidir**, mesajın kimliği değil — aynı sipariş dört yoldan gelir
    (kanal tekrarı, örtüşen polling penceresi, webhook+polling, iç kuyruk) ve mesaj id'si her
    seferinde farklıdır. Anahtar gerçek tablonun üstünde olduğu için ayrı bir "görülen
    mesajlar" tablosu ve saklama süresi sorunu doğmaz.
    Projenin iki eşzamanlılık aracı ayrıdır: satır **varsa** kilit (kural 20), satır
    **yoksa** tekil indeks.

41. **"Zaten var" mesajı çöpe atılmaz, uygulanır.** İkinci mesaj çoğu zaman tekrar değil
    güncellemedir (iptal, adres değişikliği, durum değişimi) ve ikisi ayırt edilemez. Her
    mesajda veri kanaldan çekilir; yaratmayı tekil indeks, geçişleri durum makinesi korur.
    Idempotency "mesajı yok say" değil, **etkinin bir kez olması** demektir.

42. **Push mesajı sayıyı taşımaz, ilan kimliğini taşır; sayı gönderim anında kaynaktan
    okunur** (kural 20'nin push tarafı). Mutlak sayı idempotent yapar ama **sıraya dayanıklı
    yapmaz**: gecikmiş bir "7", başarılı bir "5"in üstüne binerse kanal fazla stok gösterir ve
    hiçbir mesaj bozulmamıştır. Değer mesajda taşınmayınca yeniden deneme kendiliğinden güncel
    değeri gönderir ve birleştirme bedava gelir. Aynı ilan için aynı anda iki gönderim uçuşta
    olmasın diye kanal kuyruğu **eşzamanlılık 1** ile işlenir.

43. **Gelen mesajların sırası garanti değildir; sırayı durum makinesi korur.** Kuyruk
    sıralıdır, eşzamanlı işleme sırayı bozar; yeniden denemeler, kanalın kendi sırasızlığı ve
    webhook–polling yarışı aynı sonucu verir. Bayat bir çekim uygulanmak istendiğinde geriye
    geçiş tanımlı olmadığı için reddedilir (kural 29) — ayrı bir sıra mekanizması kurulmaz.
    Makine yalnız **durumu** korur: adres, alıcı, kargo gibi durum dışı alanlarda kanalın
    kendi güncelleme zaman damgasına bakılır, daha eski fotoğraf yazılmaz.

## Aggregate ve transaction

44. **Aggregate'i kuran şey ilişki değil, korunacak kuraldır.** Üç özelliği tek cümlenin
    parçasıdır: *tutarlılık sınırı* (invariant her commit anında içeride doğrudur), *işlem
    sınırı* (varsayılan: bir transaction bir aggregate) ve *tek kapı* (dışarıdaki kod
    içerideki nesneyi değil kökün id'sini tutar).
    Sınırın boyutunu iki şey belirler: **invariant tabanı, çekişme tavanı.** Aggregate aynı
    zamanda **kilit birimidir** — sınırın tamamı kilitlenir. Invariant'ı içine alan en küçük
    sınır seçilir; büyütmek doğruluğa hiçbir şey katmaz, yalnızca eşzamanlılığı öldürür.

45. **İki aggregate vardır: `StockItem` (varyant başına) ve `Order`.** `StockItem` = stok
    sayaçları + o varyantın açık rezervasyonları; invariant (kural 7) burada yaşar.
    `Order` = sipariş + satırları + durum makinesi (kural 28); satır stoğa nesneyle değil
    `VariantId` ile bağlanır.
    Rezervasyon `Order`'ın içinde olsaydı invariant N ayrı siparişe dağılırdı: kilitlenecek
    tek satır kalmaz ve iki eşzamanlı sipariş birbirinin henüz yazılmamış rezervasyonunu
    göremezdi — task 10 yapısal olarak imkânsız olurdu. Sınır ürün ya da depo seviyesinde
    çizilseydi doğruluk değişmez, ama farklı varyantlara gelen siparişler ortak kuralları
    olmadığı hâlde sırayla beklerdi.

46. **Sipariş oluşturma tek transaction'dır; "bir transaction bir aggregate" bilerek
    bozulur.** Ayrı transaction'da sıra belirleyici olurdu ve iki kalıntı simetrik değildir:
    *önce sipariş* → rezervasyonsuz satır, ki bu zaten yasal bir durumdur (`Received`) ve
    görünür, kurtarılabilir; *önce rezervasyon* → **sahipsiz rezervasyon**, ve rezervasyonun
    süresi olmadığı için (kural 16) o adet sonsuza kadar kilitli kalır.
    Kural bozulabiliyor çünkü var oluş sebebi bize uymuyor: ortak transaction ancak
    aggregate'ler ayrı veritabanına düştüğünde imkânsız hale gelir — bizde ikisi aynı SQL
    Server'da, aynı process'te. Geri sarma hâlinde tekil indeks satırı da yok olur, polling
    (kural 34) siparişi tekrar getirir: **hep ya hiç + tekrar denenebilirlik**. Paylaşılan şey
    transaction'dır, sınır değil: yazma yine `StockItem` kökünden geçer.

47. **Transaction'ın içinde ağ çağrısı yoktur.** Kural 35'teki "siparişi kanaldan çek" adımı
    transaction açılmadan **önce** biter. İçeride olsaydı stok satırının kilidi kanalın
    timeout'u kadar tutulurdu ve o varyantı satan herkes beklerdi.

48. **Çok satırlı siparişte stok satırları deterministik sırayla kilitlenir** (`VariantId`'ye
    göre sıralı). Aynı iki varyanta ters sırayla dokunan iki sipariş birbirini bekler
    (deadlock); sıra sabitlenince bu durum oluşamaz.

## Modül sınırları

49. **Modül = bounded context, birebir.** Dört modül aynı veritabanını ve process'i paylaşır;
    ayıran şey sahiplik ve kelime dağarcığıdır. Taktik DDD yalnız invariant'ın olduğu yerde
    uygulanır: **Inventory** ve **Ordering** aggregate kullanır; **Integration** ve
    **Identity** düz CRUD'dur. Integration'ın doğruluğu nesne grafiğinden değil üç dış
    mekanizmadan gelir — tekil indeks (40), durum makinesi (43) ve kuyruk (37).

50. **Inventory kanalları tanımaz.** İlan kaydı (kural 23), kanal konfigürasyonu ve push
    kuyruğu Integration'dadır. Satılabilir değişince Inventory
    `AvailableChanged(VariantId, YeniSayı)` domain event'ini yayınlar; ilanları bulup kanal
    kuyruklarına dağıtmak (kural 11, 24) Integration'ın işidir. Aksi hâlde kanal tabloları
    invariant'ın yaşadığı modüle girerdi ve yeni kanal eklemek sistemin en riskli kodunu
    (oversell yolu) açmayı gerektirirdi.

51. **Sınırda çeviri yapılır (anti-corruption layer).** Integration'daki "sipariş" kanaldan
    çekilmiş ham veridir: kanal kodu, kanalın kendi durum sözlüğü, rezervasyon kavramı yok.
    Ordering'deki "sipariş" bizim nesnemizdir. Çeviri sınırda yapılır ve şekil değil **karar**
    içerir: kanal kodu → `VariantId` (ilan kaydına bakarak; bulunamazsa satır `Unmatched`,
    kural 25), kanalın durum kelimesi → `ChannelStatus` alanına ham yazılır ve bizim
    durumumuza asla dönüşmez (kural 27).

## Mutabakat

52. **Sapmanın iki ekseni vardır ve kanal mutabakatı yalnız birini görür.** *İletişim
    sapması*: bizdeki satılabilir ↔ kanaldaki sayı; gece işi (task 27) bunu bulur. *Fiziksel
    sapma*: bizdeki eldeki ↔ raftaki gerçek; bunu yalnız raf sayımı bulur (kural 12). Mal
    kaybolduğunda bizim sayımız da kanalın sayısı da aynı yanlışı gösterir; karşılaştırma
    pırıl pırıl uyuşur ve hiçbir şey bulunmaz.

53. **Sapmanın kaynakları sayılıdır.** Push hiç çıkmamış ya da kaybolmuş (arıza bizde); push
    kabul edilmiş ama kanal uygulamamış (arıza kanalda); kanalın panelinde birinin elle
    değiştirmesi; sipariş bize hiç ulaşmamış (webhook düştü *ve* polling kaçırdı); kanalda
    iptal edilmiş ama duymamışız — rezervasyonun süresi olmadığı için (kural 16) bu sapma
    kendiliğinden asla kapanmaz. Kanal yalnız kendi satışını bilir: "kanalda bizden fazla"
    imzasının olağan sebebi budur (kural 11).
    Eşleşmeyen sipariş satırı (kural 25) ayrı durur: rezerve etmediğimiz için bizim sayımız
    düşmez ve bir sonraki push kanalın kendi düşürdüğü doğru sayıyı **geri şişirir** — kendi
    kendini düzeltmeyen, üstelik yanlışı aktif olarak yayan tek sapma türüdür.

54. **Her uyuşmazlık sapma değildir.** Uçuştaki sipariş sistemin normal çalışma hâlidir;
    sipariş gelince rezervasyonla kapanır. Bu durum **her zaman "kanal < biz"** yönündedir,
    tersi imkânsızdır — yön tek başına yanlış teşhislerin çoğunu eler.
    Uyuşmazlık görüldüğü an düzeltilmez: kaydedilir, **sessiz pencere** sonunda tekrar
    bakılır; ayakta kalan gerçek sapmadır. Push kuyruğunda bekleyen işi olan ya da son
    dakikalarda stok hareketi görmüş ilana dokunulmaz. Anında düzeltmek, kanalın doğru olan
    düşük sayısını yukarı çeker — **mutabakatın kendisi oversell üretir.**

55. **Teşhis üç sayının imzasından çıkar** (kural 17'nin tamamlanmış hâli):

    | Bizde | Son bildirilen | Kanalda | Teşhis | Yapılacak iş |
    |---|---|---|---|---|
    | 6 | 6 | 8 | Kanalda: push uygulanmamış **ya da** panelden elle değişiklik | Push tekrar |
    | 6 | 8 | 8 | Bizde: push hiç çıkmamış — kayıp event, ölü consumer, tıkalı kuyruk | Push + kuyruk incelenir |
    | 6 | 6 | 5 | Arıza yok — uçuştaki sipariş (kural 54) | Dokunulmaz |

    Birinci satırdaki iki ihtimal ayırt edilemez ve edilmesi gerekmez; ikisinde de yapılacak
    iş aynıdır. İlk hamle her zaman push'tur — mesaj tek seferlik de kaybolmuş olabilir.
    Belirleyici olan **tekrardır**: aynı imza üst üste düşüyorsa bu sapma değil arızadır ve
    operatöre çıkar (task 29). Her gece sessizce yeniden push atan sistem arızayı düzeltmez,
    **gizler.**

56. **Gece işi tam tarar.** Kanalların toplu listeleme endpoint'i sayfa başına ~100 ilan
    döner; 4000 ilan 40 istek eder ve geceye yayılınca hiçbir limite değmez — ilan ilan sorgu
    atılmaz. Alt küme taramanın kapsamını değil **sıklığını** belirler: satılabilir'i sıfıra
    yakın ilanlar (oversell'in mümkün olduğu tek yer), push'u başarısız olmuş ilanlar ve
    erişilemez kalmış kanallar gün içinde de kontrol edilir. Devir hızı ayrı bir girdidir:
    sapmanın ne kadar çabuk tehlikeye dönüştüğünü söyler.

57. **Sipariş mutabakatı da yapılır.** Gece işi son N günün sipariş listesini kanaldan çeker
    ve `(Kanal, KanalSiparişNo)` ile bizimkilerle eşleştirir. Kanalda olup bizde olmayan
    sipariş normal yoldan işlenir (kural 35); ayrı bir yol yoktur ve güvenlidir, çünkü sipariş
    aslında bizdeyse tekil indeks ikinci yazmayı reddeder (kural 40). Mutabakat burada
    polling'in (kural 34) altındaki üçüncü ağdır.

58. **İç mutabakat sapma değil bug alarmıdır.** `SUM(defter) == OnHand` ve
    `Reserved == SUM(açık rezervasyonlar)` kontrolleri (kural 15, 16) tutmuyorsa rafta hiçbir
    şey olmamıştır: bir yazma transaction dışında yapılmıştır. **Otomatik düzeltilmez** —
    hangi tarafın doğru olduğu bilinemez, üstelik yanlış sayaçla verilmiş rezervasyon
    kararları zaten verilmiştir; sayıyı eşitlemek onları geri almaz, yalnız kanıtı siler.

59. **Otomatik düzeltme yalnız bizim otoritemizdeki bir replikaya yapılır.** Kanaldaki stok
    sayısı replikadır (kural 1) → üstüne yazılır. Bizde eksik sipariş bizim kaydımızdır →
    yaratılır. Rezerve > eldeki (kural 19) → insana çıkar. Defter ≠ sayaç (kural 58) → insana
    çıkar. Ayırt edici soru sapmanın büyüklüğü değil: **düzeltmek bir sayıyı üzerine yazmak
    mı, yoksa bir karar vermek mi?**

60. **Mutabakat push yolundan bağımsızdır; bu yüzden periyodiktir, push sonrası doğrulama
    değildir.** Bir güvenlik ağı denetlediği mekanizmayı paylaşamaz (kural 34'ün mantığı):
    doğrulama push'un üstüne binseydi, push'u kaybettiren arızalar doğrulamayı da aynı anda
    kaybederdi. Ayrıca kanal push'u çoğu zaman asenkron uygular; hemen ardından okumak bayat
    sayı döndürür, sahte sapma üretir ve push → oku → push döngüsüne girer.
