/*
 * Created by SharpDevelop.
 * User: Eduardo
 * Date: 15/07/2014
 * Time: 21:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace KinectTest
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.campo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.campo)).BeginInit();
            this.SuspendLayout();
            // 
            // campo
            // 
            this.campo.Location = new System.Drawing.Point(1, 1);
            this.campo.Name = "campo";
            this.campo.Size = new System.Drawing.Size(640, 480);
            this.campo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.campo.TabIndex = 1;
            this.campo.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 484);
            this.Controls.Add(this.campo);
            this.Name = "MainForm";
            this.Text = "KinectTest";
            ((System.ComponentModel.ISupportInitialize)(this.campo)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.PictureBox campo;
	}
}
