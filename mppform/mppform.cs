using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace MPPFORM
{
    /// <summary>
    /// 消息框(TextBox MultiLine)
    /// </summary>
    public class MsgBox : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TextBox tb = new System.Windows.Forms.TextBox();
        /// <summary>
        /// MsgBox構造
        /// <example>
        /// MsgBox msg=new MsgBox(LastError.GetArray());
        /// MsgBox.ShowDialog();
        /// </example>
        /// </summary>
        /// <param name="a_msg">String[] 消息內容 </param>
        public MsgBox(string[] a_msg)
            : base()
        {
            tb.Multiline = true;
            tb.Lines = a_msg;
            tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(tb);
        }
    }
    /// <summary>
    /// 數字選上下表單
    /// </summary>
    public class NumericUpDownBox : System.Windows.Forms.Form
    {
        private System.Windows.Forms.NumericUpDown nud = new System.Windows.Forms.NumericUpDown();
        private System.Windows.Forms.Button OKbtn = new System.Windows.Forms.Button();
        private System.Windows.Forms.FlowLayoutPanel flp = new System.Windows.Forms.FlowLayoutPanel();
        /// <summary>
        /// 返回數字
        /// </summary>
        public decimal GetNum{get{return nud.Value;}}
        /// <summary>
        /// 構造
        /// </summary>
        /// <param name="TXT">表單標題</param>
        /// <param name="num">預設數字</param>
        public NumericUpDownBox(string TXT, int num)
        {
            this.Text = TXT;
            nud.Minimum = -1;
            nud.Maximum = 200;
            nud.Value = num;
            OKbtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKbtn.Text = "ok";
            flp.Controls.Add(nud);
            flp.Controls.Add(OKbtn);
            Controls.Add(flp);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class InputStrBox : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TextBox txtbox = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.Button OKbtn = new System.Windows.Forms.Button();
        private System.Windows.Forms.FlowLayoutPanel flp = new System.Windows.Forms.FlowLayoutPanel();
        /// <summary>
        /// 返回文字
        /// </summary>
        public string textbox_str { get { return txtbox.Text;} }
        /// <summary>
        /// 構造
        /// </summary>
        /// <param name="TXT">表單標題</param>
        /// <param name="default_txt">default text</param>
        public InputStrBox(string TXT,string default_txt)
        {
            this.Text = TXT;
            OKbtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKbtn.Text = "ok";
            txtbox.Text = default_txt;
            flp.Controls.Add(txtbox);
            flp.Controls.Add(OKbtn);
            Controls.Add(flp);
        }
    }
    /// <summary>
    /// 列表表單
    /// </summary>
    public class ListBoxForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// CheckedListBox
        /// </summary>
        public System.Windows.Forms.CheckedListBox lb = new System.Windows.Forms.CheckedListBox();
        
        private System.Windows.Forms.Button SelAllbtn = new System.Windows.Forms.Button();
        private System.Windows.Forms.Button OKbtn = new System.Windows.Forms.Button();
        private System.Windows.Forms.TableLayoutPanel tblp = new System.Windows.Forms.TableLayoutPanel();
        /// <summary>
        /// 構造
        /// </summary>
        public ListBoxForm()
        {
            OKbtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKbtn.Text = "ok";
            SelAllbtn.Text = "Select All";
            SelAllbtn.Click += selall_click;
            lb.Dock = System.Windows.Forms.DockStyle.Fill;
            lb.Width = 230;
            tblp.ColumnCount = 2;
            tblp.RowCount = 2;
            tblp.Controls.Add(lb);
            tblp.SetRowSpan(lb,2);
            tblp.Controls.Add(OKbtn);
            tblp.Controls.Add(SelAllbtn);
            tblp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            tblp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));

            tblp.Dock = System.Windows.Forms.DockStyle.Fill;
            Controls.Add(tblp);
        }
        private void selall_click(Object sender,EventArgs e)
        {
            for(int i=0;i<lb.Items.Count;i++)
                lb.SetItemChecked(i,true);
        }
    }
    /// <summary>
    /// 過濾列表表單
    /// </summary>
    public class ListBoxFilterForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// CheckedListBox
        /// </summary>
        public System.Windows.Forms.CheckedListBox lb = new System.Windows.Forms.CheckedListBox();
        
        private System.Windows.Forms.Button SelAllbtn = new System.Windows.Forms.Button();
        private System.Windows.Forms.Button OKbtn = new System.Windows.Forms.Button();
        private System.Windows.Forms.TextBox filterTB =new TextBox();
        private System.Windows.Forms.TableLayoutPanel tblp = new System.Windows.Forms.TableLayoutPanel();
        private List<String> items_bk=null;
        /// <summary>
        /// 構造
        /// </summary>
        public ListBoxFilterForm ()
        {
            OKbtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKbtn.Text = "ok";
            SelAllbtn.Text = "Select All";
            SelAllbtn.Click += selall_click;
            lb.Dock = System.Windows.Forms.DockStyle.Fill;
            lb.Width = 230;
            tblp.ColumnCount = 2;
            tblp.RowCount = 3;
            tblp.Controls.Add(lb);
            tblp.SetRowSpan(lb,3);
            tblp.Controls.Add(OKbtn);
            tblp.Controls.Add(SelAllbtn);
            filterTB.TextChanged+=new EventHandler(filterTB_TextChanged);
            tblp.Controls.Add(filterTB);
            tblp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            tblp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));

            tblp.Dock = System.Windows.Forms.DockStyle.Fill;
            Controls.Add(tblp);
        }
        private void filterTB_TextChanged(Object sender, EventArgs e)
       {
	if(items_bk==null)
	{
		items_bk=new List<String>();
		for(int i=0;i<lb.Items.Count;i++) items_bk.Add(lb.Items[i].ToString());
	}
	lb.Items.Clear();
	for(int i=0;i<items_bk.Count;i++) if(items_bk[i].ToLower().Contains(filterTB.Text.ToLower())) lb.Items.Add(items_bk[i]);
        }
        private void selall_click(Object sender,EventArgs e)
        {
            for(int i=0;i<lb.Items.Count;i++)
                lb.SetItemChecked(i,true);
        }
    }
}
