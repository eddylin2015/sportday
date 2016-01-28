using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SportDayLib;

namespace SportDay
{
    public partial class FormGirdPanel : Form
    {
        
        public FormGirdPanel(string txt):base()
        {
            InitializeComponent();
            this.Text = txt;
            String filename = Basic_HTB_Info.baseFilePath + @"\"+txt+".txt";
            FileInfo finfo = new FileInfo(filename);
            if (!finfo.Exists)
            {
                StreamWriter sw = new StreamWriter(filename, false, Encoding.Default);
                int i = 1;
               
                foreach (DictionaryEntry de in Basic_HTB_Info.GetInstance.Race_Events)
                {
                    if ("day1" == txt && int.Parse(de.Key.ToString()) / 10000 == 1)
                    { sw.WriteLine("TextBox{2}:({0}){1}", int.Parse(de.Key.ToString()), de.Value.ToString(), i++); }
                    else if ("day2" == txt && int.Parse(de.Key.ToString()) / 10000 == 2)
                    {
                        sw.WriteLine("TextBox{2}:({0}){1}", int.Parse(de.Key.ToString()), de.Value.ToString(), i++);
                    }
                }
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            StreamReader sr = new StreamReader(filename, Encoding.Default);
            string input = null;
            while ((input = sr.ReadLine()) != null)
            {
                string[] strA = input.Split(':');
                tableLayoutPanel1.Controls[strA[0]].Text = strA[1];                
            }
            sr.Close();
            sr.Dispose();

            foreach (Object tb in this.tableLayoutPanel1.Controls)
            {
                if (tb is TextBox)
                {
                    TextBox aTextBox = (TextBox)tb;
                    aTextBox.ReadOnly = true;
                    aTextBox.AllowDrop = true;
                    aTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextBox_DragDrop);
                    aTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
                    aTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextBox_MouseDown);
                }
            }
        }
        private void TextBox_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.SelectAll();
            txt.DoDragDrop(txt.Name+":"+txt.Text, DragDropEffects.Copy);
        }

        private void TextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void TextBox_DragDrop(object sender, DragEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            string[] strA = ((string)e.Data.GetData(DataFormats.Text)).Split(':');
            if (strA.Length == 2)
            {
                string temp = txt.Text;
                txt.Text = strA[1];
                tableLayoutPanel1.Controls[strA[0]].Text = temp;
            }

            //if (contr.ExchangeSubject((string)e.Data.GetData(DataFormats.Text), txt.Name)) RefreshContent();
        }

        private void FormGirdPanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            String filename = Basic_HTB_Info.baseFilePath + @"\" + this.Text + ".txt";
            StreamWriter sw = new StreamWriter(filename, false, Encoding.Default);
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                sw.WriteLine("{0}:{1}", c.Name, c.Text);
            }

            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}