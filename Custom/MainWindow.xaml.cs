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

namespace Custom
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadPdf_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openPdf = new OpenFileDialog();
            openPdf.Filter = "PDF files (*.pdf)|*.pdf";


            if (openPdf.ShowDialog() == true)
            {
                PdfReader reader = new PdfReader(openPdf.FileName);
                StringBuilder text = new StringBuilder();
                bool check = false;
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    if (currentText.Contains("Наружный блок")) check = true;
                    if (currentText.Contains("Информация о системе")) check = false;
                    if (check) text.Append(currentText);
                }
                reader.Close();

                test.Text = OutletBlock_Create(text);
                test.Text += InletBlock_Create(text);
                test.Text += Splitter_Create(text);
                test.Text += Tube_Create(text);
                test.Text += Cold_Create(text);
                //test.Text = text.ToString();
            }
        }

        //-----------------------------------------
        //Функции для создания строк
        //Возвращается строка в формате "/ кол-во / шт(или м) / Тип / Модель"
        public string OutletBlock_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Replace("Hisense", "/").Split('\n');

            string outletB = "";
            bool check = false;

            foreach(var line in text)
            {
                if (line.Contains("Наружный")) check = true;
                if (line.Contains("Внутренний")) check = false;
                if (check) outletB += line + " ";
                if (line.Contains("KFSX") || line.Contains("блок")) outletB += '\n';
            }

            return outletB;
        }

        public string InletBlock_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Replace("Hisense", "/").Split('\n');

            string inletB = "";
            bool check = false;

            foreach (var line in text)
            {
                if (line.Contains("Внутренний")) check = true;
                if (line.Contains("Разветвитель")) check = false;
                if (check) inletB += line + " ";
                if (line.Contains("AVS") || line.Contains("AVD") || line.Contains("блок")) inletB += '\n';
            }

            return inletB;
        }

        public string Splitter_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Replace("Hisense", "").Split('\n');

            string splitterB = "";
            bool check = false;

            foreach (var line in text)
            {
                if (line.Contains("Разветвитель")) check = true;
                if (line.Contains("Перечень")) check = false;
                if (check) splitterB += line + " ";
                if (line.Contains("HFQ") || line.Contains("Разветвитель")) splitterB += '\n';
            }

            return splitterB;
        }

        public string Tube_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Split('\n');

            string tubeB = "";
            bool check = false;

            foreach (var line in text)
            {
                if (line.Contains("Медная труба")) check = true;
                if (line.Contains("Хладагент")) check = false;
                if (check) tubeB += line + " ";
                if (line.Contains("?") || line.Contains("Медная труба")) tubeB += '\n';
            }

            return tubeB;
        }

        public string Cold_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Split('\n');

            string coldB = "";
            bool check = false;

            foreach (var line in text)
            {
                if (line.Contains("Хладагент")) check = true;
                if (line.Contains("Итого")) check = false;
                if (check) coldB += line + " ";
                if (line.Contains("R410A") || line.Contains("Хладагент")) coldB += '\n';
            }

            return coldB;
        }
    }
}
