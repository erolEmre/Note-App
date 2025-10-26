## 📝 Not Uygulaması

Kullanıcıların giriş yaparak not ekleyebildiği, güncelleyebildiği ve silebildiği basit bir ASP.NET Core MVC uygulaması.

👉 Demo Link: [NoteApp](http://noteapp-dev.eba-psh22n6f.eu-north-1.elasticbeanstalk.com)

⚠️ HTTPS sertifikası için ACM yapılandırması devam ediyor. Şu an uygulama HTTP üzerinden erişilebilir.

## 🚀 Özellikler

🔑 Kullanıcı Yönetimi: ASP.NET Core Identity ile kullanıcı kaydı ve giriş.

📝 Not İşlemleri: Not ekleme, güncelleme, silme,inceleme.

🏷 Etiketleme Sistemi: Notlara bir veya birden fazla #tag ekleme.

🔍 Filtreleme & Sıralama: Tarihe göre sıralama ve etiketlere göre filtreleme.

✅ Validation: FluentValidation ile kullanıcı ve not doğrulama.

🛡 Authentication: Cookie Authentication (MVC) + JWT (API için hazır).

## 🛠 Kullanılan Teknolojiler

ASP.NET Core MVC

Entity Framework Core (PostgreSQL)

ASP.NET Identity

FluentValidation

JWT Authentication (API için hazır altyapı)

Bootstrap 5 (UI için)

## ⚙️ Kurulum & Çalıştırma
1. Bağımlılıklar

.NET 9 SDK

PostgreSQL

2. Migration & Database Update
dotnet ef database update

3. Çalıştırma
dotnet run


Uygulama varsayılan olarak https://localhost:5001 adresinde çalışır.

## 🌍 Deploy

Proje AWS Elastic Beanstalk üzerine deploy edilmiştir.
👉 Demo Link : [NoteApp AWS](http://noteapp-dev.eba-psh22n6f.eu-north-1.elasticbeanstalk.com)

⚠️ HTTPS sertifikası için ACM yapılandırması devam ediyor. Şu an uygulama HTTP üzerinden erişilebilir.

🗺 Yol Haritası

 ++ Kullanıcı kimlik doğrulama (Identity)

 ++ Not CRUD işlemleri

 ++ Etiketleme sistemi (#hashtag extraction)

 -- Not paylaşma özelliği (Share action)

 -- REST API ile mobil uyumlu hale getirme (JWT kullanarak)

 -- Unit Test eklenmesi

📷 Ekran Görüntüleri

![Login Sayfası](https://i.imgur.com/0De7u98.png)
![İlk Giriş](https://i.imgur.com/2QJj8FW.png)
![Not Listesi](https://i.imgur.com/5RCurAP.png)
![Kayıt Sayfası](https://i.imgur.com/Qw8WdV1.png)
![Özellikler](https://i.imgur.com/i1dVcCd.jpeg) 
![](https://i.imgur.com/M9zfwea.png)![](https://i.imgur.com/zd0hckj.png)
![Özellikler](https://i.imgur.com/xY7Kr85.png)![](https://i.imgur.com/zJVcuv0.png)
![](https://i.imgur.com/XDPj9BN.png)![](https://i.imgur.com/Nhs9WU8.png)


👨‍💻 Katkıda Bulunma

Fork et

Yeni bir branch aç (feature/yenilik)

Commit et

Pull request aç

📜 Lisans

MIT
