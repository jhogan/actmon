using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
namespace DAC.WindowWatchers
{
	/// <summary>
	/// Summary description for DataTable.
	/// </summary>
	public class DataTableForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid DataGrid;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DataTable DataTable
		{
			set
			{
				this.DataGrid.DataSource = value;
			}
			get
			{
				return (DataTable) this.DataGrid.DataSource;
			}
		}
		public DataTableForm(DataTable dataTable)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.DataTable = dataTable;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.DataGrid = new System.Windows.Forms.DataGrid();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.DataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// DataGrid
			// 
			this.DataGrid.DataMember = "";
			this.DataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.DataGrid.Location = new System.Drawing.Point(8, 16);
			this.DataGrid.Name = "DataGrid";
			this.DataGrid.Size = new System.Drawing.Size(1224, 768);
			this.DataGrid.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(16, 808);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 24);
			this.button1.TabIndex = 1;
			this.button1.Text = "button1";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(136, 808);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(80, 24);
			this.button2.TabIndex = 2;
			this.button2.Text = "button2";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// DataTableForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1240, 917);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.DataGrid);
			this.Name = "DataTableForm";
			this.Text = "DataTable";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			((System.ComponentModel.ISupportInitialize)(this.DataGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.DataTable.Rows.RemoveAt(0);
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			//ScalerSamples samples = new ScalerSamples();

			DataRow dr = this.DataTable.NewRow();
			dr[0] = 1;
			this.DataTable.Rows.InsertAt(dr,0);
		}
	}
}
