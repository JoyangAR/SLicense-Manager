<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class NewProductfrm
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.textBoxProductName = New System.Windows.Forms.TextBox()
        Me.textBoxProductPassword = New System.Windows.Forms.TextBox()
        Me.BtnSave = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'textBoxProductName
        '
        Me.textBoxProductName.Location = New System.Drawing.Point(10, 10)
        Me.textBoxProductName.Name = "textBoxProductName"
        Me.textBoxProductName.Size = New System.Drawing.Size(200, 20)
        Me.textBoxProductName.TabIndex = 0
        '
        'textBoxProductPassword
        '
        Me.textBoxProductPassword.Location = New System.Drawing.Point(10, 40)
        Me.textBoxProductPassword.Name = "textBoxProductPassword"
        Me.textBoxProductPassword.Size = New System.Drawing.Size(200, 20)
        Me.textBoxProductPassword.TabIndex = 1
        Me.textBoxProductPassword.UseSystemPasswordChar = True
        '
        'BtnSave
        '
        Me.BtnSave.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnSave.Location = New System.Drawing.Point(10, 70)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(200, 23)
        Me.BtnSave.TabIndex = 2
        Me.BtnSave.Text = "Save Product"
        '
        'NewProductfrm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(223, 104)
        Me.Controls.Add(Me.textBoxProductName)
        Me.Controls.Add(Me.textBoxProductPassword)
        Me.Controls.Add(Me.BtnSave)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "NewProductfrm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add Product"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents textBoxProductName As TextBox
    Friend WithEvents textBoxProductPassword As TextBox
    Friend WithEvents BtnSave As Button
End Class
