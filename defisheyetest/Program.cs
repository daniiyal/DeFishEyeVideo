using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading.Tasks;


namespace defisheyetest
{
    public class Program
    {
        public static Bitmap LoadBitmap(string fileName) // Создаем Bitmap из файла
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return new Bitmap(fs);
        }

        public static ColorByteImage BitmapToByteImage(Bitmap btm) // Преобразуем Bitmap в массив байтов
        {
            int width = btm.Width,
                height = btm.Height;

            ColorByteImage result = new ColorByteImage(width, height);

            BitmapData bd = btm.LockBits(new Rectangle(0, 0, width, height),
                                            ImageLockMode.ReadWrite,
                                            btm.PixelFormat);

            IntPtr ptr = bd.Scan0;
            int bytes = bd.Stride * btm.Height;

            byte[] bitmap_array = new byte[bytes];

            Marshal.Copy(ptr, bitmap_array, 0, bytes);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte b = bitmap_array[bd.Stride * i + j * 3];
                    byte g = bitmap_array[bd.Stride * i + j * 3 + 1];
                    byte r = bitmap_array[bd.Stride * i + j * 3 + 2];
                    result.rawdata[width * i + j] = new ColorBytePixel() { b = b, g = g, r = r };
                }
            }

            btm.UnlockBits(bd);

            return result;
        }

        public static double distance(int x, int y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static ColorByteImage deFishEyeImage(ColorByteImage byteImage)
        {
            int width = byteImage.Width,
                height = byteImage.Height;

            double factor = 5.0;
            double theta;
            ColorByteImage newByteImage = new ColorByteImage(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int newX = j - width / 2;
                    int newY = i - height / 2;

                    double r = distance(newX, newY) / (distance(width, height) / factor); // Отношение расстояния от центра до точки
                                                                                          // к диагонали исходного изображения

                    if (r == 0)
                    {
                        theta = 1.0;
                    }
                    else
                    {
                        theta = Math.Atan(r) / r;                              // это угол в радианах между точкой в реальном мире
                                                                               // и оптической осью, которая проходит от центра
                                                                               // изображения через центр объектива 
                    }

                    double sourceX = (width / 2) + theta * newX;                
                    double sourceY = (height / 2) + theta * newY;

                    newByteImage.rawdata[j + width * i] = byteImage.rawdata[Convert.ToInt32(sourceX) + width * Convert.ToInt32(sourceY)];
                }
            }

            return newByteImage;
        }

        public static void ByteImageToFile(ColorByteImage byteImage, Bitmap btm, string image_path)
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
                    bitmap_array[i * width * 3 + j * 3] = byteImage.rawdata[j + width * i].b;
                    bitmap_array[i * width * 3 + j * 3 + 1] = byteImage.rawdata[j + width * i].g;
                    bitmap_array[i * width * 3 + j * 3 + 2] = byteImage.rawdata[j + width * i].r;
                }
            }

            Marshal.Copy(bitmap_array, 0, ptr, bitmap_array.Length);

            btm.UnlockBits(bmpData);
            btm.Save(image_path, ImageFormat.Png);
        }


        static void SplitVideo()
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = "-i video.mp4 frames/out%5d.png",
            };

            using (var process = new Process { StartInfo = ffmpeg })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        static void ExtractAudio()
        {
            var ffmpegExtractAudio = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = "-i video.mp4 -vn -acodec copy output-audio.aac",
            };

            using (var process = new Process { StartInfo = ffmpegExtractAudio })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        static void MergeFrames()
        {
            var ffmpegImagesToVideo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = "-r 30 -y -i frames/out%5d.png -c:v libx264 -vf fps=25 -pix_fmt yuv420p out.mp4",
            };
            using (var process = new Process { StartInfo = ffmpegImagesToVideo })
            {
                process.Start();
                process.WaitForExit();
            }
        }


        static void MergeAudio()
        {

            var ffmpegAudioToVideo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = "-i out.mp4 -i output-audio.aac -c copy -map 0:v:0 -map 1:a:0 output.mp4",
            };

            using (var process = new Process { StartInfo = ffmpegAudioToVideo })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        static void deFishEyeImage(string path)
        {
            Bitmap btm = LoadBitmap(path);

            ColorByteImage byteImage = BitmapToByteImage(btm);

            ColorByteImage newByteImage = deFishEyeImage(byteImage);

            ByteImageToFile(newByteImage, btm, path);
        }


        static void Main(string[] args)
        {


            deFishEyeImage(@"C:\Users\danik\source\repos\defisheyetest\ASPdeFish\wwwroot\images\videothumb.png");

        
        }
    }
}
