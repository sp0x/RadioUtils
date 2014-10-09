<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.lblSong = New System.Windows.Forms.Label()
        Me.BtnPlay = New System.Windows.Forms.Button()
        Me.lblName = New System.Windows.Forms.Label()
        Me.TrackBar1 = New System.Windows.Forms.TrackBar()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Interval = 50
        '
        'lblSong
        '
        Me.lblSong.AutoSize = True
        Me.lblSong.Location = New System.Drawing.Point(12, 192)
        Me.lblSong.Name = "lblSong"
        Me.lblSong.Size = New System.Drawing.Size(0, 13)
        Me.lblSong.TabIndex = 1
        '
        'Button2
        '
        Me.BtnPlay.Location = New System.Drawing.Point(153, 217)
        Me.BtnPlay.Name = "Button2"
        Me.BtnPlay.Size = New System.Drawing.Size(67, 35)
        Me.BtnPlay.TabIndex = 3
        Me.BtnPlay.Text = "Play"
        Me.BtnPlay.UseVisualStyleBackColor = True
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(12, 205)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(41, 13)
        Me.lblName.TabIndex = 4
        Me.lblName.Text = "Name: "
        '
        'TrackBar1
        '
        Me.TrackBar1.Location = New System.Drawing.Point(227, 217)
        Me.TrackBar1.Maximum = 100
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Size = New System.Drawing.Size(135, 45)
        Me.TrackBar1.SmallChange = 5
        Me.TrackBar1.TabIndex = 5
        Me.TrackBar1.TickFrequency = 5
        Me.TrackBar1.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.TrackBar1.Value = 60
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(80, 216)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(67, 35)
        Me.Button1.TabIndex = 6
        Me.Button1.Text = "Stop"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(15, 13)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(345, 178)
        Me.PictureBox1.TabIndex = 7
        Me.PictureBox1.TabStop = False
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(15, 262)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(347, 20)
        Me.TextBox1.TabIndex = 8
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(372, 288)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TrackBar1)
        Me.Controls.Add(Me.lblName)
        Me.Controls.Add(Me.BtnPlay)
        Me.Controls.Add(Me.lblSong)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents lblSong As System.Windows.Forms.Label
    Friend WithEvents BtnPlay As System.Windows.Forms.Button
    Friend WithEvents lblName As System.Windows.Forms.Label
    Friend WithEvents TrackBar1 As System.Windows.Forms.TrackBar
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox

End Class
