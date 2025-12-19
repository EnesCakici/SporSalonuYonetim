# Vora - Spor Salonu Yönetim Sistemi

Bu proje, Sakarya Üniversitesi Bilgisayar Mühendisliği Bölümü **Web Programlama** dersi kapsamında geliştirilmiş, ASP.NET Core tabanlı kapsamlı bir spor salonu yönetim ve randevu sistemidir.

## Projenin Amacı
Spor salonu üyelerinin antrenörlerden kolayca randevu alabilmesini, salon hizmetlerini inceleyebilmesini ve **Yapay Zeka (AI)** destekli asistan sayesinde kişisel antrenman/beslenme tavsiyeleri alabilmesini sağlamaktır.

## Kullanılan Teknolojiler

* **Framework:** .NET 8.0 (ASP.NET Core MVC)
* **Dil:** C#
* **Veritabanı:** PostgreSQL
* **ORM:** Entity Framework Core (Code First)
* **Frontend:** Bootstrap 5, HTML5, CSS3, JavaScript
* **AI Entegrasyonu:** Groq API (LLM)
* **Versiyon Kontrol:** Git & GitHub

## Temel Özellikler

### Üye Paneli
* **Randevu Sistemi:** Müsait eğitmenlerden, çakışma kontrolü (Conflict Check) yapılarak randevu alınması.
* **AI Spor Asistanı:** Boy, kilo ve hedeflere göre kişiye özel program önerisi (Groq API).
* **Görüntüleme:** Hizmetleri ve eğitmen profillerini inceleme.

### Admin Paneli
* **CRUD İşlemleri:** Yeni eğitmen veya hizmet ekleme, düzenleme ve silme.
* **Rol Yönetimi:** Sisteme yetkili girişi ve güvenli erişim (Authorization).

### ⚙️ Teknik Detaylar
* **Validation:** Hem istemci (Client) hem sunucu (Server) taraflı veri doğrulama.
* **Data Seeding:** Proje ayağa kalkarken otomatik Admin ve Rol tanımlaması.

## 📦 Kurulum (Nasıl Çalıştırılır?)

1.  Projeyi bilgisayarınıza klonlayın.
2.  `appsettings.json` dosyasındaki Veritabanı bağlantı cümlesini (Connection String) kendi PostgreSQL ayarlarınıza göre düzenleyin.
3.  Package Manager Console üzerinden `Update-Database` komutunu çalıştırarak veritabanını oluşturun.
4.  Projeyi başlatın.

---
**Geliştirici:** Enes Çakıcı
**Ders:** Web Programlama (2025-2026 Güz)
