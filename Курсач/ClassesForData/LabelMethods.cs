using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace Курсач.ClassesForData
{
    public class LabelMethods
    {
        public static Binding FormateBinding(double parameter)
        {
            Binding binding = new Binding("ActualWidth");
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                typeof(Page), 1);
            binding.Converter = new WidthToFontSizeConverter();
            binding.ConverterParameter = parameter;
            return binding;
        }

        public static TextBlock FormateTextBlock(string text, double fontSize)
        {
            TextBlock textBlock = new TextBlock()
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
            };
            BindingOperations.SetBinding(textBlock, TextBlock.FontSizeProperty,
                FormateBinding(fontSize));

            return textBlock;
        }
    }
}
