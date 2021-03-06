﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Render
{
    public partial class FormMain : Form
    {
        public FormMain() {
            InitializeComponent();
        }

        private static double GetTime() {
            return (double)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
        }

        int cnt = 0;
        double sum = 0;
        double fpsAvg = 0;
        double dtimeAvg = 0;
        double timeOld = 0;
        double timeDelta = 0;
        private void MeasureTime() {
            double timeNew = GetTime();
            timeDelta = (timeNew - timeOld);
            timeOld = timeNew;

            cnt++;
            sum += timeDelta;
            if (sum > 1) {
                dtimeAvg = sum / cnt;
                fpsAvg = 1 / dtimeAvg;
                cnt = 0;
                sum = 0;
            }
        }

        private void FormMain_Shown(object sender, EventArgs e) {
            timeOld = GetTime();
            while (true) {
                Application.DoEvents();

                if (IsDisposed)
                    break;

                MeasureTime();

                using (var g = Graphics.FromImage(dispBmp)) {
                    string info = $"{fpsAvg,4:f0} fps, {dtimeAvg * 1000,5:f2} ms";
                    g.Clear(Color.White);
                    g.DrawString(info, Font, Brushes.Black, 0, 0);
                }

                bufG.Graphics.DrawImage(dispBmp, 0, 0);
                bufG.Render();
            }
        }

        IntPtr dispBuf = IntPtr.Zero;
        int dispBW = 0;
        int dispBH = 0;
        int dispStride = 0;
        Bitmap dispBmp = null;
        BufferedGraphics bufG = null;
        private void FormMain_Layout(object sender, LayoutEventArgs e) {
            var size = ClientSize;
            dispBW = size.Width;
            dispBH = size.Height;
            dispStride = dispBW * 4;

            if (dispBuf != IntPtr.Zero) {
                dispBmp.Dispose();
                Marshal.FreeHGlobal(dispBuf);
            }
            dispBuf = Marshal.AllocHGlobal(dispStride * dispBH);
            dispBmp = new Bitmap(dispBW, dispBH, dispStride, PixelFormat.Format32bppPArgb, dispBuf);
            
            if (bufG != null)
                bufG.Dispose();
            bufG = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), ClientRectangle);
            bufG.Graphics.CompositingMode = CompositingMode.SourceCopy;
        }
    }
}
