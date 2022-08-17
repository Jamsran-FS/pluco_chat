using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChatApp.Classes
{
    public class Functions
    {
        public static readonly HttpClient HttpClient = new HttpClient();
        public static readonly string ServerURL = "http://18.237.253.232/";
        public static List<SolidColorBrush> ColorList { get; set; } = new List<SolidColorBrush>();

        public static string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text.TrimEnd(new char[] { '\n', '\r' });
        }

        public static void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static object FindItemControl(ItemsControl itemsControl, string controlName, object item)
        {
            ContentPresenter container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
            container.ApplyTemplate();
            return container.ContentTemplate.FindName(controlName, container);
        }

        public static void GenerateColorList()
        {
            ColorList.Add(new SolidColorBrush(Color.FromRgb(112, 81, 167)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(58, 150, 221)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(238, 109, 240)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(140, 140, 140)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(253, 100, 111)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(43, 172, 118)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(255, 189, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(224, 30, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(0, 152, 168)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(67, 67, 67)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(112, 81, 167)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(58, 150, 221)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(238, 109, 240)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(140, 140, 140)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(253, 100, 111)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(43, 172, 118)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(255, 189, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(224, 30, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(0, 152, 168)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(67, 67, 67)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(112, 81, 167)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(58, 150, 221)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(238, 109, 240)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(140, 140, 140)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(253, 100, 111)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(43, 172, 118)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(255, 189, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(224, 30, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(0, 152, 168)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(67, 67, 67)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(112, 81, 167)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(58, 150, 221)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(238, 109, 240)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(140, 140, 140)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(253, 100, 111)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(43, 172, 118)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(255, 189, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(224, 30, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(0, 152, 168)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(67, 67, 67)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(112, 81, 167)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(58, 150, 221)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(238, 109, 240)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(140, 140, 140)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(253, 100, 111)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(43, 172, 118)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(255, 189, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(224, 30, 90)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(0, 152, 168)));
            ColorList.Add(new SolidColorBrush(Color.FromRgb(67, 67, 67)));
        }
    }
}
