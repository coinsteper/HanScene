using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Dialogs;
using Excel_1 = Microsoft.Office.Interop.Excel;
using ExcelDataReader;
using System.IO;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        PictureBox[] pbName = new PictureBox[40];
        static Excel_1.Application excelApp = null;
        static Excel_1.Worksheet ws = null;
        static Excel_1.Workbook wb = null;
        static string sb = "";
        static bool bSaved = false;
        string[,] string_temp;
        int RowCount = 0;

        private BackgroundWorker backgroundWorker;

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 40; i++)
            {
                pbName[i] = new PictureBox();
                pbName[i].Size = new Size(270, 130);
                pbName[i].Parent = this.flowLayoutPanel1;
                pbName[i].SizeMode = PictureBoxSizeMode.Zoom;
                pbName[i].DoubleClick += new EventHandler(pictureBox_Click);
            }

            //backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorkerDoWork);
            //backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorkerRunWorkerCompleted);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel files (*.xls,*xlsx)|*.xls;*xlsx|All files (*.*)|*.*"; //필터는 변경 가능
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            textBox1.Text = dlg.FileName;

            if (textBox1.Text != "")
            {
                Thread excel_worker = new Thread(LoadThread);
                excel_worker.Start();
                bSaved = false;
            }
            //load_excel2();
        }

        private void LoadThread()
        {
            Load_excel(textBox1.Text);
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
        }

        // 폴더 선택
        private void button3_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true; // true : 폴더 선택 / false : 파일 선택
            string dirPath = "";

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                dirPath = dlg.FileName;
                textBox2.Text = dlg.FileName;
                if (dlg.FileName.Contains("상용_주간도심도로"))
                    sb = "1세부";
                else if (dlg.FileName.Contains("상용_야간도심도로"))
                    sb = "2세부";
                else if (dlg.FileName.Contains("상용_주간자동차전용도로"))
                    sb = "3세부";
                else if (dlg.FileName.Contains("상용_야간자동차전용도로"))
                    sb = "4세부";
                else if (dlg.FileName.Contains("상용_악천후도로_원천"))
                    sb = "5세부";
            }

            if (textBox1.Text != "")
            {
                Thread ExcelWoker = new Thread(LoadThread);
                ExcelWoker.Start();
            }

            listView1.Items.Clear();

            if (System.IO.Directory.Exists(dirPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dirPath);

                foreach (var item in di.GetDirectories())
                {
                    //MessageBox.Show(item.Name);
                    String[] aa = { item.Name, "", "" };
                    ListViewItem newitem = new ListViewItem(aa);
                    listView1.Items.Add(newitem);
                }
            }

            if (string_temp != null)
            {
                for (int i = 0; i > listView1.Items.Count; i++)
                {
                    //listView1.Items[0].Text;
                    //listView1.Items[0].SubItems[0].Text = "sub0";
                    //listView1.Items[0].SubItems[1].Text = "sub1";
                    //listView1.Items[0].SubItems[2].Text = "sub2";

                    //MessageBox.Show(listView1.Items[0].Text);
                    //MessageBox.Show(listView1.Items[0].SubItems[0].Text);
                    //MessageBox.Show(listView1.Items[0].SubItems[1].Text);
                    //MessageBox.Show(listView1.Items[0].SubItems[2].Text);
                }
                //listView1.SelectedItems[0].SubItems[1].Text = ox;
            }
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureForm pf = new PictureForm();
            pf.Picture = ((PictureBox)sender).Image;
            pf.ShowDialog();
            //MessageBox.Show(((PictureBox)sender).ImageLocation);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }

            Thread image_load_worker = new Thread(LoadImgaeThread);
            image_load_worker.Start();
            flowLayoutPanel1.VerticalScroll.Value = 0;
            flowLayoutPanel1.PerformLayout();            
        }
        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        public void LoadImgaeThread()
        //private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            String ImgPath = "";
            /*
            if (listView1.SelectedItems.Count != 0)
            {
                int SelectRow = listView1.SelectedItems[0].Index;

                string a = listView1.Items[SelectRow].SubItems[0].Text;
                string b = listView1.Items[SelectRow].SubItems[1].Text;
                                
                ImgPath = textBox2.Text + "\\" + a;
                //MessageBox.Show(ImgFullPath);
            }*/

            //if (listView1.SelectedItems == null | listView1.SelectedIndices.Count <= 0)
            //{
            //    return;
            //}
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    if (listView1.SelectedItems.Count > 0)
                    {
                        string a = listView1.SelectedItems[0].Text;
                        ImgPath = textBox2.Text + "\\" + a;
                    }

                }));
            }          

            String img_folder_name = ImgPath + "\\sensor_raw_data\\camera";

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(img_folder_name);
            if (di.Exists == false)
                return;

            int i = 0;
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".jpg") == 0)
                {
                    String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
                    String FullFileName = File.FullName;

                    pbName[i].ImageLocation = FullFileName;
                    i++;
                    //pictureBox1.ImageLocation = FullFileName;

                    //MessageBox.Show(FullFileName + " " + FileNameOnly);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            if (textBox1.Text == "")
            {
                MessageBox.Show("엑셀 파일을 선택해 주세요");
                return;
            }

            if (string_temp == null)
            {
                MessageBox.Show("엑셀이 로드 될때까지 잠시만 기다려 주세요");

                if (string_temp == null)
                    return;
            }

            Change_OX("O");
            go_next();


            //Write_excel("O");
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            if (textBox1.Text == "")
            {
                MessageBox.Show("엑셀 파일을 선택해 주세요");
                return;
            }

            if (string_temp == null)
            {
                MessageBox.Show("엑셀이 로드 될때까지 잠시만 기다려 주세요");
                if (string_temp == null)
                    return;
            }

            Change_OX("X");
            go_next();

            //Write_excel("X");
        }

        void Change_OX(string ox)
        {
            for (int i = 0; i < string_temp.Length / 3; i++)
            {
                if (listView1.SelectedItems[0].Text == string_temp[i, 0])
                {
                    string_temp[i, 1] = ox;
                    break;
                }
            }
                listView1.SelectedItems[0].SubItems[1].Text = ox;
            bSaved = false;
        }

        /// prev
        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            go_prev();
        }
        /// next
        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            go_next();
        }
        private void go_prev()
        {
            if (listView1.SelectedItems[0].Index == 0)
            {
                MessageBox.Show("처음 입니다.");
                return;
            }

            if (listView1.SelectedItems[0].Index >= listView1.Items.Count)
                return;

            int index_temp = listView1.SelectedItems[0].Index;
            listView1.SelectedItems[0].Selected = false;
            listView1.Focus();
            listView1.Items[index_temp - 1].Selected = true;
            listView1.SelectedItems[0].Focused = true;
        }
        private void go_next()
        {
            if (listView1.SelectedItems[0].Index >= listView1.Items.Count - 1)
            {
                MessageBox.Show("마지막 입니다.");
                return;
            }

            int index_temp = listView1.SelectedItems[0].Index;
            listView1.SelectedItems[0].Selected = false;
            //listView1.SelectedItems[0].Focused = false;
            listView1.Focus();
            listView1.Items[index_temp + 1].Selected = true;
            listView1.SelectedItems[0].Focused = true;
        }

        private void Write_excel(string ox)
        {
            if (sb == "")
                return;

            ws = wb.Worksheets.Item[sb];

            //Excel.Range cel1 = ws.Cells[3, 3];
            //MessageBox.Show(ws.UsedRange.Rows.Count.ToString());
            //MessageBox.Show(ws.UsedRange.Columns.Count.ToString());
            //MessageBox.Show(cel1.Value.ToString());
            //Excel.Application application = new Excel.Application();
            //application.Visible = false;
            //wb = application.Workbooks.Open(textBox1.Text);

            for (int i = 1; i < ws.UsedRange.Rows.Count + 1; i++)
            {
                Excel_1.Range cel1 = ws.Cells[i, 3];
                if (cel1.Value != null)
                {
                    //MessageBox.Show(cel1.Value.ToString());
                    //MessageBox.Show(listView1.SelectedItems[0].Text);


                    if (listView1.SelectedItems[0].Text == cel1.Value.ToString())
                        ws.Cells[i, 4] = ox;
                    //MessageBox.Show(cel1.Value.ToString() + ":" + i.ToString());
                }
            }

            wb.Save();
        }

        private void Load_excel(string FilePath)
        {
            uint excelProcessId = 0;

            try
            {
                if (excelApp == null)
                    excelApp = new Excel_1.Application();

                excelApp.Visible = false;
                wb = excelApp.Workbooks.Open(FilePath);

                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                excelApp.ScreenUpdating = false;
                excelApp.Calculation = Excel_1.XlCalculation.xlCalculationManual;
                excelApp.EnableEvents = false;
                excelApp.ErrorCheckingOptions.BackgroundChecking = false;
                excelApp.DisplayStatusBar = false;
                excelApp.PrintCommunication = false;
                excelApp.ActiveWorkbook.EnableAutoRecover = false;

                //ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;
                if (sb == "")
                    return;

                ws = wb.Worksheets.Item[sb];
                //ws = workbook.Worksheets.Item[1];

                //Excel.Range cel1 = ws.Cells[3, 3];

                //MessageBox.Show(cel1.Value.ToString());
                //MessageBox.Show(str);
                //wb.Save();
                //wb.Close(true);

                //Marshal.ReleaseComObject(wb);
                //Marshal.ReleaseComObject(application);

                if (ws == null)
                    return;

                RowCount = ws.UsedRange.Rows.Count;                

                //string_temp = ReadRange(3, 3, RowCount + 1, 5);
                string_temp = ReadRange(3, 3, RowCount + 1, 6);
            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                // Clean up
                //ReleaseExcelObject(ws);
                //ReleaseExcelObject(wb);
                //ReleaseExcelObject(excelApp);

                if (excelApp != null && excelProcessId > 0)
                {
                    //  Process.GetProcessById((int)excelProcessId).Kill();
                }
            }
        }

        void Close_excel(bool bSave)
        {
            uint excelProcessId = 0;

            try
            {
                if (bSave)
                    wb.Save();

                GetWindowThreadProcessId(new IntPtr(excelApp.Hwnd), out excelProcessId);
                wb.Close(false);
                excelApp.Quit();
            }

            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Clean up
                ReleaseExcelObject(ws);
                ReleaseExcelObject(wb);
                ReleaseExcelObject(excelApp);

                if (excelApp != null && excelProcessId > 0)
                {
                    Process.GetProcessById((int)excelProcessId).Kill();
                }
            }
        }

        private void load_excel2()
        {
            string fname = textBox1.Text;

            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "엑셀 데이터 불러오기";
            fdlg.Filter = "All files (*.*)|*.*";
            fdlg.RestoreDirectory = true;

            using (var stream = File.Open(fname, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration() //데이터셋 변환하기
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            //Column 자동생성을 무시하고 첫번째 행을 열로 자동 지정.
                            UseHeaderRow = true,
                        }
                    });

                    MessageBox.Show(result.Tables[0].ToString());
                }
            }

            //dataGridView1.RowHeadersVisible = false; //왼쪽 화살표 제거
            //dataGridView1.DataSource = result.Tables[0]; //엑셀파일의 첫번째 Table을 가져온다
        }

        static public string[,] ReadRange(int starti, int starty, int endi, int endy)
        {
            Excel_1.Range range = (Excel_1.Range)ws.Range[ws.Cells[starti, starty], ws.Cells[endi, endy]];
            object[,] holder = range.Value2;
            string[,] returnstring = new string[endi - starti, endy - starty];

            for (int p = 1; p <= endi - starti; p++)
            {
                for (int q = 1; q <= endy - starty; q++)
                {
                    if (holder[p, q] == null)
                        continue;

                    returnstring[p - 1, q - 1] = holder[p, q].ToString();
                }
            }
            return returnstring;
        }
        static public void WriteRange(int starti, int starty, int endi, int endy, string[,] writestring)
        {
            if (sb == "")
                return;

            //ws = wb.Worksheets.Item[sb];

            if (ws == null)
                return;

            Excel_1.Range range = (Excel_1.Range)ws.Range[ws.Cells[starti, starty], ws.Cells[endi, endy]];
            range.Value2 = writestring;
            wb.Save();
            bSaved = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // saveExcel()
            /*
            if (ws == null)
            {
                return;
            }
            */

            if (excelApp == null)
                return;

            bool bSave = false;

            /// 최근에 저장 했으면 더 물어보지 않는다.
            if (bSaved == false)
            {
                if (MessageBox.Show("엑셀에 저장하시겠습니까?", "Excel Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    bSave = true;
                }
            }

            Close_excel(bSave);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton_control();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton_control();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton_control();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radioButton_control();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            radioButton_control();
        }

        void radioButton_control()
        {
            Size ss = new Size(270, 130);
            int pictureNumber = 0;
            if (radioButton1.Checked)
            {
                pictureNumber = 40;
                ss = new Size(270, 130);
            }
            else if (radioButton2.Checked)
            {
                pictureNumber = 40;
                ss = new Size(405, 195);
            }
            else if (radioButton3.Checked)
            {
                pictureNumber = 40;
                ss = new Size(500, 250);
            }
            else if (radioButton4.Checked)
            {
                pictureNumber = 40;
                ss = new Size(800, 350);
            }
            else if (radioButton5.Checked)
            {
                pictureNumber = 40;
                ss = new Size(1200, 700);
            }

            for (int i = 0; i < 40; i++)
            {
                //pbName[i] = new PictureBox();
                //pbName[i].Parent = this.flowLayoutPanel2;
                pbName[i].Visible = false;
                //this.Controls.Remove(pbName[i]);
            }

            for (int i = 0; i < pictureNumber; i++)
            {
                //pbName[i] = new PictureBox();

                pbName[i].Parent = this.flowLayoutPanel1;
                pbName[i].SizeMode = PictureBoxSizeMode.Zoom;
                pbName[i].Visible = true;
                pbName[i].Size = ss;
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.PerformClick();
            listView1.MultiSelect = false;
        }

        // save
        private void button7_Click(object sender, EventArgs e)
        {
            if (ws == null)
            {
                MessageBox.Show("엑셀 파일을 선택해 주세요");
                return;
            }

            WriteRange(3, 3, RowCount, 5, string_temp);

            MessageBox.Show("저장 완료");
        }
        private static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception)
            {
                obj = null;
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox1.Focused | textBox2.Focused | textBox3.Focused)
            {
                return;
            }

            switch (e.KeyCode)
            {
                //case Keys.Space:
                 //   button2.PerformClick();
                 //   break;
                case Keys.D1:
                    radioButton1.Checked = true;
                    break;
                case Keys.D2:
                    radioButton2.Checked = true;
                    break;
                case Keys.D3:
                    radioButton3.Checked = true;
                    break;
                case Keys.D4:
                    radioButton4.Checked = true;
                    break;
                case Keys.D5:
                    radioButton5.Checked = true;
                    break;

                case Keys.A:
                    button4.PerformClick();
                    break;

                case Keys.D:
                    button5.PerformClick();
                    break;

                case Keys.F:
                    button8.PerformClick();
                    break;

                case Keys.G:
                    button9.PerformClick();
                    break;

                case Keys.Space:
                    button2.PerformClick();
                    break;

                case Keys.ShiftKey:
                    button6.PerformClick();
                    break;

                case Keys.RControlKey & Keys.S:
                    button7.PerformClick();
                    break;

                //case Keys.RControlKey & Keys.O:
                //    button1.PerformClick();
                //    break;
                

                default:
                    break;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("폴더를 선택해 주세요");
                return;
            }

            if (textBox1.Text == "")
            {
                MessageBox.Show("엑셀 파일을 선택해 주세요");
                return;
            }

            if (textBox3.Text == "")
            {
                return;
            }

            listView1.SelectedItems[0].SubItems[2].Text = textBox3.Text;
            for (int i = 0; i < string_temp.Length / 3; i++)
            {
                if (listView1.SelectedItems[0].Text == string_temp[i, 0])
                {
                    string_temp[i, 2] = textBox3.Text;
                    break;
                }
            }
            bSaved = false;
        }


        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            Keys keys = keyData & ~(Keys.Shift | Keys.Control | Keys.Alt);

            switch (keys)
            {
                //case Keys.Down:
                //    {
                //        //flowLayoutPanel1.Focus();
                //        //if (pbName[0] != null)
                //        //    pbName[0].Focus();
                //        //flowLayoutPanel1.Focus();


                //        flowLayoutPanel1.PerformLayout();
                //        if (flowLayoutPanel1.VerticalScroll.Value >= 100)
                //        {
                //            flowLayoutPanel1.VerticalScroll.Value = flowLayoutPanel1.VerticalScroll.Value + 100;
                //            flowLayoutPanel1.PerformLayout();
                //        }                        
                //    }
                //    break;

                //case Keys.Up:
                //    {
                //        if (pbName[0] != null)
                //            pbName[0].Focus();
                //        //flowLayoutPanel1.Focus();
                //        pictureBox1.Focus();
                //        if (flowLayoutPanel1.VerticalScroll.Value <= 50)
                //        {
                //            flowLayoutPanel1.VerticalScroll.Value = flowLayoutPanel1.VerticalScroll.Value - 50;
                //            flowLayoutPanel1.PerformLayout();
                //        }                    
                //    }
                //    break;
                case Keys.S:
                    {
                        if ((keyData & Keys.Control) != 0)
                        {
                            button7.PerformClick();
                        }
                        break;
                    }

                case Keys.O:
                    {
                        if ((keyData & Keys.Control) != 0)
                        {
                            button1.PerformClick();
                        }
                        break;
                    }                                
            }

            return base.ProcessCmdKey(ref message, keyData);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }
    }
}
