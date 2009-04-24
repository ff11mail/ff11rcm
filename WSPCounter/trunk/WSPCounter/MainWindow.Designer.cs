namespace WSPCounter
{
    partial class MainWindow
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.buttonApply = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textSample = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numBackAlpha = new System.Windows.Forms.NumericUpDown();
            this.numTextAlpha = new System.Windows.Forms.NumericUpDown();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBackAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTextAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "表示位置";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y";
            // 
            // numX
            // 
            this.numX.Location = new System.Drawing.Point(36, 39);
            this.numX.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(53, 19);
            this.numX.TabIndex = 3;
            this.numX.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // numY
            // 
            this.numY.Location = new System.Drawing.Point(36, 64);
            this.numY.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(53, 19);
            this.numY.TabIndex = 4;
            this.numY.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(63, 125);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 5;
            this.buttonApply.Text = "適用";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(95, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "色";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(97, 36);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "背景";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(97, 65);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(63, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "テキスト";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textSample
            // 
            this.textSample.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textSample.Location = new System.Drawing.Point(97, 94);
            this.textSample.Name = "textSample";
            this.textSample.Size = new System.Drawing.Size(100, 12);
            this.textSample.TabIndex = 9;
            this.textSample.Text = "サンプル";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(166, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "Alpha";
            // 
            // numBackAlpha
            // 
            this.numBackAlpha.Location = new System.Drawing.Point(166, 39);
            this.numBackAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numBackAlpha.Name = "numBackAlpha";
            this.numBackAlpha.Size = new System.Drawing.Size(43, 19);
            this.numBackAlpha.TabIndex = 11;
            this.numBackAlpha.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // numTextAlpha
            // 
            this.numTextAlpha.Location = new System.Drawing.Point(166, 66);
            this.numTextAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numTextAlpha.Name = "numTextAlpha";
            this.numTextAlpha.Size = new System.Drawing.Size(43, 19);
            this.numTextAlpha.TabIndex = 12;
            this.numTextAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 160);
            this.Controls.Add(this.numTextAlpha);
            this.Controls.Add(this.numBackAlpha);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textSample);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.numY);
            this.Controls.Add(this.numX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "あと何回";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBackAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTextAlpha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numX;
        private System.Windows.Forms.NumericUpDown numY;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textSample;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numBackAlpha;
        private System.Windows.Forms.NumericUpDown numTextAlpha;
        private System.Windows.Forms.Timer timer1;
    }
}

