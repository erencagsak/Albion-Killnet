Albion-Killnet API
Albion-Killnet, Albion Online oyunundaki oyuncu istatistiklerini ve ölüm (killboard) verilerini otomatik olarak çekip işleyen, bulut üzerinde 7/24 kesintisiz çalışan bir arka plan (backend) servisidir.

🚀 Kullanılan Teknolojiler
ASP.NET Core (C#): Sistemin core'u, veri işleme mantığı ve API yönetimi.

Turso (Edge SQLite): Çekilen oyun verilerinin yüksek performansla ve düşük gecikmeyle depolandığı modern veritabanı.

Render.com: Projenin bulut ortamında barındırıldığı (hosting) sunucu altyapısı.

UptimeRobot: Render'ın ücretsiz planındaki uyku modunu (idle state) engellemek için sisteme entegre edilen otomatik tetikleyici.

⚙️ Sistem Mimarisi ve Çalışma Mantığı
Dış Kaynak Entegrasyonu: ASP.NET servisi, Albion Online'ın resmi API'lerine bağlanarak sunuculardaki (Americas, Asia, Europe) güncel verileri düzenli olarak çeker.

Veri Depolama: API'den elde edilen ham veriler işlenerek, dışa aktarıma hazır bir şekilde Turso veritabanına güvenli ve optimize bir şekilde kaydedilir.

Kesintisiz Operasyon: Render.com üzerinde yayınlanan bu yapı normal şartlarda 15 dakika işlem görmediğinde uyku moduna girmektedir. Bunu engellemek için UptimeRobot üzerinden sistemin /health uç noktasına (endpoint) düzenli pingler gönderilerek API'nin 7/24 uyanık ve istek karşılamaya hazır kalması sağlanmıştır.
