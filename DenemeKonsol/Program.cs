using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace DenemeKonsol
{
    internal class Program
    {
        private static QRCodeData _qrCodeData;
        private static string logoPath;
        private static string qrUrl;
        private static string qrCodeName;

        static void Main(string[] args)
        {
            try
            {
                FileName();
                QrCodeUrl(); // QR kod URL'sini al
                GenerateTheQr(); // QR kodu oluştur

                // Logolu veya logosuz QR kod oluşturma
                if (QrLogoOption())
                {
                    Console.WriteLine("Enter The Path Of Logo");
                    logoPath = Console.ReadLine();
                    Bitmap qrWithLogo = CreateTheQrWithLogo(logoPath);
                    SaveQrCodeWithLogo(qrWithLogo, qrCodeName + ".png");
                }
                else
                {
                    SaveTheQr(qrCodeName + ".png");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Kullanıcıdan URL alma metodu
        private static void QrCodeUrl()
        {
            Console.WriteLine("Enter the URL for the QR code:");
            qrUrl = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(qrUrl))
            {
                Console.WriteLine("Qr code couldn't be created. The URL is empty.");
                Environment.Exit(1); // Hata kodu 1 ile uygulamadan çık
            }
        }
        private static void FileName()
        {
            qrCodeName = "newqr";
            Console.WriteLine("Give The File Name");
            qrCodeName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(qrCodeName))
            {
                Console.WriteLine("Qr code couldn't be created. The Name is empty.");
                Environment.Exit(1);
            }
        }
        // QR kodun logolu olup olmayacağını sorar
        private static bool QrLogoOption()
        {
            Console.WriteLine("Do you want the QR code with a logo? (y/n)");
            string option = Console.ReadLine();

            return option?.Trim().ToLower() == "y";
        }

        // QR Kod Verisi Oluştur
        static void GenerateTheQr()
        {
            var qrGenerator = new QRCodeGenerator();
            _qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
        }

        // QR Kod Görüntüsü Oluştur ve Logoyu Ekleyerek Geri Döndür
        static Bitmap CreateTheQrWithLogo(string logoPath)
        {
            var qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            if (!File.Exists(logoPath))
            {
                throw new FileNotFoundException("File doesn't exist.", logoPath);
            }

            Bitmap logo = new Bitmap(logoPath);
            int logoSize = qrCodeImage.Width / 3;
            Bitmap resizedLogo = new Bitmap(logo, new Size(logoSize, logoSize));

            using (Graphics g = Graphics.FromImage(qrCodeImage))
            {
                int logoPositionX = (qrCodeImage.Width - resizedLogo.Width) / 2;
                int logoPositionY = (qrCodeImage.Height - resizedLogo.Height) / 2;
                g.DrawImage(resizedLogo, logoPositionX, logoPositionY, resizedLogo.Width, resizedLogo.Height);
            }

            return qrCodeImage;
        }

        // QR Kod Görüntüsünü Kaydet
        static void SaveTheQr(string fileName)
        {
            try
            {
                Bitmap qrCodeImage = CreateTheQr();
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                qrCodeImage.Save(filePath, ImageFormat.Png);
                Console.WriteLine($"QR code saved successfully: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the QR code: {ex.Message}");
            }
        }

        // QR Kod Görüntüsünü Logoyla Kaydet
        static void SaveQrCodeWithLogo(Bitmap qrCodeWithLogo, string fileName)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                qrCodeWithLogo.Save(filePath, ImageFormat.Png);
                Console.WriteLine($"QR code saved successfully: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the QR code: {ex.Message}");
            }
        }

        // QR Kod Görüntüsü Oluştur
        static Bitmap CreateTheQr()
        {
            var qrCode = new QRCode(_qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}
