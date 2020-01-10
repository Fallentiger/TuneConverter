using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//using TuneConvertHelper;


namespace TuneConverter.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BtnReset_Click(this, null);

            this.Title = "转调工具" + version + " @FallenTiger";
        }

        public readonly string version = "V1.1.0";

        TuneConvertHelper.TuneConverter tuneCVT;

        //private int OctOffset { get; set; }
        private int octOffset;
        private int OctOffset
        {
            get { return octOffset; }
            set
            {
                if (Math.Abs(value) <= 4)
                    octOffset = value;
                if (octOffset > 0) TbOctOffset.Text = "+" + octOffset.ToString();
                else TbOctOffset.Text = octOffset.ToString();
            }
        }

        private int halfOffset;
        private int HalfOffset
        {
            get { return halfOffset; }
            set
            {
                //halfOffset = value;
                if (value >= 12)
                {
                    halfOffset = 0;
                    OctOffset += 1;
                }
                else if (value <= -12)
                {
                    halfOffset = 0;
                    OctOffset -= 1;
                }
                else
                {
                    halfOffset = value;
                }
                if (halfOffset > 0) TbHalfOffset.Text = "+" + halfOffset.ToString();
                else TbHalfOffset.Text = halfOffset.ToString();
            }
        }


        private void BtnAddOct_Click(object sender, RoutedEventArgs e)
        {
            OctOffset += 1;
            BtnConvert_Click(this, null);
        }

        private void BtnMinusOct_Click(object sender, RoutedEventArgs e)
        {
            OctOffset -= 1;
            BtnConvert_Click(this, null);
        }

        private void BtnAddHalf_Click(object sender, RoutedEventArgs e)
        {
            HalfOffset += 1;
            BtnConvert_Click(this, null);
        }

        private void BtnMinusHalf_Click(object sender, RoutedEventArgs e)
        {
            HalfOffset -= 1;
            BtnConvert_Click(this, null);
        }

        private bool isSimplified;
        private bool IsSimplified
        {
            get { return isSimplified; }
            set
            {
                //if(isSimplified!= value)
                {
                    isSimplified = value;
                    if (isSimplified) BtnSimplify.Background = Brushes.LightGoldenrodYellow;
                    else BtnSimplify.Background = new BrushConverter().ConvertFromInvariantString("#FFDDDDDD") as Brush;

                }
            }
        }

        private void BtnSimplify_Click(object sender, RoutedEventArgs e)
        {
            IsSimplified = !IsSimplified;
            tuneCVT.CombineBracket = IsSimplified;
            BtnConvert_Click(this, null);
        }

        private bool markOnLeft;
        private bool MarkOnLeft
        {
            get { return markOnLeft; }
            set
            {
                //if (markOnLeft != value)
                {
                    markOnLeft = value;
                    if (markOnLeft) BtnChangeSharpMark.Content = "左侧升降号";
                    else BtnChangeSharpMark.Content = "右侧升降号";
                }
            }
        }

        private void BtnChangeSharpMark_Click(object sender, RoutedEventArgs e)
        {
            MarkOnLeft = !MarkOnLeft;
            tuneCVT.MarkOnLeft = MarkOnLeft;
            BtnConvert_Click(this, null);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            //markOnLeft = false;
            //isSimplified = false;

            OctOffset = 0;
            HalfOffset = 0;
            MarkOnLeft = true;
            IsSimplified = false;

            tuneCVT = new TuneConvertHelper.TuneConverter(MarkOnLeft, IsSimplified);

            //CbSrcTune.Items.Clear();
            //CbDstTune.Items.Clear();
            CbSrcTune.ItemsSource = tuneCVT.majorTuneList;
            CbDstTune.ItemsSource = tuneCVT.majorTuneList;
            CbSrcTune.SelectedIndex = 5;
            CbDstTune.SelectedIndex = 5;
        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TbInputTunes.Text))
            {
                try
                {
                    TbOutputTunes.Text = tuneCVT.StartConvert(TbInputTunes.Text.Trim(), CbSrcTune.SelectedIndex - CbDstTune.SelectedIndex + OctOffset * 12 + HalfOffset);
                }
                catch (TuneConvertHelper.ConverterException ce)
                {
                    MessageBox.Show(ce.Message, "转换失败");
                }
            }
        }

        private void BtnExchangeTune_Click(object sender, RoutedEventArgs e)
        {
            int temp = CbSrcTune.SelectedIndex;
            CbSrcTune.SelectedIndex = CbDstTune.SelectedIndex;
            CbDstTune.SelectedIndex = temp;

            BtnConvert_Click(this, null);

        }

        private void CbSrcTune_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnConvert_Click(this, null);
        }

        private void CbDstTune_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnConvert_Click(this, null);
        }

    }
}
