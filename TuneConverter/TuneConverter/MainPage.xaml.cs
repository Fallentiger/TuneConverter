using System;
using Xamarin.Forms;

namespace TuneConverter
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BtnReset_Click(this, null);

        }

        TuneConvertHelper.TuneConverter tuneCVT;
        private string inputNodes;

        //private int OctOffset { get; set; }
        private int octOffset;
        private int OctOffset
        {
            get { return octOffset; }
            set
            {
                if (Math.Abs(value) <= 4) octOffset = value;
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
                //if (Math.Abs(value) <= 11) halfOffset = value;
                if (value >= 12)
                {
                    halfOffset = 0;
                    OctOffset += 1;
                }
                else if(value <= -12)
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


        private void BtnAddOct_Click(object sender, EventArgs e)
        {
            OctOffset += 1;
            BtnConvert_Click(this, null);
        }

        private void BtnMinusOct_Click(object sender, EventArgs e)
        {
            OctOffset -= 1;
            BtnConvert_Click(this, null);
        }

        private void BtnAddHalf_Click(object sender, EventArgs e)
        {
            HalfOffset += 1;
            BtnConvert_Click(this, null);
        }

        private void BtnMinusHalf_Click(object sender, EventArgs e)
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
                    if (isSimplified) BtnSimplify.BackgroundColor = Color.LightGoldenrodYellow;
                    else BtnSimplify.BackgroundColor = new Color(210, 210, 210);

                }
            }
        }

        private void BtnSimplify_Click(object sender, EventArgs e)
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
                    if (markOnLeft) BtnChangeSharpMark.Text = "左侧升降号";
                    else BtnChangeSharpMark.Text = "右侧升降号";
                }
            }
        }

        private void BtnChangeSharpMark_Click(object sender, EventArgs e)
        {
            MarkOnLeft = !MarkOnLeft;
            tuneCVT.MarkOnLeft = MarkOnLeft;
            BtnConvert_Click(this, null);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            //markOnLeft = false;
            //isSimplified = false;
            inputNodes = null;

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

        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (e != null) inputNodes = TbInputTunes.Text.Trim();

            if (!string.IsNullOrWhiteSpace(inputNodes))
            {
                try
                {
                    TbInputTunes.Text = tuneCVT.StartConvert(inputNodes, CbSrcTune.SelectedIndex - CbDstTune.SelectedIndex + OctOffset * 12 + HalfOffset);
                }
                catch (TuneConvertHelper.ConverterException ce)
                {
                    await DisplayAlert("转换失败", ce.Message, "OK");

                    //MessageBox.Show("转换失败\n" + ce.Message);
                }
            }
        }

        private void BtnExchangeTune_Click(object sender, EventArgs e)
        {
            int temp = CbSrcTune.SelectedIndex;
            CbSrcTune.SelectedIndex = CbDstTune.SelectedIndex;
            CbDstTune.SelectedIndex = temp;

            BtnConvert_Click(this, null);
        }

        private void CbSrcTune_SelectionChanged(object sender, EventArgs e)
        {
            BtnConvert_Click(this, null);
        }

        private void CbDstTune_SelectionChanged(object sender, EventArgs e)
        {
            BtnConvert_Click(this, null);
        }

        private void CbSrcTune_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnConvert_Click(this, null);
        }

        private void CbDstTune_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnConvert_Click(this, null);
        }

    }
}
