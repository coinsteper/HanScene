using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Dialogs;
using Excel_1 = Microsoft.Office.Interop.Excel;
using ExcelDataReader;
using System.IO;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        PictureBox[] pbName = new PictureBox[40];
        Excel_1.Worksheet ws = null;
        Excel_1.Workbook wb = null;
        string sb = "";
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel files (*.xls,*xlsx)|*.xls;*xlsx|All files (*.*)|*.*"; //필터는 변경 가능
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            textBox1.Text = dlg.FileName;

            /*
            Excel_1.Application application = new Excel_1.Application();
            application.Visible = false;
            wb = application.Workbooks.Open(dlg.FileName);

            application.Visible = false;
            application.DisplayAlerts = false;
            application.ScreenUpdating = false;
            application.Calculation = Excel_1.XlCalculation.xlCalculationManual;
            application.EnableEvents = false;
            application.ErrorCheckingOptions.BackgroundChecking = false;
            application.DisplayStatusBar = false;
            application.PrintCommunication = false;
            application.ActiveWorkbook.EnableAutoRecover = false;
            */

            //ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;
            //ws = wb.Worksheets.Item["1세부"];
            //ws = workbook.Worksheets.Item[1];

            //Excel.Range cel1 = ws.Cells[3, 3];

            //MessageBox.Show(cel1.Value.ToString());
            //MessageBox.Show(str);
            //wb.Save();
            //wb.Close(true);

            //Marshal.ReleaseComObject(wb);
            //Marshal.ReleaseComObject(application);

            //load_excel();
            load_excel2();
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
            
            listView1.Items.Clear();                        

            if (System.IO.Directory.Exists(dirPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dirPath);

                foreach(var item in di.GetDirectories())
                {
                    //MessageBox.Show(item.Name);
                    String[] aa = { item.Name, "" };
                    ListViewItem newitem = new ListViewItem(aa);                    
                    listView1.Items.Add(newitem);
                }
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

            if (listView1.SelectedIndices.Count <= 0)
            {
                return;
            }

            if (listView1.SelectedItems.Count > 0)
            {
                string a = listView1.SelectedItems[0].Text;
                ImgPath = textBox2.Text + "\\" + a;
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
            listView1.SelectedItems[0].SubItems[1].Text = "O";
            go_next();

            write_excel("O");
        }
        private void button6_Click(object sender, EventArgs e)
        {
            listView1.SelectedItems[0].SubItems[1].Text = "X";
            go_next();

            write_excel("X");
        }

        /// prev
        private void button4_Click(object sender, EventArgs e)
        {
            go_prev();          
        }
        /// next
        private void button5_Click(object sender, EventArgs e)
        {
            go_next();
        }
        private void go_prev()
        {
            if (listView1.SelectedItems[0].Index == 0)
                return;

            if (listView1.SelectedItems[0].Index >= listView1.Items.Count)
                return;

            int index_temp = listView1.SelectedItems[0].Index;
            listView1.SelectedItems[0].Selected = false;

            listView1.Focus();
            listView1.Items[index_temp - 1].Selected = true;

        }
        private void go_next()
        {
            if (listView1.SelectedItems[0].Index >= listView1.Items.Count - 1)
                return;

            int index_temp = listView1.SelectedItems[0].Index;
            listView1.SelectedItems[0].Selected = false;

            listView1.Focus();
            listView1.Items[index_temp + 1].Selected = true;

        }

        private void write_excel(string ox)
        {
            if (sb != "")
                ws = wb.Worksheets.Item[sb];

            //Excel.Range cel1 = ws.Cells[3, 3];
            //MessageBox.Show(ws.UsedRange.Rows.Count.ToString());
            //MessageBox.Show(ws.UsedRange.Columns.Count.ToString());
            //MessageBox.Show(cel1.Value.ToString());
            //Excel.Application application = new Excel.Application();
            //application.Visible = false;
            //wb = application.Workbooks.Open(textBox1.Text);

            for (int i=1; i< ws.UsedRange.Rows.Count + 1; i++)
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

        private void load_excel()
        {

            DataSet dsExcel = null;

            string filepath = textBox1.Text;
            using (var stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                ExcelReaderConfiguration conf = new ExcelReaderConfiguration();

                // 한글 인코딩
                //conf.FallbackEncoding = Encoding.GetEncoding("ks_c_5601-1987");

                using (var dtreader = ExcelReaderFactory.CreateCsvReader(stream, conf))
                {
                    dsExcel = dtreader.AsDataSet();
                }
            }
            
            foreach (DataTable datatableExcel in dsExcel.Tables)
            {
                foreach (DataRow DtRw in datatableExcel.Rows)
                {
                    MessageBox.Show(DtRw.ToString());
                    /*
                    for (int i = 0; i < exDtRw.ItemArray.Length; i++)
                    {
                        MessageBox.Show(exDtRw[i].ToString());
                    }
                    */

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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // saveExcel()
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
                pictureNumber = 20;
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

        }
    }
}
