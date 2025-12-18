# Real-time, Database-Driven Markov Chain Text Generator

**Bu proje, klasik bir istatistiksel metin üretme algoritması olan Markov Zinciri'ni, modern ve interaktif bir web uygulamasına dönüştüren bir teknoloji demosudur.**

Kullanıcı tarafından sağlanan metinlerden öğrenerek, belirli bir yazarın veya metnin tarzını taklit eden, tutarlı ve akıcı yeni metinler üretebilir. Proje, basit bir konseptin veritabanı entegrasyonu ve gerçek zamanlı web teknolojileriyle nasıl ölçeklenebilir ve kullanıcı dostu bir ürüne dönüştürülebileceğini göstermektedir.

|                                                                         **Eğitim Arayüzü (Train Page)**                                                                         |                                                     **Üretim Arayüzü (Generate Page)**                                                     |
| :-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------------------------------: |
|                                                                    ![Eğitim Arayüzü](assets/train-page.png)                                                                     |                                                ![Üretim Arayüzü](assets/generate-page.png)                                                 |
| _Model, bu arayüz üzerinden büyük metinlerle beslenerek eğitilir. Sistem, kullanıcı deneyimini iyileştirmek için tahmini bir süre ve simüle edilmiş bir ilerleme çubuğu sunar._ | _Kullanıcı bir başlangıç metni girer ve model, cümlenin geri kalanını SignalR kullanarak gerçek zamanlı olarak ekrana akıtır (streaming)._ |

## Kavramsal Bakış

Bu proje üç temel teknolojik sütun üzerine inşa edilmiştir:

### 1\. İstatistiksel Öğrenme ve Metin Üretimi

Projenin kalbinde, metin verilerindeki kelime sıralamalarının istatistiksel olasılıklarını öğrenen bir **N-gram Markov Zinciri** modeli bulunur. Sistem, "hangi kelime diziliminden sonra hangi kelimenin gelme olasılığı en yüksektir?" sorusunu sorarak metin üretir. Bu, modelin belirli bir yazarın üslubunu, kelime seçimlerini ve cümle yapılarını taklit etmesini sağlar.

### 2\. Ölçeklenebilir Hafıza ve Kalıcılık

Modelin "hafızası", basit bir dosya yerine **SQLite** veritabanında saklanır. Entity Framework Core aracılığıyla yönetilen bu veritabanı, modelin şunları yapmasına olanak tanır:

-   **Artımlı Öğrenme:** Her yeni eğitim verisi, mevcut bilgi birikiminin üzerine eklenir.
-   **Büyük Veri Setleri:** Milyonlarca N-gram'ı verimli bir şekilde saklayabilir ve sorgulayabilir.
-   **Kalıcılık:** Öğrenilen bilgiler, uygulama yeniden başlasa bile kaybolmaz.

### 3\. Gerçek Zamanlı Etkileşim

Kullanıcı deneyimini statik web sayfalarının ötesine taşımak için **SignalR** teknolojisi kullanılmıştır. Metin üretme işlemi başladığında, sayfa yenilenmez. Bunun yerine, üretilen her kelime veya kelime öbeği, sunucudan kullanıcının tarayıcısına anında "akıtılır" (streaming). Bu, kullanıcıya sistemin "düşünme" sürecini canlı olarak izleme imkanı sunarak dinamik ve interaktif bir deneyim yaratır.

## Öne Çıkan Özellikler

-   **Dinamik Metin Üretimi:** Kullanıcının verdiği başlangıç metnine (seed phrase) dayanarak yeni ve özgün metinler oluşturur.
-   **Ayarlanabilir Uzunluk:** Üretilecek metnin kelime sayısı kullanıcı tarafından belirlenebilir.
-   **Akıllı Metin Biçimlendirme:** Üretilen metin, noktalama işaretleri ve tırnak işaretleri gibi dilbilgisi kurallarına uygun, temiz ve okunabilir bir formatta sunulur.
-   **Ayrılmış Eğitim ve Üretim Arayüzleri:** Biri modeli yeni verilerle beslemek, diğeri ise modelden metin üretmek için tasarlanmış iki farklı ve kullanıcı dostu arayüz.

## Teknoloji Mimarisi

-   **Backend:** ASP.NET Core MVC, SignalR, Entity Framework Core (.NET 8)
-   **Veritabanı:** SQLite
-   **Frontend:** Razor Pages, JavaScript, Bootstrap 5
-   **Core Algoritma:** N-gram tabanlı Markov Zinciri

## Kurulum ve Çalıştırma Detayları

Bu projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin.

**Ön Gereksinimler:**

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) veya üstü
-   EF Core CLI aracı (`dotnet tool install --global dotnet-ef`)

**Adımlar:**

1.  **Projeyi Klonlayın:**
    ```bash
    git clone <bu_projenin_github_adresi>
    cd MarkovWebApp
    ```
2.  **Bağımlılıkları Yükleyin:**
    ```bash
    dotnet restore
    ```
3.  **Veritabanını Oluşturun:**
    ```bash
    dotnet ef database update
    ```
4.  **Uygulamayı Çalıştırın:**
    ```bash
    dotnet run
    ```
    Uygulama varsayılan olarak `http://localhost:5xxx` gibi bir adreste çalışmaya başlayacaktır.
