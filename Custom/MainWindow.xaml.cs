using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Data;
using System.Globalization;

namespace Custom
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<OutletBlocks> outletBlocks_List = new List<OutletBlocks>(); //Лист с внешними блоками
        List<InletBlocks> inletBlocks_List = new List<InletBlocks>(); //Лист с внутренними блоками
        List<Splitters> Splitters_List = new List<Splitters>(); //Лист с разветвителями
        List<Tubes> Tubes_List = new List<Tubes>(); //Лист с трубами
        List<Colds> Colds_List = new List<Colds>(); //Лист с хладагентами

        bool isHisense = false, isDantex = false, isClivet = false;

        public MainWindow()
        {
            InitializeComponent();
            Logs.Text += "Логи\n";
            projectName();
        }

        //==========================================
        //Кнопка для очистки всего
        //==========================================

        private void AllClear_Click(object sender, RoutedEventArgs e)
        {
            outletBlocks_List.Clear();
            inletBlocks_List.Clear();
            Splitters_List.Clear();
            Tubes_List.Clear();
            Colds_List.Clear();
            Logs.Text = "Логи\n";

            isHisense = false;
        }

        //==========================================
        //==========================================
        //==========================================

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_BlockTypes.SelectedIndex)
            {
                case 0:
                    if (CB_ModelType.SelectedItem != null && TB_CountText.Text != "")
                    {
                        outletBlocks_List.Add(new OutletBlocks { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToDouble(TB_CountText.Text) });
                        Logs.Text += $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] Добавлен новый элемент:\nВнешний блок\n" +
                            outletBlocks_List[outletBlocks_List.Count - 1].name + " || " +
                            outletBlocks_List[outletBlocks_List.Count - 1].count.ToString() + " шт.\n";
                        CB_ModelType.SelectedItem = null;
                        TB_CountText.Text = "";
                    }
                    break;
                case 1:
                    if (CB_ModelType.SelectedItem != null && TB_CountText.Text != "")
                    {
                        inletBlocks_List.Add(new InletBlocks { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToDouble(TB_CountText.Text)});
                        Logs.Text += $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] Добавлен новый элемент:\nВнутренний блок\n" +
                            inletBlocks_List[inletBlocks_List.Count - 1].name + " || " +
                            inletBlocks_List[inletBlocks_List.Count - 1].count.ToString() + " шт.\n";
                        CB_ModelType.SelectedItem = null;
                        TB_CountText.Text = "";
                    }
                    break;
                case 2:
                    if (CB_ModelType.SelectedItem != null && TB_CountText.Text != "")
                    {
                        Splitters_List.Add(new Splitters { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToDouble(TB_CountText.Text)});
                        Logs.Text += $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] Добавлен новый элемент:\nРазветвитель\n" +
                            Splitters_List[Splitters_List.Count - 1].name + " || " +
                            Splitters_List[Splitters_List.Count - 1].count.ToString() + " шт.\n";
                        CB_ModelType.SelectedItem = null;
                        TB_CountText.Text = "";
                    }
                    break;
                case 3:
                    if (CB_ModelType.SelectedItem != null && TB_CountText.Text != "")
                    {
                        Tubes_List.Add(new Tubes { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToDouble(TB_CountText.Text)});
                        Logs.Text += $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] Добавлен новый элемент:\nМедная труба\n" +
                            Tubes_List[Tubes_List.Count - 1].name + " || " +
                            Tubes_List[Tubes_List.Count - 1].count.ToString() + " м\n";
                        CB_ModelType.SelectedItem = null;
                        TB_CountText.Text = "";
                    }
                    break;
                case 4:
                    if (CB_ModelType.SelectedItem != null && TB_CountText.Text != "")
                    {
                        Colds_List.Add(new Colds { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToDouble(TB_CountText.Text)});
                        Logs.Text += $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] Добавлен новый элемент:\nХладагент\n" +
                            Colds_List[Colds_List.Count - 1].name + " || " +
                            Colds_List[Colds_List.Count - 1].count.ToString() + " кг.\n";
                        CB_ModelType.SelectedItem = null;
                        TB_CountText.Text = "";
                    }
                    break;
            }

        }

        //==========================================
        //Считывание с блокнотиков моделей при выбранном типе объекта
        //==========================================
        private void OutletBlock_Selected(object sender, RoutedEventArgs e)
        {
            CB_ModelType.ItemsSource = null;
            List<string> tempList = new List<string>();
            StreamReader f = new StreamReader("Files/OutletBlocks.txt");
            while (!f.EndOfStream)
            {
                string s = f.ReadLine();
                tempList.Add(s.Replace("\t", " "));
            }
            f.Close();
            CB_ModelType.ItemsSource = tempList;
        }

        private void InletBlock_Selected(object sender, RoutedEventArgs e)
        {
            CB_ModelType.ItemsSource = null;
            List<string> tempList = new List<string>();
            StreamReader f = new StreamReader("Files/InletBlocks.txt");
            while (!f.EndOfStream)
            {
                string s = f.ReadLine();
                tempList.Add(s.Replace("\t", " "));
            }
            f.Close();
            CB_ModelType.ItemsSource = tempList;
        }

        //==========================================
        //==========================================
        //==========================================


        //==========================================
        //Поиск модели в ComboBox
        //==========================================
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)e.OriginalSource;
            if (tb.SelectionStart != 0)
            {
                CB_ModelType.SelectedItem = null; // Если набирается текст сбросить выбраный элемент
            }
            if (tb.SelectionStart == 0 && CB_ModelType.SelectedItem == null)
            {
                CB_ModelType.IsDropDownOpen = false; // Если сбросили текст и элемент не выбран, сбросить фокус выпадающего списка
            }

            CB_ModelType.IsDropDownOpen = true;
            if (CB_ModelType.SelectedItem == null)
            {
                // Если элемент не выбран менять фильтр
                CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(CB_ModelType.ItemsSource);
                cv.Filter = s => ((string)s).IndexOf(CB_ModelType.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
            }
        }

        private void Hisense_Click(object sender, RoutedEventArgs e)
        {
            isHisense = true;
            isDantex = false;
            isClivet = false;
            Hisense.Background = new SolidColorBrush(Color.FromRgb(8, 217, 214));
            Dantex.Background = new SolidColorBrush(Color.FromRgb(0, 173, 181));
            Clivet.Background = new SolidColorBrush(Color.FromRgb(0, 173, 181));
        }

        private void Dantex_Click(object sender, RoutedEventArgs e)
        {
            isHisense = false;
            isDantex = true;
            isClivet = false;
            Dantex.Background = new SolidColorBrush(Color.FromRgb(8, 217, 214));
            Hisense.Background = new SolidColorBrush(Color.FromRgb(0, 173, 181));
            Clivet.Background = new SolidColorBrush(Color.FromRgb(0, 173, 181));
        }

        private void Clivet_Click(object sender, RoutedEventArgs e)
        {
            isHisense = false;
            isDantex = false;
            isClivet = true;
            Clivet.Background = new SolidColorBrush(Color.FromRgb(8, 217, 214));
            Dantex.Background = new SolidColorBrush(Color.FromRgb(0, 173, 181));
            Hisense.Background = new SolidColorBrush(Color.FromRgb(0, 173, 181));
        }


        //==========================================
        //Написать название проекта на картинке
        //==========================================
        void projectName()
        {
            var imagePath = @"C:\Users\user\Desktop\FirstList.jpg";
            var text = "Здесь будет наименование проекта";
            var resultPath = @"C:\Users\user\Desktop\FirstList2222.jpg";
            
            var textColor = Brushes.Red;

            var fontSize = 40;

            var dpi = 96;

            var font =
                new Typeface(
                    new FontFamily("Segoe UI"), FontStyles.Normal,
                    FontWeights.Bold, FontStretches.SemiExpanded);

            var image = BitmapFrame.Create(new Uri("file://" + imagePath));
            var imageWidth = (double)image.PixelWidth;
            var imageHeight = (double)image.PixelHeight;

            var formattedText =
                new FormattedText(
                    text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    font, fontSize, textColor)
                {
                    MaxTextWidth = imageWidth,
                    TextAlignment = TextAlignment.Left,
                };

            var textWidth = formattedText.Width;
            var textHeight = formattedText.Height;
            
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {

                drawingContext.DrawImage(
                    image,
                    new Rect(0, 0, imageWidth, imageHeight));
                drawingContext.DrawText(
                    formattedText,
                    new Point(195, 510));
            }

            var bmp =
                new RenderTargetBitmap(
                    (int)imageWidth, (int)imageHeight, dpi, dpi,
                    PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (var stream = File.Create(resultPath))
                encoder.Save(stream);
        }
        //==========================================
        //==========================================
        //==========================================
    }


    //==========================================-
    //Классы для записи элементов
    //==========================================

    public class OutletBlocks
    {
        public string name { get; set; }
        public double count { get; set; }
    }

    public class InletBlocks
    {
        public string name { get; set; }
        public double count { get; set; }
    }

    public class Splitters
    {
        public string name { get; set; }
        public double count { get; set; }
    }

    public class Tubes
    {
        public string name { get; set; }
        public double count { get; set; }
    }

    public class Colds
    {
        public string name { get; set; }
        public double count { get; set; }
    }
}
