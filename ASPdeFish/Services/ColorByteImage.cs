using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPdeFish
{
    public struct ColorBytePixel // Структура, отвечающая за цвет пикселя
                                 // b - blue, g - green, r - red
    {
        public byte b, g, r;
    }
    public class ColorByteImage // Класс изображения, каждый пиксель которого
                                // выражен элементом массива структур ColorBytePixel
    {
        public int Width { get; private set; } // Ширина изображения
        public int Height { get; private set; } // Высота изображения
        public readonly ColorBytePixel[] rawdata; // Массив структур ColorBytePixel

        public ColorByteImage(int Width, int Height) // Конструктор класса
        {
            this.Width = Width;
            this.Height = Height;
            rawdata = new ColorBytePixel[Width * Height]; 
        }
    }

}
