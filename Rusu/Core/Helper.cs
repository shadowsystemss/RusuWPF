using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rusu.Core
{
    /// <summary>
    /// Вспомогательный класс.
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        /// Получить текст с интернета или с файла.
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>Текст</returns>
        internal static string? ReadText(string path)
        {
            // Загрзука с интернета.
            if (path[0] == 'h'
             && path[1] == 't'
             && path[2] == 't'
             && path[3] == 'p')
                using (HttpClient client = new HttpClient())
                    return client.GetStringAsync(path).GetAwaiter().GetResult();
            // Чтение файла.
            if (File.Exists(path)) return File.ReadAllText(path);

            return null;
        }

        /// <summary>
        /// Конвертировать строку в кисть.
        /// @ - картинка.
        /// http - картинка с интернета.
        /// # - Hex цвет.
        /// red - Цвет по слову.
        /// </summary>
        /// <param name="color">Цвет</param>
        /// <returns>Кисть</returns>
        internal static Brush ConvertToBrush(string color)
        {
            try
            {
                if (File.Exists(color)
                    || (color[0] == 'h'
                     && color[1] == 't'
                     && color[2] == 't'
                     && color[3] == 'p')
                    || color.StartsWith("pack://application:"))
                    return new ImageBrush(new BitmapImage(new Uri(color, UriKind.RelativeOrAbsolute)));
                if (color[0] == '^')
                {
                    bool radial = color[1] == 'R';
                    string[] list = color.Remove(0, 2).Split('|');
                    double angle;
                    var start = 0;
                    if (double.TryParse(list[0], out angle)) start = 1;
                    var colors = new GradientStopCollection();
                    while (start < list.Length)
                    {
                        colors.Add(new GradientStop((Color)ColorConverter.ConvertFromString(color), start/list.Length));
                        start++;
                    }
                    return radial ? new RadialGradientBrush(colors) : new LinearGradientBrush(colors, angle);
                }
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
            catch
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("red"));
            }
        }

        /// <summary>
        /// Поставить в конце [/].
        /// </summary>
        /// <param name="value">path</param>
        /// <returns>endpath</returns>
        internal static string GetPath(string value)
        {
            if (!string.IsNullOrWhiteSpace(value)
            && !(value[^1] == '/' || value[^1] == '\\')) return value + '/';
            return value;
        }
    }
    /// <summary>
    /// Конвертер строки в цвет.
    /// </summary>
    public class ConvertToBrushConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string v) return Helper.ConvertToBrush(v);
            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush solid) return solid.Color.ToString();
            return null;
        }
    }
}
