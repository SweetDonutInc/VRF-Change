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
        List<OutletBlocks> outletBlocks_List = new List<OutletBlocks>(); //Лист с внешними блоками
        List<InletBlocks> inletBlocks_List = new List<InletBlocks>(); //Лист с внутренними блоками
        List<Splitters> Splitters_List = new List<Splitters>(); //Лист с разветвителями
        List<Tubes> Tubes_List = new List<Tubes>(); //Лист с трубами
        List<Colds> Colds_List = new List<Colds>(); //Лист с хладагентами

        bool isHisense = false;

        public MainWindow()
        {
            InitializeComponent();
            Logs.Text += "Логи\n";
        }

        private void LoadPdf_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openPdf = new OpenFileDialog();
            openPdf.Filter = "PDF files (*.pdf)|*.pdf";

            if (isHisense)
            {
                if (openPdf.ShowDialog() == true)
                {
                    outletBlocks_List.Clear();
                    inletBlocks_List.Clear();
                    Splitters_List.Clear();
                    Tubes_List.Clear();
                    Colds_List.Clear();

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

                    HisenseOutletBlock_Create(text);
                    HisenseInletBlock_Create(text);
                    HisenseSplitter_Create(text);
                    HisenseTube_Create(text);
                    HisenseCold_Create(text);


                    Logs.Text += "============================\n";
                    Logs.Text += "Считаны элементы: " + '\n';
                    foreach (var item in outletBlocks_List) Logs.Text += item.name + " || " + item.type + " || " + item.countType + '\n';
                    foreach (var item in inletBlocks_List) Logs.Text += item.name + " || " + item.type + " || " + item.countType + '\n';
                    foreach (var item in Splitters_List) Logs.Text += item.name + " || " + item.countType + '\n';
                    foreach (var item in Tubes_List) Logs.Text += item.name + " || " + item.countType + '\n';
                    foreach (var item in Colds_List) Logs.Text += item.name + " || " + item.countType + '\n';
                    Logs.Text += "============================\n";
                    LoadPdfBtnText.Visibility = Visibility.Visible;
                }
            }
        }

        //-----------------------------------------
        //Методы для Hisense
        //Заполняются листы в формате "Модель / Тип / Кол-во (шт/м)"
        //-----------------------------------------

        public void HisenseOutletBlock_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Replace("Hisense", "/").Split('\n');

            string outletB = "";
            bool check = false;

            foreach(var line in text)
            {

                if (line.Contains("Внутренний")) check = false;
                if (check) outletB += line + " ";
                if (line.Contains("Наружный")) check = true;
                if (line.Contains("HKF") || line.Contains("блок")) outletB += '\n';
            }

            string[] outletBlocks = outletB.Split('\n');

            foreach(var item in outletBlocks)
            {
                if(item != string.Empty)
                {
                    string[] temp = item.Split('/');
                    outletBlocks_List.Add(
                        new OutletBlocks
                        {
                            name = temp[3].Replace(" ", ""),
                            type = temp[2].Replace(" ", ""),
                            countType = temp[1].Replace(" ", "")
                        });
                }
            }
        }

        public void HisenseInletBlock_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Replace("Hisense", "/").Split('\n');

            string inletB = "";
            bool check = false;

            foreach (var line in text)
            {
                if (line.Contains("Разветвитель")) check = false;
                if (check) inletB += line + " ";
                if (line.Contains("Внутренний")) check = true;
                if (line.Contains("AVS") || line.Contains("AVD") || line.Contains("блок")) inletB += '\n';
            }

            string[] inletBlocks = inletB.Split('\n');
            foreach (var item in inletBlocks)
            {
                if (item != string.Empty)
                {
                    string[] temp = item.Split('/');
                    inletBlocks_List.Add(
                        new InletBlocks
                        {
                            name = temp[3].Replace(" ", ""),
                            type = temp[2].Replace(" ", ""),
                            countType = temp[1].Replace(" ", "")
                        });
                }
            }
        }

        public void HisenseSplitter_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Replace("Hisense", "").Split('\n');

            string splitterB = "";
            bool check = false;

            foreach (var line in text)
            {
                if (line.Contains("Перечень")) check = false;
                if (check) splitterB += line + " ";
                if (line.Contains("Разветвитель")) check = true;
                if (line.Contains("HFQ") || line.Contains("Разветвитель")) splitterB += '\n';
            }

            string[] splitters = splitterB.Split('\n');
            foreach (var item in splitters)
            {
                if (item != string.Empty)
                {
                    string[] temp = item.Split('/');
                    Splitters_List.Add(
                        new Splitters
                        {
                            name = temp[2].Replace(" ", ""),
                            countType = temp[1].Replace(" ", "")
                        });
                }
            }
        }

        public void HisenseTube_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Split('\n');

            string tubeB = "";
            bool check = false;

            foreach (var line in text)
            {
                if (line.Contains("Хладагент")) check = false;
                if (check) tubeB += line + " ";
                if (line.Contains("Медная труба")) check = true;
                if (line.Contains("?") || line.Contains("Медная труба")) tubeB += '\n';
            }

            string[] tubes = tubeB.Split('\n');
            foreach (var item in tubes)
            {
                if (item != string.Empty)
                {
                    string[] temp = item.Split('/');
                    Tubes_List.Add(
                        new Tubes
                        {
                            name = temp[2].Replace(" ", "").Remove(0, 1),
                            countType = temp[1].Replace(" ", "")
                        });
                }
            }
        }

        public void HisenseCold_Create(StringBuilder sb)
        {
            string[] text = sb.ToString().Split('\n');

            string coldB = "";
            bool check = false;

            foreach (var line in text)
            {
                if (line.Contains("Итого")) check = false;
                if (check) coldB += line + " ";
                if (line.Contains("Хладагент")) check = true;
                if (line.Contains("R410A") || line.Contains("Хладагент")) coldB += '\n';
            }

            string[] colds = coldB.Split('\n');
            foreach (var item in colds)
            {
                if (item != string.Empty)
                {
                    string[] temp = item.Split('/');
                    Colds_List.Add(
                        new Colds
                        {
                            name = temp[2].Replace(" ", ""),
                            countType = temp[1].Replace(" ", "")
                        });
                }
            }
        }

        //-----------------------------------------
        //Все листы Hisense заполнены
        //-----------------------------------------

        private void Hisense_Click(object sender, RoutedEventArgs e)
        {
            LoadPdfBtn.Visibility = Visibility.Visible;
            isHisense = true;
        }

        private void AllClear_Click(object sender, RoutedEventArgs e)
        {
            LoadPdfBtn.Visibility = Visibility.Collapsed;
            LoadPdfBtnText.Visibility = Visibility.Collapsed;
            outletBlocks_List.Clear();
            inletBlocks_List.Clear();
            Splitters_List.Clear();
            Tubes_List.Clear();
            Colds_List.Clear();
            Logs.Text = "Логи\n";

            isHisense = false;
        }
    }


    //-----------------------------------------
    //Классы для записи элементов Hisense
    //-----------------------------------------

    public class OutletBlocks
    {
        public string name { get; set; }
        public string type { get; set; }
        public string countType { get; set; }
    }

    public class InletBlocks
    {
        public string name { get; set; }
        public string type { get; set; }
        public string countType { get; set; }
    }

    public class Splitters
    {
        public string name { get; set; }
        public string countType { get; set; }
    }

    public class Tubes
    {
        public string name { get; set; }
        public string countType { get; set; }
    }

    public class Colds
    {
        public string name { get; set; }
        public string countType { get; set; }
    }
}
