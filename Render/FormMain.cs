using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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

                Invalidate();
            }
        }

        private void FormMain_Paint(object sender, PaintEventArgs e) {
            MeasureTime();

            string info = $"{fpsAvg,4:f0} fps, {dtimeAvg * 1000,5:f2} ms";
            e.Graphics.DrawString(info, Font, Brushes.Black, 0, 0);
        }
    }
}
