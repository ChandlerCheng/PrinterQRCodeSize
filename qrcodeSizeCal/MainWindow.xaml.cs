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
using System.Text.RegularExpressions;

namespace qrcodeSizeCal
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    ///     
    public partial class MainWindow : Window
    {
        private const string ErrorMessage = "Result Error"; 
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
        public void ChangeResultColor(bool IsError)
        {
            if (IsError)
            {
                ResultRatio.Foreground = new SolidColorBrush(Colors.Red);
                ResultLevel.Foreground = new SolidColorBrush(Colors.Red);
                ResultMm.Foreground = new SolidColorBrush(Colors.Red);
                ResultDot.Foreground = new SolidColorBrush(Colors.Red);
                ResultMinusDot.Foreground = new SolidColorBrush(Colors.Red);
                ResultDpi.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ResultRatio.Foreground = new SolidColorBrush(Colors.Green);
                ResultLevel.Foreground = new SolidColorBrush(Colors.Green);
                ResultMm.Foreground = new SolidColorBrush(Colors.Green);
                ResultDot.Foreground = new SolidColorBrush(Colors.Green);
                ResultMinusDot.Foreground = new SolidColorBrush(Colors.Green);
                ResultDpi.Foreground = new SolidColorBrush(Colors.Green);
            }
        }
        
        public static int[] qr_sizes = {
        21, 25, 29, 33, 37,						/* 1 - 5*/
	    41, 45, 49, 53, 57, 					/* 6 - 10*/
	    61, 65, 69, 73, 77, 					/* 11 - 15*/
	    81, 85, 89, 93, 97,						/* 16 - 20*/
	    101, 105, 109, 113, 117, 			/* 21 - 25*/
	    121, 125, 129, 133, 137, 			/* 26 - 30*/
	    141, 145, 149, 153, 157, 			/* 31 - 35*/
	    161, 165, 169, 173, 177				/* 36 - 40*/
        };
        public static int[] dpi = { 203, 300 };
        public string [] unit= {"毫米(mm)","點數(dot)"};
        public string[] levelUse = { "等於", "大於等於","小於等於" };
        public MainWindow()
        {
            InitializeComponent();

            /* 選項添加 */
            for (int i = 1; i < qr_sizes.Length + 1; i++)
                comboBarLevel.Items.Add(i);// barcode選項添加

            for (int i = 0; i < dpi.Length; i++)
                comboDpi.Items.Add(dpi[i]); // dpi選項添加

            for (int i = 0; i < unit.Length; i++)
                comboUnit.Items.Add(unit[i]);   // 解析度選項添加

            for (int i = 0; i < levelUse.Length; i++)
                comboSelectLevel.Items.Add(levelUse[i]);    //計算方式選項添加

            /* 選項初始化 */
            comboDpi.SelectedIndex = 0;
            comboBarLevel.SelectedIndex = 0;
            comboUnit.SelectedIndex = 0;
            comboSelectLevel.SelectedIndex = 0;
        }
        
        private void CalFinal_Click(object sender, RoutedEventArgs e)
        {
            int getBarLevel, dpi, Minus = 0, useLevelType = 0;
            int i,j;
            int getFinalMm;
            int tempFinal = 0, tempRatio = 0, tempBarLevel = 0, tempBarIndex = 0;
            int FinalValue = 0, FinalRatio = 0, FinalBarLevel = 0, FinalBarIndex = 0;
            bool unitIsMm;    

            /* 檢查長度 */
            if (InputFinalMm.Text.Length == 0)            
                return;

            /* 確定輸入單位與換算 */
            if (comboSelectLevel.SelectedIndex == -1 || comboSelectLevel.SelectedIndex == 0)
                useLevelType = 0;
            else if (comboSelectLevel.SelectedIndex == 1)
                useLevelType = 1;
            else if (comboSelectLevel.SelectedIndex == 2)
                useLevelType = 2;

            /* 確定輸入單位與換算 */
            if (comboDpi.SelectedIndex == -1 || comboDpi.SelectedIndex == 0)
                dpi = 8;
            else if (comboDpi.SelectedIndex == 1)
                dpi = 12;
            else if (comboDpi.SelectedIndex == 2)
                dpi = 15;
            else
                dpi = 8;

            if (comboUnit.SelectedIndex == -1 || comboUnit.SelectedIndex == 0)
                unitIsMm = true;
            else
                unitIsMm = false;

            /* 確認基礎條碼等級 */
            if (comboBarLevel.SelectedIndex == -1)
                getBarLevel = qr_sizes[0];
            else
                getBarLevel = qr_sizes[comboBarLevel.SelectedIndex];

           

            if (unitIsMm)
                getFinalMm = ((int)double.Parse(InputFinalMm.Text)) * dpi;
            else
                getFinalMm = (int)double.Parse(InputFinalMm.Text);

            if (getFinalMm <= 0)
                return;

            /* 計算 */
            for (i = 0; i < qr_sizes.Length; i++)
            {
                if (useLevelType == 1)
                {
                    /* 使用等於或高於設定條碼 */
                    if (getBarLevel > qr_sizes[i])
                        continue;
                }
                else if (useLevelType == 2)
                {
                    /* 使用等於或低於設定條碼 */
                    if (getBarLevel < qr_sizes[i])
                        continue;
                }
                else
                {
                    /* 指定條碼尺寸計算  */
                    if (getBarLevel != qr_sizes[i])
                        continue;
                }

                for (j = 1; j < 11; j++)
                {
                    if (qr_sizes[i] * j <= getFinalMm)
                    {
                        tempBarIndex = i;
                        tempRatio = j;
                        tempFinal = qr_sizes[i] * j;
                        tempBarLevel = qr_sizes[i];
                    }
                    else
                        break;
                }

                if (getFinalMm - tempFinal < Minus || Minus == 0)
                {
                    Minus = getFinalMm - tempFinal;
                    FinalBarIndex = tempBarIndex;
                    FinalValue = tempFinal;
                    FinalRatio = tempRatio;
                    FinalBarLevel = tempBarLevel;
                }
            }

            if (Minus == 0 || FinalValue == 0|| FinalRatio==0 || FinalBarLevel== 0)
            {
                /* 輸出結果異常 */
                ChangeResultColor(true);
                ResultRatio.Text = ResultLevel.Text =ResultMm.Text = ResultDot.Text = ResultMinusDot.Text = ErrorMessage;
            }
            else
            {
                ChangeResultColor(false);
                ResultRatio.Text = FinalRatio.ToString();
                ResultLevel.Text = (FinalBarIndex + 1) + "(" + FinalBarLevel.ToString() + ")";
                ResultMm.Text = (FinalValue / dpi).ToString() + " mm";
                ResultDot.Text = FinalValue.ToString() + " dot";
                ResultMinusDot.Text = Minus.ToString() + " dot";
            }

            if (dpi == 12)
                ResultDpi.Text = "300dpi";
            else if (dpi == 15)
                ResultDpi.Text = "600dpi";
            else
                ResultDpi.Text = "203dpi";
            //TextResult.Text = "倍率:" + FinalRatio + "等級:" + FinalBarLevel + "mm:" + FinalValue / 8;
        }
    }
}
