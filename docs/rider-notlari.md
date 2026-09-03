# Rider Notları

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

## Çalışma akışı

1. Terminalde `claude`
2. Claude dosyaları diskte değiştirir, Rider anında görür
3. Review: Commit penceresi → değişen dosyaya tıkla → diff
4. Düzeltmeyi terminalde söyle ya da doğrudan editörde yap

## Ekran görüntüsü

`Ctrl+V` ile yapıştırma Windows'ta güvenilir değil. Görüntüyü `docs/img/` içine kaydet,
mesajda yolunu ver.

## Öğrendiklerim

**Klasörü değil solution'ı aç.** File → Open → `OrderSync.sln`. Klasör olarak açarsan Rider
projelerin birbirine referansını bilmez; kod doğru olsa ve `dotnet build` temiz geçse bile
`using Ordering;` gibi satırlar kırmızı görünür.

**Solution Explorer'ın iki görünümü var.** Panelin üstündeki seçici:
- **Solution** — `.sln`'deki mantıksal ağaç. `bin`/`obj` görünmez. Varsayılan bu olmalı.
- **File System** — diskteki her şey, `bin`/`obj` dahil.

**`.csproj` açmak.** Solution görünümünde ayrı dosya olarak listelenmez; proje düğümünün kendisi
o dosyadır. Sağ tık → Edit → `Edit 'X.csproj'`, ya da `Ctrl+Shift+T` ile adını yaz.
`Directory.Build.props` gibi projeye ait olmayan dosyalar için de `Ctrl+Shift+T`.
