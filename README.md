Library Reservation System
📌 Proje Hakkında

Bu proje, bir eğitim kurumu için geliştirilmiş basit bir kütüphane yönetim ve rezervasyon sistemidir. Kullanıcıların sistem üzerinden kitapları görüntüleyebilmesi ve rezervasyon yapabilmesi amaçlanmıştır.

Proje, temel seviyede bir web uygulaması olarak geliştirilmiş olup kullanıcı yönetimi ve rezervasyon süreçlerini kapsamaktadır.

🚀 Özellikler
👤 Kullanıcı kayıt olma (Sign Up)
🔐 Kullanıcı giriş yapma (Login)
📖 Kitapları listeleme
🔍 Kitap bilgilerini görüntüleme
📅 Kitap rezervasyonu oluşturma
❌ Rezervasyon işlemleri
🛠️ Kullanılan Teknolojiler
ASP.NET Core (.NET)
Microsoft SQL Server
ADO.NET (SqlConnection, SqlCommand)
HTML / CSS (temel arayüz)
🗄️ Veritabanı Yapısı

Projede temel olarak aşağıdaki tablolar kullanılmaktadır:

Users
Id
Name
StudentNumber
Password
Books
Id
BookName
Author
Status
Reservations
Id
UserId
BookId
ReservationDate
⚙️ Kurulum
1. Projeyi Klonla
git clone https://github.com/kullaniciadi/rezervation.git
2. Proje Klasörüne Gir
cd rezervation
3. Veritabanını Ayarla
SQL Server üzerinde bir veritabanı oluştur
Gerekli tabloları oluştur
Connection string’i proje içinde güncelle
4. Uygulamayı Çalıştır
dotnet run
⚠️ Notlar
Bu proje geliştirme aşamasındadır
Güvenlik iyileştirmeleri (şifre hashleme, authentication vb.) henüz eklenmemiştir
Eğitim ve öğrenme amaçlı geliştirilmiştir
🎯 Geliştirme Planları
🔐 Güvenli authentication sistemi eklenmesi (JWT / Cookie)
🔑 Şifrelerin hashlenmesi
🧠 İş kuralları (aynı kitabın çakışan rezervasyonlarının engellenmesi)
🧑‍💼 Admin paneli
📊 Gelişmiş veritabanı ilişkileri
🎨 Frontend iyileştirmeleri
👨‍💻 Geliştirici

Bu proje bireysel olarak geliştirilmiştir ve backend geliştirme pratiği yapmak amacıyla oluşturulmuştur.
