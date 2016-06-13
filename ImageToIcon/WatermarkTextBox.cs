
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace wmgCMS
{
    class WaterMarkTextBox : RichTextBox
    {
        private Font oldFont = null;

        private Boolean waterMarkTextEnabled = false;
        #region "Attributes"
        private Color _waterMarkColor = Color.Gray;
        public Color WaterMarkColor
        {
            get { return _waterMarkColor; }
            set
            {
                _waterMarkColor = value;
                //thanks to Bernhard Elbl for Invalidate()
                Invalidate();
            }
        }

        private string _waterMarkText = "Water Mark";
        public string WaterMarkText
        {
            get { return _waterMarkText; }
            set
            {
                _waterMarkText = value;
                Invalidate();
            }
        }

        private bool _waterMarkLocked = false;
        public bool WaterMarkLocked
        {
            get { return _waterMarkLocked; }
            set { _waterMarkLocked = value; }
        }
        #endregion

        //Default constructor
        public WaterMarkTextBox()
        {
            JoinEvents(true);
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        //Override OnCreateControl ... thanks to  "lpgray .. codeproject guy"
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            WaterMark_Toggel(null, null);
        }

        //Override OnPaint
        protected override void OnPaint(PaintEventArgs args)
        {
            // Use the same font that was defined in base class
            System.Drawing.Font drawFont = new System.Drawing.Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);
            //Create new brush with gray color or 
            SolidBrush drawBrush = new SolidBrush(WaterMarkColor);
            //use Water mark color
            //Draw Text or WaterMark
            args.Graphics.DrawString((waterMarkTextEnabled ? WaterMarkText : Text), drawFont, drawBrush, new PointF(1f, 1f));
            base.OnPaint(args);
        }

        private void JoinEvents(Boolean @join)
        {
            if (@join)
            {
                this.TextChanged += this.WaterMark_Toggel;
                this.LostFocus += this.WaterMark_Toggel;
                //No one of the above events will start immeddiatlly 
                //TextBox control still in constructing, so,
                //Font object (for example) couldn't be catched from within WaterMark_Toggle
                //So, call WaterMark_Toggel through OnCreateControl after TextBox is totally created
                //No doupt, it will be only one time call

                //Old solution uses Timer.Tick event to check Create property
                this.FontChanged += this.WaterMark_FontChanged;
            }
        }

        public void WaterMark_Toggel(object sender, EventArgs args)
        {
            if (this.Text.Length <= 0)
            {
                EnableWaterMark();
            }
            else
            {
                DisbaleWaterMark();
            }
        }

        public void EnableWaterMark()
        {
            if (WaterMarkLocked == false)
            {
                //Save current font until returning the UserPaint style to false (NOTE: It is a try and error advice)
                oldFont = new System.Drawing.Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);
                //Enable OnPaint event handler
                this.SetStyle(ControlStyles.UserPaint, true);
                this.waterMarkTextEnabled = true;
                //Triger OnPaint immediatly
                Refresh();
            }
        }

        public void DisbaleWaterMark()
        {
            //Disbale OnPaint event handler
            this.waterMarkTextEnabled = false;
            this.SetStyle(ControlStyles.UserPaint, false);
            //Return back oldFont if existed
            if (oldFont != null)
            {
                this.Font = new System.Drawing.Font(oldFont.FontFamily, oldFont.Size, oldFont.Style, oldFont.Unit);
            }
        }

        private void WaterMark_FontChanged(object sender, EventArgs args)
        {
            if (waterMarkTextEnabled)
            {
                oldFont = new System.Drawing.Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);
                Refresh();
            }
        }
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_CONTEXTMENU = 0x7b;

            if (m.Msg != WM_CONTEXTMENU)
            {
                base.WndProc(ref m);
            }
        }
    }
}
