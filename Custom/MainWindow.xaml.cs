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
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using System.Globalization;
using System.Drawing.Printing;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel;
using Spire.Xls;
using WW.Cad.Base;
using WW.Cad.Drawing;
using WW.Cad.IO;
using WW.Cad.Model;
using WW.Cad.Model.Objects;
using WW.Cad.Model.Tables;
using WW.Drawing;
using WW.Math;
using WW.Math.Geometry;


namespace Custom
{
    public partial class MainWindow : Window
    {
        List<OutletBlocks> outletBlocks_List = new List<OutletBlocks>(); //Лист с внешними блоками
        List<InletBlocks> inletBlocks_List = new List<InletBlocks>(); //Лист с внутренними блоками
        List<Splitters> Splitters_List = new List<Splitters>(); //Лист с разветвителями
        List<Tubes> Tubes_List = new List<Tubes>(); //Лист с трубами
        List<Colds> Colds_List = new List<Colds>(); //Лист с хладагентами

        List<OutBlocksData> OutB_Data = new List<OutBlocksData>();
        List<InBlocksData> InB_Data = new List<InBlocksData>();

        bool isHisense = false, isDantex = false, isClivet = false;

        public MainWindow()
        {
            InitializeComponent();
            Logs.Text += "Логи:\n\nКраткая инструкция:\n1. Выбираем производителя\n2. Заполняем блоки\n" +
                "3. Заполняем всю информацию для шапки Excel-файла\n4. Загружаем DWG-чертёж (Может занять несколько секунд)\n5. Выгружаем Excel-файл\n" +
                "6. Загружаем PDF чертежа\n7. Выгружаем итоговый PDF-файл\n8. Вы великолепны\n";
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
            Logs.Text = "Логи:\n\nКраткая инструкция:\n1. Выбираем производителя\n2. Заполняем блоки\n" +
                "3. Заполняем всю информацию для шапки Excel-файла\n4. Загружаем DWG-чертёж\n5. Выгружаем Excel-файл\n" +
                "6. Загружаем PDF чертежа\n7. Выгружаем итоговый PDF-файл\n8. Вы великолепны\n";

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
                        outletBlocks_List.Add(new OutletBlocks { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToInt32(TB_CountText.Text) });
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
                        inletBlocks_List.Add(new InletBlocks { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToInt32(TB_CountText.Text) });
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
                        Splitters_List.Add(new Splitters { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToDouble(TB_CountText.Text) });
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
                        Tubes_List.Add(new Tubes { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToDouble(TB_CountText.Text.Replace('.', ',')) });
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
                        Colds_List.Add(new Colds { name = CB_ModelType.SelectedItem.ToString(), count = Convert.ToDouble(TB_CountText.Text.Replace('.', ',')) });
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
        //Считывание с Excel-файлов моделей при выбранном типе производителя
        //==========================================
        private void OutletBlock_Selected(object sender, RoutedEventArgs e)
        {
            OutB_Data.Clear();
            CB_ModelType.ItemsSource = null;
            List<string> tempList = new List<string>();

            IWorkbook workbook;
            using (FileStream fileStream = new FileStream("Files/AllBlock.xlsx", FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream); // Считываем шаблонный файл
            }

            int sheetNum = 0;
            int rowCnt = 0; //задавать номер последней строки в таблице
            if (isHisense)
            {
                sheetNum = 0;
                rowCnt = 32;
            }
            if (isDantex)
            {
                sheetNum = 1;
                rowCnt = 71;
            }
            if (isClivet)
            {
                sheetNum = 2;
                rowCnt = 125;
            }

            ISheet BlockOut = workbook.GetSheetAt(sheetNum);
            for(int i = 3; i < rowCnt; i++)
            {
                IRow row = BlockOut.GetRow(i);
                if (row.GetCell(0).StringCellValue != "")
                {
                    OutB_Data.Add(
                        new OutBlocksData
                        {
                            realName = row.GetCell(0).StringCellValue,
                            AWName = row.GetCell(1).StringCellValue,
                            nominalCooling = row.GetCell(2).NumericCellValue.ToString(),
                            CoolingPower = row.GetCell(3).NumericCellValue.ToString(),
                            EER = row.GetCell(4).NumericCellValue.ToString(),
                            NominalHeating = row.GetCell(5).NumericCellValue.ToString(),
                            HeatingPower = row.GetCell(6).NumericCellValue.ToString(),
                            COP = row.GetCell(7).NumericCellValue.ToString(),
                            tubeDiameter = row.GetCell(10).StringCellValue,
                            RefrigerantType = row.GetCell(11).StringCellValue,
                            PowerSupply = row.GetCell(12).StringCellValue,
                            SoundLevel = row.GetCell(13).NumericCellValue.ToString(),
                            Size = row.GetCell(14).StringCellValue,
                            Weight = row.GetCell(15).NumericCellValue.ToString(),
                        });
                }
            }

            for(int i = 0; i < OutB_Data.Count; i++)
            {
                tempList.Add(OutB_Data[i].realName + " / " + OutB_Data[i].AWName);
            }

            CB_ModelType.ItemsSource = tempList;
        }

        private void InletBlock_Selected(object sender, RoutedEventArgs e)
        {
            InB_Data.Clear();
            CB_ModelType.ItemsSource = null;
            List<string> tempList = new List<string>();

            IWorkbook workbook;
            using (FileStream fileStream = new FileStream("Files/AllBlock.xlsx", FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream); // Считываем шаблонный файл
            }

            int sheetNum = 3;
            int rowCnt = 0; //задавать номер последней строки в таблице
            if (isHisense)
            {
                sheetNum = 3;
                rowCnt = 70;
            }
            if (isDantex)
            {
                sheetNum = 4;
                rowCnt = 104;
            }
            if (isClivet)
            {
                sheetNum = 5;
                rowCnt = 62;
            }

            ISheet BlockOut = workbook.GetSheetAt(sheetNum);
            for (int i = 3; i < rowCnt; i++)
            {
                IRow row = BlockOut.GetRow(i);
                if (row.GetCell(0).StringCellValue != "")
                {
                    InB_Data.Add(
                        new InBlocksData
                        {
                            realName = row.GetCell(0).StringCellValue,
                            AWName = row.GetCell(1).StringCellValue,
                            nominalCooling = row.GetCell(2).NumericCellValue.ToString(),
                            NominalHeating = row.GetCell(5).NumericCellValue.ToString(),
                            PowerConsumption = row.GetCell(8).NumericCellValue.ToString(),
                            CondensateTubeDiameter = row.GetCell(9).NumericCellValue.ToString(),
                            tubeDiameter = row.GetCell(10).StringCellValue,
                            PowerSupply = row.GetCell(12).StringCellValue,
                            SoundLevel = row.GetCell(13).NumericCellValue.ToString(),
                            Size = row.GetCell(14).StringCellValue,
                            Weight = row.GetCell(15).NumericCellValue.ToString(),
                            AirExchange = ""
                        });
                }
            }

            for (int i = 0; i < InB_Data.Count; i++)
            {
                tempList.Add(InB_Data[i].realName + " / " + InB_Data[i].AWName);
            }

            CB_ModelType.ItemsSource = tempList;
        }

        private void Splitters_Selected(object sender, RoutedEventArgs e)
        {
            CB_ModelType.ItemsSource = null;
            List<string> tempList = new List<string>();
            StreamReader f = new StreamReader("Files/Hisense/Splitters.txt");
            while (!f.EndOfStream)
            {
                string s = f.ReadLine();
                tempList.Add(s.Replace("\t", " "));
            }
            f.Close();
            CB_ModelType.ItemsSource = tempList;
        }

        private void Tubes_Selected(object sender, RoutedEventArgs e)
        {
            CB_ModelType.ItemsSource = null;
            List<string> tempList = new List<string>();
            StreamReader f = new StreamReader("Files/Hisense/Tubes.txt");
            while (!f.EndOfStream)
            {
                string s = f.ReadLine();
                tempList.Add(s.Replace("\t", " "));
            }
            f.Close();
            CB_ModelType.ItemsSource = tempList;
        }

        private void Colds_Selected(object sender, RoutedEventArgs e)
        {
            CB_ModelType.ItemsSource = null;
            List<string> tempList = new List<string>();
            StreamReader f = new StreamReader("Files/Hisense/XColds.txt");
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
            Hisense.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(8, 217, 214));
            Dantex.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 173, 181));
            Clivet.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 173, 181));
        }

        private void Dantex_Click(object sender, RoutedEventArgs e)
        {
            isHisense = false;
            isDantex = true;
            isClivet = false;
            Dantex.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(8, 217, 214));
            Hisense.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 173, 181));
            Clivet.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 173, 181));
        }

        private void Clivet_Click(object sender, RoutedEventArgs e)
        {
            isHisense = false;
            isDantex = false;
            isClivet = true;
            Clivet.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(8, 217, 214));
            Dantex.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 173, 181));
            Hisense.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 173, 181));
        }


        //==========================================
        //Написать название проекта на картинке
        //==========================================
        void projectName(string name)
        {
            var imagePath = System.IO.Path.GetFullPath(@"Files\Images\FirstList.jpg");
            var text = name;
            var resultPath = @"Files\TempFiles\newImage.jpg";

            var textColor = System.Windows.Media.Brushes.Black;

            var fontSize = 36;

            var dpi = 96;

            var font =
                new Typeface(
                    new System.Windows.Media.FontFamily("Segoe UI"), FontStyles.Normal,
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
                    new System.Windows.Point(195, 500));
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


        //==========================================
        //Создания Excel и PDF файлов
        //==========================================
        private void CreateExcel_Click(object sender, RoutedEventArgs e)
        {
            double nominalCooling = 0.0;
            double nominalHeating = 0.0;
            int outletCnt = 0;
            int inletCnt = 0;

            IWorkbook workbook;
            using (FileStream fileStream = new FileStream("Files/BlocksTemplate.xls", FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(fileStream); //Считываем шаблонный файл
            }

            //==========================================
            //Второй лист: перечисление всех блоков
            //==========================================

            ISheet sheet3 = workbook.GetSheetAt(3); //Лист с заголовком внешнего блока
            ISheet sheet4 = workbook.GetSheetAt(4); //Лист с внешними блоками

            ISheet sheet5 = workbook.GetSheetAt(5); //Лист с заголовком внешнего блока
            ISheet sheet6 = workbook.GetSheetAt(6); //Лист с внешними блоками

            ISheet sheet7 = workbook.GetSheetAt(7); //Лист с заголовком внешнего рефнетов
            ISheet sheet8 = workbook.GetSheetAt(8); //Лист с рефнетами

            ISheet sheet9 = workbook.GetSheetAt(9); //Лист с заголовком медных труб
            ISheet sheet10 = workbook.GetSheetAt(10); //Лист с медными трубами

            ISheet sheet11 = workbook.GetSheetAt(11); //Лист с хладагентом
            ISheet sheet12 = workbook.GetSheetAt(12); //Лист результата перечисления блоков

            ISheet sheet13 = workbook.GetSheetAt(13); //Лист с шапкой

            int newPos2List = 0;

            //Заполняем внешние блоки
            if (outletBlocks_List.Count > 0)
            {
                CopyRow(workbook, workbook, sheet12, sheet3, 0, 0);
                for (int j = 0; j < outletBlocks_List.Count; j++)
                {
                    if (j < outletBlocks_List.Count - 1)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            CopyRow(workbook, workbook, sheet12, sheet4, i + 3, i + 1 + 2 * j);
                            switch (i + 1)
                            {
                                case 2:
                                    string[] temp = outletBlocks_List[j].name.Split('/');
                                    sheet12.GetRow(i + 1 + 2 * j).GetCell(3).SetCellValue(temp[1].Replace(" ", ""));
                                    sheet12.GetRow(i + 1 + 2 * j).GetCell(16).SetCellValue(outletBlocks_List[j].count);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            CopyRow(workbook, workbook, sheet12, sheet4, i, i + 1 + 2 * j);
                            switch (i + 1)
                            {
                                case 2:
                                    string[] temp = outletBlocks_List[j].name.Split('/');
                                    sheet12.GetRow(i + 1 + 2 * j).GetCell(3).SetCellValue(temp[1].Replace(" ", ""));
                                    sheet12.GetRow(i + 1 + 2 * j).GetCell(16).SetCellValue(outletBlocks_List[j].count);
                                    break;
                            }
                        }
                    }
                }

                newPos2List = outletBlocks_List.Count * 2 + 2; //Запоминаем позицию окончания
            }

            //Заполняем внутренние блоки
            if (inletBlocks_List.Count > 0)
            {
                CopyRow(workbook, workbook, sheet12, sheet5, 0, newPos2List);
                for (int j = 0; j < inletBlocks_List.Count; j++)
                {
                    if (j < inletBlocks_List.Count - 1)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            CopyRow(workbook, workbook, sheet12, sheet6, i + 4, i + 1 + 3 * j + newPos2List);
                            switch (i + 1)
                            {
                                case 2:
                                    string[] temp = inletBlocks_List[j].name.Split('/');
                                    sheet12.GetRow(i + 1 + 3 * j + newPos2List).GetCell(3).SetCellValue(temp[1].Replace(" ", ""));
                                    sheet12.GetRow(i + 1 + 3 * j + newPos2List).GetCell(21).SetCellValue(inletBlocks_List[j].count);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            CopyRow(workbook, workbook, sheet12, sheet6, i, i + 1 + 3 * j + newPos2List);
                            switch (i + 1)
                            {
                                case 2:
                                    string[] temp = inletBlocks_List[j].name.Split('/');
                                    sheet12.GetRow(i + 1 + 3 * j + newPos2List).GetCell(3).SetCellValue(temp[1].Replace(" ", ""));
                                    sheet12.GetRow(i + 1 + 3 * j + newPos2List).GetCell(21).SetCellValue(inletBlocks_List[j].count);
                                    break;
                            }
                        }
                    }
                }

                newPos2List += inletBlocks_List.Count * 3 + 2; //Запоминаем позицию окончания
            }

            //Заполняем рефнеты/разветвители
            if (Splitters_List.Count > 0)
            {
                CopyRow(workbook, workbook, sheet12, sheet7, 0, newPos2List);
                for (int j = 0; j < Splitters_List.Count; j++)
                {
                    if (j < Splitters_List.Count - 1)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            CopyRow(workbook, workbook, sheet12, sheet8, i + 3, i + 1 + 2 * j + newPos2List);
                            if(i == 1) insertImage(workbook, sheet12, 1, i + 2 * j + newPos2List, "Files/Images/Splitter.png");
                            switch (i + 1)
                            {
                                case 2:
                                    string[] temp = Splitters_List[j].name.Split('/');
                                    sheet12.GetRow(i + 1 + 2 * j + newPos2List).GetCell(3).SetCellValue(temp[1].Replace(" ", ""));
                                    sheet12.GetRow(i + 1 + 2 * j + newPos2List).GetCell(16).SetCellValue(Splitters_List[j].count);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            CopyRow(workbook, workbook, sheet12, sheet8, i, i + 1 + 2 * j + newPos2List);
                            if (i == 1) insertImage(workbook, sheet12, 1, i + 2 * j + newPos2List, "Files/Images/Splitter.png");
                            switch (i + 1)
                            {
                                case 2:
                                    string[] temp = Splitters_List[j].name.Split('/');
                                    sheet12.GetRow(i + 1 + 2 * j + newPos2List).GetCell(3).SetCellValue(temp[1].Replace(" ", ""));
                                    sheet12.GetRow(i + 1 + 2 * j + newPos2List).GetCell(16).SetCellValue(Splitters_List[j].count);
                                    break;
                            }
                        }
                    }
                }

                newPos2List += Splitters_List.Count * 2 + 2; //Запоминаем позицию окончания
            }

            //Заполняем медные трубы
            if (Tubes_List.Count > 0)
            {
                CopyRow(workbook, workbook, sheet12, sheet9, 0, newPos2List);
                for (int j = 0; j < Tubes_List.Count; j++)
                {
                    if (j < Tubes_List.Count - 1)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            CopyRow(workbook, workbook, sheet12, sheet10, i + 3, i + 1 + 2 * j + newPos2List);
                            if(i == 1) insertImage(workbook, sheet12, 1, i + 2 * j + newPos2List, "Files/Images/Tube.png");
                            switch (i + 1)
                            {
                                case 2:
                                    sheet12.GetRow(i + 1 + 2 * j + newPos2List).GetCell(3).SetCellValue(Tubes_List[j].name);
                                    sheet12.GetRow(i + 1 + 2 * j + newPos2List).GetCell(16).SetCellValue(Tubes_List[j].count);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            CopyRow(workbook, workbook, sheet12, sheet10, i, i + 1 + 2 * j + newPos2List);
                            if (i == 1) insertImage(workbook, sheet12, 1, i + 2 * j + newPos2List, "Files/Images/Tube.png");
                            switch (i + 1)
                            {
                                case 2:
                                    sheet12.GetRow(i + 1 + 2 * j + newPos2List).GetCell(3).SetCellValue(Tubes_List[j].name);
                                    sheet12.GetRow(i + 1 + 2 * j + newPos2List).GetCell(16).SetCellValue(Tubes_List[j].count);
                                    break;
                            }
                        }
                    }
                }

                newPos2List += Tubes_List.Count * 2 + 2; //Запоминаем позицию окончания
            }

            //Заполняем хладагент
            if (Colds_List.Count > 0)
            {
                for (int j = 0; j < Colds_List.Count; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        CopyRow(workbook, workbook, sheet12, sheet11, i, i + 3 * j + newPos2List);
                        if (i == 1) insertImage(workbook, sheet12, 1, i + 2 * j + newPos2List, "Files/Images/Cold.png");
                        switch (i)
                        {
                            case 2:
                                sheet12.GetRow(i + 3 * j + newPos2List).GetCell(3).SetCellValue(Colds_List[j].name);
                                sheet12.GetRow(i + 3 * j + newPos2List).GetCell(16).SetCellValue(Colds_List[j].count);
                                break;
                        }
                    }
                }

                newPos2List += 4; //Запоминаем позицию окончания
            }

            sheet12.SetRowBreak(newPos2List - 1);

            //==========================================
            //Третий лист с характеристиками внешних и внутренних блоков
            //==========================================

            ISheet sheet = workbook.GetSheetAt(0); //Лист внешних блоков
            ISheet sheet1 = workbook.GetSheetAt(1); //Лист внутренних блоков
            ISheet sheet2 = workbook.GetSheetAt(2); //Лист с двумя типами блоков

            //Внешние блоки
            if (outletBlocks_List.Count > 0)
            {
                //Заполним новый лист путём сравнения
                List<OutBlocksData> tempOut = new List<OutBlocksData>();
                for(int n = 0; n < outletBlocks_List.Count; n++)
                {
                    for(int m = 0; m < OutB_Data.Count; m++)
                    {
                        string[] s = outletBlocks_List[n].name.Replace(" ", "").Split('/');
                        if (s[0].Equals(OutB_Data[m].realName)) tempOut.Add(OutB_Data[m]);
                    }
                }

                //Лист заполнен, дальше заполняем ячейки Excel-файла

                for (int j = 0; j < outletBlocks_List.Count; j++)
                {
                    outletCnt += outletBlocks_List[j].count;
                    nominalCooling += Convert.ToDouble(tempOut[j].nominalCooling);
                    nominalHeating += Convert.ToDouble(tempOut[j].NominalHeating);
                    for (int i = 0; i < 16; i++)
                    {
                        CopyRow(workbook, workbook, sheet2, sheet, i, i + 16 * j);
                        switch (i)
                        {
                            case 2:
                                sheet2.GetRow(i + 16 * j).GetCell(1).SetCellValue(tempOut[j].AWName);
                                sheet2.GetRow(i + 16 * j).GetCell(16).SetCellValue(outletBlocks_List[j].count);
                                break;
                            case 4:
                                sheet2.GetRow(i + 16 * j).GetCell(1).SetCellValue(tempOut[j].nominalCooling);
                                sheet2.GetRow(i + 16 * j).GetCell(16).SetCellValue(tempOut[j].NominalHeating);
                                break;
                            case 6:
                                sheet2.GetRow(i + 16 * j).GetCell(1).SetCellValue(tempOut[j].CoolingPower);
                                sheet2.GetRow(i + 16 * j).GetCell(16).SetCellValue(tempOut[j].HeatingPower);
                                break;
                            case 8:
                                sheet2.GetRow(i + 16 * j).GetCell(1).SetCellValue(tempOut[j].tubeDiameter);
                                sheet2.GetRow(i + 16 * j).GetCell(16).SetCellValue(tempOut[j].RefrigerantType);
                                break;
                            case 10:
                                sheet2.GetRow(i + 16 * j).GetCell(1).SetCellValue(tempOut[j].PowerSupply);
                                sheet2.GetRow(i + 16 * j).GetCell(16).SetCellValue(tempOut[j].SoundLevel);
                                break;
                            case 12:
                                sheet2.GetRow(i + 16 * j).GetCell(1).SetCellValue(tempOut[j].EER);
                                sheet2.GetRow(i + 16 * j).GetCell(16).SetCellValue(tempOut[j].COP);
                                break;
                            case 14:
                                sheet2.GetRow(i + 16 * j).GetCell(1).SetCellValue(tempOut[j].Size);
                                sheet2.GetRow(i + 16 * j).GetCell(16).SetCellValue(tempOut[j].Weight);
                                break;
                        }
                    }
                }
            }

            //Внутренние блоки

            if (inletBlocks_List.Count > 0)
            {
                //Заполним новый лист путём сравнения
                List<InBlocksData> tempIn = new List<InBlocksData>();
                for (int n = 0; n < inletBlocks_List.Count; n++)
                {
                    for (int m = 0; m < InB_Data.Count; m++)
                    {
                        string[] s = inletBlocks_List[n].name.Replace(" ", "").Split('/');
                        if (s[0].Equals(InB_Data[m].realName)) tempIn.Add(InB_Data[m]);
                    }
                }

                //Лист заполнен, дальше заполняем ячейки Excel-файла

                if (inletBlocks_List.Count > 0)
                {
                    for (int j = 0; j < inletBlocks_List.Count; j++)
                    {
                        inletCnt += inletBlocks_List[j].count;
                        for (int i = 0; i < 14; i++)
                        {
                            int newPos = i + 14 * j + 16 * outletBlocks_List.Count;
                            CopyRow(workbook, workbook, sheet2, sheet1, i, newPos);
                            switch (i)
                            {
                                case 2:
                                    sheet2.GetRow(newPos).GetCell(1).SetCellValue(tempIn[j].AWName);
                                    sheet2.GetRow(newPos).GetCell(16).SetCellValue(inletBlocks_List[j].count);
                                    break;
                                case 4:
                                    sheet2.GetRow(newPos).GetCell(1).SetCellValue(tempIn[j].nominalCooling);
                                    sheet2.GetRow(newPos).GetCell(16).SetCellValue(tempIn[j].NominalHeating);
                                    break;
                                case 6:
                                    sheet2.GetRow(newPos).GetCell(1).SetCellValue(tempIn[j].PowerConsumption);
                                    sheet2.GetRow(newPos).GetCell(16).SetCellValue(tempIn[j].SoundLevel);
                                    break;
                                case 8:
                                    sheet2.GetRow(newPos).GetCell(1).SetCellValue(tempIn[j].PowerSupply);
                                    sheet2.GetRow(newPos).GetCell(16).SetCellValue(tempIn[j].AirExchange);
                                    break;
                                case 10:
                                    sheet2.GetRow(newPos).GetCell(1).SetCellValue(tempIn[j].tubeDiameter);
                                    sheet2.GetRow(newPos).GetCell(16).SetCellValue(tempIn[j].CondensateTubeDiameter);
                                    break;
                                case 12:
                                    sheet2.GetRow(newPos).GetCell(1).SetCellValue(tempIn[j].Size);
                                    sheet2.GetRow(newPos).GetCell(16).SetCellValue(tempIn[j].Weight);
                                    break;
                            }
                        }
                    }
                }

            }

            //Заполняем шапку

            fillHeader(sheet13, nominalCooling, nominalHeating, outletCnt, inletCnt); 

            for (int i = 0; i < 18; i++)
            {
                CopyRow(workbook, workbook, sheet12, sheet13, i, newPos2List + i);
                if (i == 1) insertImage(workbook, sheet12, 0, newPos2List + i, "Files/Images/AW.png");
            }

            newPos2List += 18;

            for (int i = 0; i < outletBlocks_List.Count * 16 + inletBlocks_List.Count * 14; i++)
            {
                CopyRow(workbook, workbook, sheet12, sheet2, i, newPos2List + i);
            }

            //==========================================
            //==========================================
            //==========================================

            for (int i = 0; i < 12; i++)
            {
                workbook.RemoveSheetAt(0);
            }
            workbook.RemoveSheetAt(1);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Excel";
            saveFileDialog.Filter = "Excel files (*.xls)|*.xls";
            if (saveFileDialog.ShowDialog() == true)
            {
                excelPath = saveFileDialog.FileName;
                FileStream file = File.Create(excelPath);
                workbook.Write(file);
                file.Close();
            }

            Workbook wb = new Workbook();
            wb.LoadFromFile(excelPath);
            wb.SaveToFile("Files/TempFiles/tempExcel.pdf", Spire.Xls.FileFormat.PDF);

            CreateTitle();
            Logs.Text += "Excel-файл выгружен\n";
        }

        private void fillHeader(ISheet sheet, double cool, double heat, int outletCnt, int inletCnt)
        {
            sheet.GetRow(0).GetCell(12).SetCellValue(OrderNum.Text); // Номер предложения
            sheet.GetRow(0).GetCell(17).SetCellValue(Date.Text); // Дата
            sheet.GetRow(1).GetCell(12).SetCellValue(PjNum.Text); // Номер проекта
            sheet.GetRow(1).GetCell(18).SetCellValue(Worker.Text); // Руководитель проекта
            sheet.GetRow(2).GetCell(16).SetCellValue(SystemName.Text); // Система
            sheet.GetRow(3).GetCell(16).SetCellValue(outletCnt); // Кол-во наружних блоков
            sheet.GetRow(4).GetCell(16).SetCellValue(inletCnt); // Кол-во внутренних блоков
            sheet.GetRow(5).GetCell(12).SetCellValue(Name); // Объект
            sheet.GetRow(7).GetCell(11).SetCellValue(cool); // Номинальная холодопроизводительность
            sheet.GetRow(7).GetCell(27).SetCellValue(heat); // Номинальная теплопроизводительность
        }

        public string excelPath { get; set; }

        public void CopyRow(IWorkbook destWorkbook, IWorkbook sourceWorkbook, ISheet newWorksheet, ISheet oldWorksheet, int sourceRowNum, int destinationRowNum)
        {
            IRow newRow = newWorksheet.GetRow(destinationRowNum);
            IRow sourceRow = oldWorksheet.GetRow(sourceRowNum);

            if (sourceRow != null)
            {
                if (newRow == null)
                {
                    newRow = newWorksheet.CreateRow(destinationRowNum);
                }
                newRow.Height = sourceRow.Height;

                // Loop through source columns to add to new row
                for (int i = 0; i < sourceRow.LastCellNum; i++)
                {
                    // Grab a copy of the old/new cell
                    HSSFCell oldCell = (HSSFCell)sourceRow.GetCell(i);
                    HSSFCell newCell = (HSSFCell)newRow.CreateCell(i);

                    // If the old cell is null jump to next cell
                    if (oldCell == null)
                    {
                        continue;
                    }

                    HSSFCellStyle origCellStyle = (HSSFCellStyle)sourceWorkbook.GetCellStyleAt(oldCell.CellStyle.Index);
                    //// Copy style from old cell and apply to new cell
                    HSSFCellStyle newCellStyle; // = (HSSFCellStyle)destWorkbook.CreateCellStyle();
                    //newCellStyle.CloneStyleFrom(origCellStyle);
                    newCellStyle = origCellStyle;
                    newCell.CellStyle = newCellStyle;

                    // If there is a cell comment, copy
                    if (oldCell.CellComment != null)
                        newCell.CellComment = oldCell.CellComment;

                    // If there is a cell hyperlink, copy
                    if (oldCell.Hyperlink != null)
                        newCell.Hyperlink = oldCell.Hyperlink;

                    //// Set the cell data type
                    newCell.SetCellType(oldCell.CellType);

                    if (sourceRow.IsFormatted)
                    {
                        newRow.RowStyle = sourceRow.RowStyle;
                    }

                    // Set the cell data value
                    switch (oldCell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Blank:
                            newCell.SetCellValue(oldCell.StringCellValue);
                            break;
                        case NPOI.SS.UserModel.CellType.Boolean:
                            newCell.SetCellValue(oldCell.BooleanCellValue);
                            break;
                        case NPOI.SS.UserModel.CellType.Error:
                            newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                            break;
                        case NPOI.SS.UserModel.CellType.Formula:
                            newCell.SetCellFormula(oldCell.CellFormula);
                            break;
                        case NPOI.SS.UserModel.CellType.Numeric:
                            newCell.SetCellValue(oldCell.NumericCellValue);
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                            newCell.SetCellValue(oldCell.RichStringCellValue);
                            break;
                        case NPOI.SS.UserModel.CellType.Unknown:
                            newCell.SetCellValue(oldCell.StringCellValue);
                            break;
                    }
                }

                for (int i = 0; i < oldWorksheet.NumMergedRegions; i++)
                {
                    CellRangeAddress cellRangeAddress = oldWorksheet.GetMergedRegion(i);
                    if (cellRangeAddress.FirstRow == sourceRow.RowNum)
                    {
                        CellRangeAddress newCellRangeAddress = new CellRangeAddress(destinationRowNum,
                            destinationRowNum + cellRangeAddress.LastRow - cellRangeAddress.FirstRow,
                            cellRangeAddress.FirstColumn,
                            cellRangeAddress.LastColumn);

                        newWorksheet.AddMergedRegion(newCellRangeAddress);
                    }
                }
            }
        }

        //==========================================
        //Загружаем DWG, заменяем названия блоков на наши 
        //==========================================

        private void UploadDwg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DWG files (*.dwg)|*.dwg";
            if (openFileDialog.ShowDialog() == true)
            {
                //Считываем загруженный DWG временно превращая его в DXF
                DwgReader dwgReader = new DwgReader(openFileDialog.FileName);
                DxfModel model = dwgReader.Read();
                DxfWriter.Write("Files/TempFiles/tempDXF.dxf", model);
                string s;
                string text = "";
                using (var f = new StreamReader("Files/TempFiles/tempDXF.dxf", Encoding.GetEncoding(1251)))
                {
                    while ((s = f.ReadLine()) != null)
                    {
                        if (s.Contains("1021"))
                            text += s.Replace("1021", "1032") +"\n";
                        else
                        {
                            text += checkContains(s) + "\n";
                        }
                    }
                }

                string path = "";

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save DWG";
                saveFileDialog.Filter = "DWG files (*.dwg)|*.dwg";
                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                }

                StreamWriter sw = new StreamWriter(path, true, Encoding.GetEncoding(1251));
                sw.Write(text);
                sw.Close();
            }
        }

        public string checkContains(string str)
        {
            string newStr = "";
            if(outletBlocks_List.Count > 0)
            {
                for(int i = 0; i < outletBlocks_List.Count; i++)
                {
                    string[] s = outletBlocks_List[i].name.Replace(" ", "").Split('/');
                    if (str.Contains(s[0]))
                    {
                        newStr = str.Replace(s[0], s[1]);
                        return newStr;
                    }

                    else newStr = str;
                }
            }
            if (inletBlocks_List.Count > 0)
            {
                for (int i = 0; i < inletBlocks_List.Count; i++)
                {
                    string[] s = inletBlocks_List[i].name.Replace(" ", "").Split('/');
                    if (str.Contains(s[0]))
                    {
                        newStr = str.Replace(s[0], s[1]);
                        return newStr;
                    }
                    else newStr = str;
                }
            }
            return newStr;
        }

        //==========================================
        //Загружаем PDF с чертежём с картинками 
        //==========================================

        public string dwgPDFpath { get; set; }

        private void UploadPDF_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                dwgPDFpath = openFileDialog.FileName;
                Logs.Text += "PDF-файл чертежа считан\n";
            }
        }

        //==========================================
        //Объединяем PDF титульник + Excel + чертёж 
        //==========================================

        private void CollectPDF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save PDF";
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Merge(new[] { "Files/TempFiles/out.pdf", "Files/TempFiles/tempExcel.pdf", dwgPDFpath }, stream);
                }
                Logs.Text += $"PDF файл выгружен и доступен по пути {saveFileDialog.FileName}\n";
            }

        }

        public static void Merge(IEnumerable<string> fileNames, Stream target)

        {
            using (var document = new Document())
            using (var pdf = new PdfCopy(document, target))
            {
                document.Open();
                foreach (string file in fileNames)
                {
                    using (var reader = new PdfReader(file))
                    {
                        pdf.AddDocument(reader);
                    }
                }
            }
        }

        //==========================================
        //Создание титульника и его запись в PDF
        //==========================================

        private void CreateTitle()
        {
            projectName(projectNameTxt.Text);

            string pdfpath = @"Files\TempFiles\out.pdf";
            string imagepath =  @"Files\TempFiles\newImage.jpg";

            Document doc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);

            try
            {
                PdfWriter.GetInstance(doc, new FileStream(pdfpath, FileMode.Create));
                doc.Open();
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagepath);
                image.ScalePercent(48f);
                doc.Add(image);
            }
            catch (Exception ex)
            {
                //Log error;
            }
            finally
            {
                doc.Close();
            }


        }

        //==========================================
        //Добавляем картинки внешних/внутренних блоков,
        //рефнетов, трубок, хладагента в самый нижний лист
        //==========================================
        void insertImage(IWorkbook workbook, ISheet sheet, int col, int row, string path)
        {
            byte[] data = File.ReadAllBytes(path);
            int pictureindex = workbook.AddPicture(data, PictureType.PNG);
            ICreationHelper helper = workbook.GetCreationHelper();
            IDrawing drawing = sheet.CreateDrawingPatriarch();
            IClientAnchor anchor = helper.CreateClientAnchor();
            anchor.Col1 = col;//0 index based column
            anchor.Row1 = row;//0 index based row
            IPicture picture = drawing.CreatePicture(anchor, pictureindex);
            picture.Resize();
        }
    }


    //==========================================-
    //Классы для записи элементов
    //==========================================

    public class OutletBlocks
    {
        public string name { get; set; }
        public int count { get; set; }
    }

    public class InletBlocks
    {
        public string name { get; set; }
        public int count { get; set; }
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

    public class OutBlocksData
    {
        public string realName { get; set; }
        public string AWName { get; set; }
        public string nominalCooling { get; set; }
        public string CoolingPower { get; set; }
        public string EER { get; set; }
        public string NominalHeating { get; set; }
        public string HeatingPower { get; set; }
        public string COP { get; set; }
        public string tubeDiameter { get; set; }
        public string RefrigerantType { get; set; }
        public string PowerSupply { get; set; }
        public string SoundLevel { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
    }

    public class InBlocksData
    {
        public string realName { get; set; }
        public string AWName { get; set; }
        public string nominalCooling { get; set; }
        public string NominalHeating { get; set; }
        public string PowerConsumption { get; set; }
        public string CondensateTubeDiameter { get; set; }
        public string tubeDiameter { get; set; }
        public string PowerSupply { get; set; }
        public string AirExchange { get; set; }
        public string SoundLevel { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
    }
}