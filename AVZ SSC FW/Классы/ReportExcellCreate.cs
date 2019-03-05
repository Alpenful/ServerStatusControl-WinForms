using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace AVZ_SSC_FW
{
    class ReportExcellCreate
    {
        public void createReportExcell(SettingApplication setting, List<LogEvent> logs, string nameReport)
        {
            // Создаём экземпляр нашего приложения
            Excel.Application excelApp = new Excel.Application();
            // Задаем количество листов
            excelApp.SheetsInNewWorkbook = 1;
            // Создаём экземпляр рабочий книги Excel
            Excel.Workbook workBook;
            // Создаём экземпляр листа Excel
            Excel.Worksheet workSheet;

            workBook = excelApp.Workbooks.Add();
            workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);

            //Excel.Range rng1 = workSheet.Range["A1", $"Y25"];
            //rng1.EntireColumn.AutoFit();

            //// Заполняем первую строку числами от 1 до 10
            //for (int j = 1; j <= 10; j++)
            //{
            workSheet.Cells[1, 1] = "Средний пинг по    " + logs[0].host;
                workSheet.Cells[2, 1] = "Дни / Часы     ";
                workSheet.Cells[2, 2] = "00:00 - 00:59"; workSheet.Cells[2, 3] = "01:00 - 01:59"; workSheet.Cells[2, 4] = "02:00 - 02:59"; workSheet.Cells[2, 5] = "03:00 - 03:59"; workSheet.Cells[2, 6] = "04:00 - 04:59"; workSheet.Cells[2, 7] = "05:00 - 05:59";
                workSheet.Cells[2, 8] = "06:00 - 06:59"; workSheet.Cells[2, 9] = "07:00 - 07:59"; workSheet.Cells[2, 10] = "08:00 - 08:59"; workSheet.Cells[2, 11] = "09:00 - 09:59"; workSheet.Cells[2, 12] = "10:00 - 10:59"; workSheet.Cells[2, 13] = "11:00 - 11:59";
                workSheet.Cells[2, 14] = "12:00 - 12:59"; workSheet.Cells[2, 15] = "13:00 - 13:59"; workSheet.Cells[2, 16] = "14:00 - 14:59"; workSheet.Cells[2, 17] = "15:00 - 15:59"; workSheet.Cells[2, 18] = "16:00 - 16:59"; workSheet.Cells[2, 19] = "17:00 - 17:59";
                workSheet.Cells[2, 20] = "18:00 - 18:59"; workSheet.Cells[2, 21] = "19:00 - 19:59"; workSheet.Cells[2, 22] = "20:00 - 20:59"; workSheet.Cells[2, 23] = "21:00 - 21:59"; workSheet.Cells[2, 24] = "22:00 - 22:59"; workSheet.Cells[2, 25] = "23:00 - 23:59";

            Excel.Range rng2 = workSheet.Range["B2", "Y2"];
            rng2.Font.Size = 9;
            rng2.ColumnWidth = 9;
            rng2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            (workSheet.Cells[2, 1] as Excel.Range).ColumnWidth = 11;

            int days = (logs[logs.Count - 1].date - logs[0].date).Days;
            int currentDay = logs[0].date.Day;
            int hourStep = 0;
            int countPing = 0;
            int summPing = 0;
            //DateTime startLogReport = logs[0].date;
            //DateTime hourPeriodStart = new DateTime(startLogReport.Year, startLogReport.Month, startLogReport.Day + hourStep, 0, 0, 0); 
            //DateTime hourPeriodEnd = new DateTime(startLogReport.Year, startLogReport.Month, startLogReport.Day + hourStep, 0, 59, 59);
           

            Excel.Range rng = workSheet.Range["A3", $"Y{days + 3}"];
            //rng.EntireColumn.AutoFit();
            rng.Cells.NumberFormat = "@";

            int startX = 2;     int stepX = 0;
            int startY = 3;     int stepY = 0;

            bool NotNull = false;
            string DayName = "";


            // Разбиваем наш массив и фаршируем таблицу
            for (int i = 0; i < logs.Count; i++)
            {
                if (logs[i].date.Day == currentDay)
                {
                    NotNull = true;
                    string day = "";
                    string month = "";

                    if (logs[i].date.Day < 10) day = "0" + currentDay;
                    else day = logs[i].date.Day.ToString();

                    if (logs[i].date.Month < 10) month = "0" + logs[i].date.Month;
                    else month = logs[i].date.Month.ToString();

                    DayName = day + "." + month + "." + logs[i].date.Year;

                    //Работа с часами
                    if (logs[i].date.Hour == hourStep)
                    {
                        if (logs[i].hostEvent != "Хост недоступен")
                        {
                            string[] ev = logs[i].hostEvent.Split(',');
                            string[] vs = ev[1].Split('=');
                            summPing += int.Parse(vs[1].Trim());
                            countPing++;
                        }
                        else
                        {
                            workSheet.Cells[startY + stepY, startX + stepX] = "Offline";
                            (workSheet.Cells[startY + stepY, startX + stepX] as Excel.Range).Interior.Color = Color.OrangeRed;

                            if (hourStep < 24)
                            {
                                hourStep++;
                                stepX++;
                            }
                            else
                            {
                                hourStep = 0;
                                stepX = 0;
                            }
                            countPing = 0;
                            summPing = 0;
                        }
                    }
                    else
                    {
                        if (countPing != 0)
                        {
                            if (hourStep != 23)
                            {
                                int averPing = summPing / countPing;
                                workSheet.Cells[startY + stepY, startX + stepX] = averPing + " ms";
                                if (averPing <= 5) { (workSheet.Cells[startY + stepY, startX + stepX] as Excel.Range).Interior.Color = Color.PaleGreen; }
                                if (averPing > 5 && averPing <= 20) { (workSheet.Cells[startY + stepY, startX + stepX] as Excel.Range).Interior.Color = Color.GreenYellow; }
                                if (averPing > 20 && averPing <= 100) { (workSheet.Cells[startY + stepY, startX + stepX] as Excel.Range).Interior.Color = Color.Yellow; }
                                if (averPing > 100) { (workSheet.Cells[startY + stepY, startX + stepX] as Excel.Range).Interior.Color = Color.Goldenrod; }
                            }
                            else
                            {
                                int averPing = summPing / countPing;
                                workSheet.Cells[startY + stepY -1, startX + stepX] = averPing + " ms";
                                if (averPing <= 5) { (workSheet.Cells[startY -1 + stepY, startX + stepX] as Excel.Range).Interior.Color = Color.PaleGreen; }
                                if (averPing > 5 && averPing <= 20) { (workSheet.Cells[startY + stepY -1, startX + stepX] as Excel.Range).Interior.Color = Color.GreenYellow; }
                                if (averPing > 20 && averPing <= 100) { (workSheet.Cells[startY + stepY -1, startX + stepX] as Excel.Range).Interior.Color = Color.Yellow; }
                                if (averPing > 100) { (workSheet.Cells[startY + stepY -1, startX + stepX] as Excel.Range).Interior.Color = Color.Goldenrod; }
                            }
                            

                            if (hourStep < 24)
                            {
                                hourStep++;
                                stepX++;
                            }
                            else
                            {
                                hourStep = 0;
                                stepX = 0;
                            }
                            countPing = 0;
                            summPing = 0;
                            i--;
                        }
                        else
                        {
                            if (hourStep < 24)
                            {
                                hourStep++;
                                stepX++;
                            }
                            else
                            {
                                hourStep = 0;
                                stepX = 0;
                            }
                            countPing = 0;
                            summPing = 0;
                            i--;
                        }
                    }
                }
                else
                {
                    if (NotNull)
                    {   
                        workSheet.Cells[startY + stepY, 1] = DayName;
                        stepY++;
                        currentDay++;
                        i--;
                        NotNull = false;
                    }
                    else
                    {
                        currentDay++;
                        i--;
                    }
                }

                if(i == logs.Count - 1)
                {
                    workSheet.Cells[startY + stepY, 1] = DayName;
                }
            }


            Excel.Range rngFont = workSheet.Range["A1", "Y2"];
            rngFont.Cells.Font.Bold = true;
            //rngFont.Cells.Font.Size = 12;
            //rng.Formula = "=SUM(A1:L1)";
            //rng.FormulaHidden = false;

            Excel.Range rngDash = workSheet.Range["A3", $"Y{startY + stepY}"];
            rngDash.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            // Выделяем границы у этой ячейки
            Excel.Borders border = rngDash.Borders;
            border.LineStyle = Excel.XlLineStyle.xlDash;

            // Строим круговую диаграмму
            //Excel.ChartObjects chartObjs = (Excel.ChartObjects)workSheet.ChartObjects();
            //Excel.ChartObject chartObj = chartObjs.Add(5, 50, 500, 300);
            //Excel.Chart xlChart = chartObj.Chart;
            //Excel.Range rng2 = workSheet.Range["A1:L1"];
            // Устанавливаем тип диаграммы
            //xlChart.ChartType = Excel.XlChartType.xlLineMarkers;
            // Устанавливаем источник данных (значения от 1 до 10)
            //xlChart.SetSourceData(rng2);
            //Сохраняем документ

            string path = setting.reportPath + @"\" + "Report_" + nameReport + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" +DateTime.Now.Second +  ".xlsx";
            excelApp.Application.ActiveWorkbook.SaveAs(path, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Открываем созданный excel-файл
            excelApp.Visible = true;
            excelApp.UserControl = true;
        } 
    }
}
