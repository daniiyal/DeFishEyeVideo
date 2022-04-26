using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace defisheyetest
{
    public struct ColorBytePixel // Структура для сохранения цвета пикселя
    {
        public byte b, g, r;
    }
    public class ColorByteImage // Класс изображения
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public readonly ColorBytePixel[] rawdata;

        public ColorByteImage(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            rawdata = new ColorBytePixel[Width * Height];
        }
    }

}
