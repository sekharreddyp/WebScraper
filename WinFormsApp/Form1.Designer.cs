using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using RazorClassLibrary.Components;

namespace WinFormsApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //this.blazorWebView1 = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();

            //this.SuspendLayout();

            //this.blazorWebView1.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.blazorWebView1.Location = new System.Drawing.Point(0, 0);
            //this.blazorWebView1.Name = "blazorWebView1";
            //this.blazorWebView1.Size = new System.Drawing.Size(800, 450);
            //this.blazorWebView1.TabIndex = 0;


            //this.components = new System.ComponentModel.Container();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(800, 450);
            //this.Controls.Add(this.blazorWebView1);
            //this.Text = "Form1";
            //this.ResumeLayout(false);

            this.blazorWebView = new BlazorWebView();
            this.SuspendLayout();

            // 
            // blazorWebView
            // 
            this.blazorWebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blazorWebView.HostPage = @"wwwroot/index.html";
            this.blazorWebView.Location = new System.Drawing.Point(0, 0);
            this.blazorWebView.Name = "blazorWebView";
            this.blazorWebView.Size = new System.Drawing.Size(900, 500);
            this.blazorWebView.TabIndex = 0;
            this.blazorWebView.Services = Program.CreateHostBuilder().Build().Services;
            this.blazorWebView.RootComponents.Add<App>("#app");

            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.blazorWebView);
            this.Name = "Form1";
            this.Text = "Lets Scrape";
            this.ResumeLayout(false);

        }

        #endregion
        private BlazorWebView blazorWebView;
        //private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView1;
    }
}
