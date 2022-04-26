using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


namespace ASPdeFish
{
    public class DeFishEyeAlgorithm
    {
        public static Bitmap LoadBitmap(string fileName) // Создание Bitmap
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open,
                                                 FileAccess.Read, // FileAccess.Read - Доступ для чтения файла. Данные могут быть считаны из файла.
                                                 FileShare.Read)) // FileShare.Read -  Любой запрос на открытие файла для чтения
                                                                  //                   (данным процессом или другим процессом)
                                                                  //                   не выполняется до тех пор, пока файл не будет закрыт
                return new Bitmap(fs); // Возвращаем Bitmap                                                                      
        }

        public static ColorByteImage BitmapToByteImage(Bitmap btm)           // Метод, который переводит Bitmap в массив byte
        {
            int width = btm.Width,                                          // Задаем ширину и высоту, используя свойства созданного Bitmap 
                height = btm.Height;

            ColorByteImage result = new ColorByteImage(width, height);       // Создаем экземпляр класса ColorByteImage

            BitmapData bd = btm.LockBits(new Rectangle(0, 0, width, height), // LockBits - Блокирует объект Bitmap в системной памяти.
                                            ImageLockMode.ReadWrite,         // Rectangle - определяет часть растрового изображения для блокировки. 
                                            btm.PixelFormat);

            IntPtr ptr = bd.Scan0;                                          // Получаем адрес первого пикселя

            int bytes = Math.Abs(bd.Stride) * btm.Height;                   // Задаем размер массива
                                                                            // bd.Stride - количество байт в одной строке

            byte[] bitmap_array = new byte[bytes];                          // Создаем массив

            Marshal.Copy(ptr, bitmap_array, 0, bytes);                      // Копируем в массив значения из Bitmap

            for (int i = 0; i < height; i++)                                // Заполняем массив rawdata - значениями из bitmap_array
            {
                for (int j = 0; j < width; j++)
                {
                    byte b = bitmap_array[bd.Stride * i + j * 3];
                    byte g = bitmap_array[bd.Stride * i + j * 3 + 1];
                    byte r = bitmap_array[bd.Stride * i + j * 3 + 2];
                    result.rawdata[width * i + j] = new ColorBytePixel() { b = b, g = g, r = r };
                }
            }                                                               // После заполнения получим массив структур ColorBytePixel
                                                                            // со значениями каждого пикселя исходного изображения

            btm.UnlockBits(bd);                                             // Разблокировка объекта Bitmap

            return result;
        }

        public static double distance(int x, int y)                         // Расстояние от центра до точки
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static ColorByteImage deFishEyeImage(ColorByteImage byteImage, double factor)
        {
            int width = byteImage.Width,
                height = byteImage.Height;

            ColorByteImage newByteImage = new ColorByteImage(width, height);                 // Создаем экземпляр класса ColorByteImage

            double r, focus, theta, scale;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int newX = j - width / 2;                         // Перевод декартовых координат в полярные
                    int newY = i - height / 2;

                    r = distance(newX, newY);                         // Расстояние от центра до точки

                    focus = (distance(width, height) / factor);       // Фокусное расстояние - радиус сферы

                    theta = r / focus;

                    if (r == 0)
                    {
                        scale = 1.0;
                    }
                    else
                    {
                        scale = Math.Atan(theta) / theta;             // Коэффициент масштабирования
                    }

                    double sourceX = (width / 2) + scale * newX;    // Перевод полярных координат в декартовы
                    double sourceY = (height / 2) + scale * newY;

                    newByteImage.rawdata[j + width * i] = byteImage.rawdata[Convert.ToInt32(sourceX) +
                                                                            width * Convert.ToInt32(sourceY)];
                }
            }

            return newByteImage;
        }

        public static void ByteImageToFile(ColorByteImage byteImage, Bitmap btm, string image_path) // Переводим массив байтов в файл
        {
            int width = byteImage.Width,
                height = byteImage.Height;

            BitmapData bmpData = btm.LockBits(new Rectangle(0, 0, btm.Width, btm.Height),
                                                  ImageLockMode.ReadWrite, btm.PixelFormat);

            int bytes = bmpData.Stride * btm.Height;
            IntPtr ptr = bmpData.Scan0;

            byte[] bitmap_array = new byte[bytes];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bitmap_array[i * bmpData.Stride + j * bmpData.Stride / width] = byteImage.rawdata[j + width * i].b;
                    bitmap_array[i * bmpData.Stride + j * bmpData.Stride / width + 1] = byteImage.rawdata[j + width * i].g;
                    bitmap_array[i * bmpData.Stride + j * bmpData.Stride / width + 2] = byteImage.rawdata[j + width * i].r;
                }
            }

            Marshal.Copy(bitmap_array, 0, ptr, bitmap_array.Length);

            btm.UnlockBits(bmpData);
            btm.Save(image_path, ImageFormat.Png);
        }


        public static void SplitVideo(string videoPath, string framePath)
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-y -i {videoPath} {framePath}/out%5d.png",
            };

            using (var process = new Process { StartInfo = ffmpeg })
            {
                process.Start();
                process.WaitForExit();
            }
        }
        //public static void ExtractFrame(string path)
        //{
        //    var ffmpeg = new ProcessStartInfo
        //    {
        //        FileName = "ffmpeg.exe",
        //        Arguments = @$"-i {path} -vf 'select = eq(n\, 0)' -q:v 3 output_image.png",

        //    };

        //    using (var process = new Process { StartInfo = ffmpeg })
        //    {
        //        process.Start();
        //        process.WaitForExit();
        //    }
        //}

        public static void ExtractAudio(string path, string audioPath)
        {
            var ffmpegExtractAudio = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-y -i {path} -vn -acodec copy {audioPath}",
            };

            using (var process = new Process { StartInfo = ffmpegExtractAudio })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        public static void MergeFrames(string framePath, string outVideoPath)
        {
            var ffmpegImagesToVideo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-r 30 -y -i {framePath}/out%5d.png -c:v libx264 -vf fps=30 -pix_fmt yuv420p {outVideoPath}",
            };
            using (var process = new Process { StartInfo = ffmpegImagesToVideo })
            {
                process.Start();
                process.WaitForExit();
            }
        }


        public static void MergeAudio(string outVideoWOAudioPath, string audioPath, string outVideoPath)
        {

            var ffmpegAudioToVideo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-y -i {outVideoWOAudioPath} -i {audioPath} -c copy -map 0:v:0 -map 1:a:0 {outVideoPath}",
            };

            using (var process = new Process { StartInfo = ffmpegAudioToVideo })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        public static void deFishEye(string path, double factor, string copyPath)
        {
            Bitmap btm = LoadBitmap(path);

            ColorByteImage byteImage = BitmapToByteImage(btm);

            ColorByteImage newByteImage = deFishEyeImage(byteImage, factor);

            ByteImageToFile(newByteImage, btm, copyPath);
        }

        public static void deFishEye(string path, double factor)
        {
            Bitmap btm = LoadBitmap(path);

            ColorByteImage byteImage = BitmapToByteImage(btm);

            ColorByteImage newByteImage = deFishEyeImage(byteImage, factor);

            ByteImageToFile(newByteImage, btm, path);
        }
    }
}
