<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LicenseViewfrm
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.LbxClients = New System.Windows.Forms.ListBox()
        Me.BtnCreateLic = New System.Windows.Forms.Button()
        Me.GpbValidation = New System.Windows.Forms.GroupBox()
        Me.TxtValidation = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtEMail = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtAttributeValue = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CbxLicenseType = New System.Windows.Forms.ComboBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.DtpExpiration = New System.Windows.Forms.DateTimePicker()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.GpbLicDetails = New System.Windows.Forms.GroupBox()
        Me.VscrlUsers = New System.Windows.Forms.VScrollBar()
        Me.CbxUsers = New System.Windows.Forms.ComboBox()
        Me.CbxCustomer = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BtnApplyAtt = New System.Windows.Forms.Button()
        Me.CbxAttributes = New System.Windows.Forms.ComboBox()
        Me.LbxFeatures = New System.Windows.Forms.ListBox()
        Me.TxtComment = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.CbxCompany = New System.Windows.Forms.ComboBox()
        Me.CbxPostal = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.CbxCity = New System.Windows.Forms.ComboBox()
        Me.CbxState = New System.Windows.Forms.ComboBox()
        Me.CbxCountry = New System.Windows.Forms.ComboBox()
        Me.TxtAddress = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtPhone = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.BtnUpdateLic = New System.Windows.Forms.Button()
        Me.BtnNewLic = New System.Windows.Forms.Button()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.TxtSearch = New System.Windows.Forms.TextBox()
        Me.GpbUserDetails = New System.Windows.Forms.GroupBox()
        Me.BtnExportKeys = New System.Windows.Forms.Button()
        Me.BtnExportLic = New System.Windows.Forms.Button()
        Me.GpbClients = New System.Windows.Forms.GroupBox()
        Me.GpbValidation.SuspendLayout()
        Me.GpbLicDetails.SuspendLayout()
        Me.GpbUserDetails.SuspendLayout()
        Me.GpbClients.SuspendLayout()
        Me.SuspendLayout()
        '
        'LbxClients
        '
        Me.LbxClients.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.LbxClients.FormattingEnabled = True
        Me.LbxClients.Location = New System.Drawing.Point(10, 62)
        Me.LbxClients.Name = "LbxClients"
        Me.LbxClients.Size = New System.Drawing.Size(144, 290)
        Me.LbxClients.TabIndex = 0
        '
        'BtnCreateLic
        '
        Me.BtnCreateLic.Location = New System.Drawing.Point(537, 523)
        Me.BtnCreateLic.Name = "BtnCreateLic"
        Me.BtnCreateLic.Size = New System.Drawing.Size(91, 24)
        Me.BtnCreateLic.TabIndex = 50
        Me.BtnCreateLic.Text = "Create License"
        Me.BtnCreateLic.UseVisualStyleBackColor = True
        '
        'GpbValidation
        '
        Me.GpbValidation.Controls.Add(Me.TxtValidation)
        Me.GpbValidation.Location = New System.Drawing.Point(12, 380)
        Me.GpbValidation.Name = "GpbValidation"
        Me.GpbValidation.Size = New System.Drawing.Size(714, 137)
        Me.GpbValidation.TabIndex = 15
        Me.GpbValidation.TabStop = False
        Me.GpbValidation.Text = "Validation Messages"
        '
        'TxtValidation
        '
        Me.TxtValidation.Location = New System.Drawing.Point(10, 19)
        Me.TxtValidation.Multiline = True
        Me.TxtValidation.Name = "TxtValidation"
        Me.TxtValidation.Size = New System.Drawing.Size(697, 112)
        Me.TxtValidation.TabIndex = 40
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 23)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Customer:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 51)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(39, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "E-Mail:"
        '
        'TxtEMail
        '
        Me.TxtEMail.Location = New System.Drawing.Point(69, 48)
        Me.TxtEMail.Name = "TxtEMail"
        Me.TxtEMail.Size = New System.Drawing.Size(472, 20)
        Me.TxtEMail.TabIndex = 2
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(9, 126)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(60, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Max Users:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(9, 160)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(80, 13)
        Me.Label5.TabIndex = 50
        Me.Label5.Text = "Attribute Name:"
        '
        'TxtAttributeValue
        '
        Me.TxtAttributeValue.Location = New System.Drawing.Point(316, 156)
        Me.TxtAttributeValue.Name = "TxtAttributeValue"
        Me.TxtAttributeValue.Size = New System.Drawing.Size(175, 20)
        Me.TxtAttributeValue.TabIndex = 15
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(237, 160)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(79, 13)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Attribute Value:"
        '
        'CbxLicenseType
        '
        Me.CbxLicenseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbxLicenseType.FormattingEnabled = True
        Me.CbxLicenseType.Items.AddRange(New Object() {"Trial", "Standard"})
        Me.CbxLicenseType.Location = New System.Drawing.Point(316, 74)
        Me.CbxLicenseType.Name = "CbxLicenseType"
        Me.CbxLicenseType.Size = New System.Drawing.Size(225, 21)
        Me.CbxLicenseType.TabIndex = 12
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(236, 79)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(74, 13)
        Me.Label12.TabIndex = 13
        Me.Label12.Text = "License Type:"
        '
        'DtpExpiration
        '
        Me.DtpExpiration.Location = New System.Drawing.Point(316, 101)
        Me.DtpExpiration.Name = "DtpExpiration"
        Me.DtpExpiration.Size = New System.Drawing.Size(225, 20)
        Me.DtpExpiration.TabIndex = 14
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(254, 102)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(56, 13)
        Me.Label14.TabIndex = 15
        Me.Label14.Text = "Expires at:"
        '
        'GpbLicDetails
        '
        Me.GpbLicDetails.Controls.Add(Me.VscrlUsers)
        Me.GpbLicDetails.Controls.Add(Me.CbxUsers)
        Me.GpbLicDetails.Controls.Add(Me.CbxCustomer)
        Me.GpbLicDetails.Controls.Add(Me.Label1)
        Me.GpbLicDetails.Controls.Add(Me.BtnApplyAtt)
        Me.GpbLicDetails.Controls.Add(Me.CbxAttributes)
        Me.GpbLicDetails.Controls.Add(Me.LbxFeatures)
        Me.GpbLicDetails.Controls.Add(Me.Label14)
        Me.GpbLicDetails.Controls.Add(Me.DtpExpiration)
        Me.GpbLicDetails.Controls.Add(Me.Label12)
        Me.GpbLicDetails.Controls.Add(Me.CbxLicenseType)
        Me.GpbLicDetails.Controls.Add(Me.Label6)
        Me.GpbLicDetails.Controls.Add(Me.TxtAttributeValue)
        Me.GpbLicDetails.Controls.Add(Me.Label5)
        Me.GpbLicDetails.Controls.Add(Me.Label4)
        Me.GpbLicDetails.Controls.Add(Me.TxtEMail)
        Me.GpbLicDetails.Controls.Add(Me.Label3)
        Me.GpbLicDetails.Controls.Add(Me.Label2)
        Me.GpbLicDetails.Location = New System.Drawing.Point(178, 12)
        Me.GpbLicDetails.Name = "GpbLicDetails"
        Me.GpbLicDetails.Size = New System.Drawing.Size(547, 188)
        Me.GpbLicDetails.TabIndex = 16
        Me.GpbLicDetails.TabStop = False
        Me.GpbLicDetails.Text = "License Details"
        '
        'VscrlUsers
        '
        Me.VscrlUsers.Location = New System.Drawing.Point(204, 123)
        Me.VscrlUsers.Name = "VscrlUsers"
        Me.VscrlUsers.Size = New System.Drawing.Size(26, 21)
        Me.VscrlUsers.TabIndex = 52
        '
        'CbxUsers
        '
        Me.CbxUsers.FormattingEnabled = True
        Me.CbxUsers.Items.AddRange(New Object() {"1", "5", "15", "30", "50", "100"})
        Me.CbxUsers.Location = New System.Drawing.Point(70, 123)
        Me.CbxUsers.Name = "CbxUsers"
        Me.CbxUsers.Size = New System.Drawing.Size(131, 21)
        Me.CbxUsers.TabIndex = 51
        '
        'CbxCustomer
        '
        Me.CbxCustomer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CbxCustomer.FormattingEnabled = True
        Me.CbxCustomer.Location = New System.Drawing.Point(69, 21)
        Me.CbxCustomer.Name = "CbxCustomer"
        Me.CbxCustomer.Size = New System.Drawing.Size(472, 21)
        Me.CbxCustomer.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 82)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 13)
        Me.Label1.TabIndex = 20
        Me.Label1.Text = "Features"
        '
        'BtnApplyAtt
        '
        Me.BtnApplyAtt.Location = New System.Drawing.Point(497, 155)
        Me.BtnApplyAtt.Name = "BtnApplyAtt"
        Me.BtnApplyAtt.Size = New System.Drawing.Size(44, 21)
        Me.BtnApplyAtt.TabIndex = 18
        Me.BtnApplyAtt.Text = "Apply"
        Me.BtnApplyAtt.UseVisualStyleBackColor = True
        '
        'CbxAttributes
        '
        Me.CbxAttributes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbxAttributes.FormattingEnabled = True
        Me.CbxAttributes.Location = New System.Drawing.Point(96, 156)
        Me.CbxAttributes.Name = "CbxAttributes"
        Me.CbxAttributes.Size = New System.Drawing.Size(134, 21)
        Me.CbxAttributes.TabIndex = 19
        '
        'LbxFeatures
        '
        Me.LbxFeatures.FormattingEnabled = True
        Me.LbxFeatures.Location = New System.Drawing.Point(70, 74)
        Me.LbxFeatures.Name = "LbxFeatures"
        Me.LbxFeatures.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.LbxFeatures.Size = New System.Drawing.Size(160, 43)
        Me.LbxFeatures.TabIndex = 18
        '
        'TxtComment
        '
        Me.TxtComment.Location = New System.Drawing.Point(68, 132)
        Me.TxtComment.Name = "TxtComment"
        Me.TxtComment.Size = New System.Drawing.Size(472, 20)
        Me.TxtComment.TabIndex = 11
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(8, 135)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(54, 13)
        Me.Label16.TabIndex = 37
        Me.Label16.Text = "Comment:"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(8, 108)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(54, 13)
        Me.Label15.TabIndex = 36
        Me.Label15.Text = "Company:"
        '
        'CbxCompany
        '
        Me.CbxCompany.FormattingEnabled = True
        Me.CbxCompany.Location = New System.Drawing.Point(68, 105)
        Me.CbxCompany.Name = "CbxCompany"
        Me.CbxCompany.Size = New System.Drawing.Size(207, 21)
        Me.CbxCompany.TabIndex = 10
        '
        'CbxPostal
        '
        Me.CbxPostal.FormattingEnabled = True
        Me.CbxPostal.Location = New System.Drawing.Point(329, 77)
        Me.CbxPostal.Name = "CbxPostal"
        Me.CbxPostal.Size = New System.Drawing.Size(211, 21)
        Me.CbxPostal.TabIndex = 9
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(281, 80)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(39, 13)
        Me.Label13.TabIndex = 33
        Me.Label13.Text = "Postal:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(9, 54)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(27, 13)
        Me.Label11.TabIndex = 32
        Me.Label11.Text = "City:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(9, 27)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(46, 13)
        Me.Label10.TabIndex = 31
        Me.Label10.Text = "Country:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(281, 27)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(35, 13)
        Me.Label9.TabIndex = 30
        Me.Label9.Text = "State:"
        '
        'CbxCity
        '
        Me.CbxCity.FormattingEnabled = True
        Me.CbxCity.Location = New System.Drawing.Point(68, 51)
        Me.CbxCity.Name = "CbxCity"
        Me.CbxCity.Size = New System.Drawing.Size(207, 21)
        Me.CbxCity.TabIndex = 6
        '
        'CbxState
        '
        Me.CbxState.FormattingEnabled = True
        Me.CbxState.Location = New System.Drawing.Point(329, 24)
        Me.CbxState.Name = "CbxState"
        Me.CbxState.Size = New System.Drawing.Size(211, 21)
        Me.CbxState.TabIndex = 5
        '
        'CbxCountry
        '
        Me.CbxCountry.FormattingEnabled = True
        Me.CbxCountry.Location = New System.Drawing.Point(68, 24)
        Me.CbxCountry.Name = "CbxCountry"
        Me.CbxCountry.Size = New System.Drawing.Size(207, 21)
        Me.CbxCountry.TabIndex = 4
        '
        'TxtAddress
        '
        Me.TxtAddress.Location = New System.Drawing.Point(329, 51)
        Me.TxtAddress.Name = "TxtAddress"
        Me.TxtAddress.Size = New System.Drawing.Size(211, 20)
        Me.TxtAddress.TabIndex = 7
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(281, 54)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(48, 13)
        Me.Label8.TabIndex = 25
        Me.Label8.Text = "Address:"
        '
        'TxtPhone
        '
        Me.TxtPhone.Location = New System.Drawing.Point(68, 78)
        Me.TxtPhone.Name = "TxtPhone"
        Me.TxtPhone.Size = New System.Drawing.Size(207, 20)
        Me.TxtPhone.TabIndex = 8
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(9, 81)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(41, 13)
        Me.Label7.TabIndex = 21
        Me.Label7.Text = "Phone:"
        '
        'BtnUpdateLic
        '
        Me.BtnUpdateLic.Location = New System.Drawing.Point(634, 523)
        Me.BtnUpdateLic.Name = "BtnUpdateLic"
        Me.BtnUpdateLic.Size = New System.Drawing.Size(91, 24)
        Me.BtnUpdateLic.TabIndex = 17
        Me.BtnUpdateLic.Text = "Update License"
        Me.BtnUpdateLic.UseVisualStyleBackColor = True
        '
        'BtnNewLic
        '
        Me.BtnNewLic.Location = New System.Drawing.Point(440, 523)
        Me.BtnNewLic.Name = "BtnNewLic"
        Me.BtnNewLic.Size = New System.Drawing.Size(91, 24)
        Me.BtnNewLic.TabIndex = 18
        Me.BtnNewLic.Text = "New License"
        Me.BtnNewLic.UseVisualStyleBackColor = True
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(6, 20)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(44, 13)
        Me.Label17.TabIndex = 20
        Me.Label17.Text = "Search:"
        '
        'TxtSearch
        '
        Me.TxtSearch.Location = New System.Drawing.Point(9, 36)
        Me.TxtSearch.Name = "TxtSearch"
        Me.TxtSearch.Size = New System.Drawing.Size(145, 20)
        Me.TxtSearch.TabIndex = 19
        '
        'GpbUserDetails
        '
        Me.GpbUserDetails.Controls.Add(Me.CbxState)
        Me.GpbUserDetails.Controls.Add(Me.TxtComment)
        Me.GpbUserDetails.Controls.Add(Me.TxtAddress)
        Me.GpbUserDetails.Controls.Add(Me.Label15)
        Me.GpbUserDetails.Controls.Add(Me.CbxCompany)
        Me.GpbUserDetails.Controls.Add(Me.Label8)
        Me.GpbUserDetails.Controls.Add(Me.Label16)
        Me.GpbUserDetails.Controls.Add(Me.CbxCountry)
        Me.GpbUserDetails.Controls.Add(Me.TxtPhone)
        Me.GpbUserDetails.Controls.Add(Me.Label7)
        Me.GpbUserDetails.Controls.Add(Me.CbxCity)
        Me.GpbUserDetails.Controls.Add(Me.Label9)
        Me.GpbUserDetails.Controls.Add(Me.Label10)
        Me.GpbUserDetails.Controls.Add(Me.CbxPostal)
        Me.GpbUserDetails.Controls.Add(Me.Label11)
        Me.GpbUserDetails.Controls.Add(Me.Label13)
        Me.GpbUserDetails.Location = New System.Drawing.Point(179, 206)
        Me.GpbUserDetails.Name = "GpbUserDetails"
        Me.GpbUserDetails.Size = New System.Drawing.Size(546, 168)
        Me.GpbUserDetails.TabIndex = 39
        Me.GpbUserDetails.TabStop = False
        Me.GpbUserDetails.Text = "User Details"
        '
        'BtnExportKeys
        '
        Me.BtnExportKeys.Location = New System.Drawing.Point(12, 523)
        Me.BtnExportKeys.Name = "BtnExportKeys"
        Me.BtnExportKeys.Size = New System.Drawing.Size(147, 24)
        Me.BtnExportKeys.TabIndex = 40
        Me.BtnExportKeys.Text = "Export Private/Public Keys"
        Me.BtnExportKeys.UseVisualStyleBackColor = True
        '
        'BtnExportLic
        '
        Me.BtnExportLic.Location = New System.Drawing.Point(343, 523)
        Me.BtnExportLic.Name = "BtnExportLic"
        Me.BtnExportLic.Size = New System.Drawing.Size(91, 24)
        Me.BtnExportLic.TabIndex = 41
        Me.BtnExportLic.Text = "Export License"
        Me.BtnExportLic.UseVisualStyleBackColor = True
        '
        'GpbClients
        '
        Me.GpbClients.Controls.Add(Me.TxtSearch)
        Me.GpbClients.Controls.Add(Me.Label17)
        Me.GpbClients.Controls.Add(Me.LbxClients)
        Me.GpbClients.Location = New System.Drawing.Point(12, 12)
        Me.GpbClients.Name = "GpbClients"
        Me.GpbClients.Size = New System.Drawing.Size(160, 362)
        Me.GpbClients.TabIndex = 42
        Me.GpbClients.TabStop = False
        Me.GpbClients.Text = "Clients"
        '
        'LicenseViewfrm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(734, 558)
        Me.Controls.Add(Me.GpbClients)
        Me.Controls.Add(Me.BtnExportLic)
        Me.Controls.Add(Me.BtnExportKeys)
        Me.Controls.Add(Me.GpbUserDetails)
        Me.Controls.Add(Me.BtnNewLic)
        Me.Controls.Add(Me.BtnCreateLic)
        Me.Controls.Add(Me.BtnUpdateLic)
        Me.Controls.Add(Me.GpbLicDetails)
        Me.Controls.Add(Me.GpbValidation)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "LicenseViewfrm"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "LicenseView"
        Me.GpbValidation.ResumeLayout(False)
        Me.GpbValidation.PerformLayout()
        Me.GpbLicDetails.ResumeLayout(False)
        Me.GpbLicDetails.PerformLayout()
        Me.GpbUserDetails.ResumeLayout(False)
        Me.GpbUserDetails.PerformLayout()
        Me.GpbClients.ResumeLayout(False)
        Me.GpbClients.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LbxClients As ListBox
    Friend WithEvents BtnCreateLic As Button
    Friend WithEvents GpbValidation As GroupBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtEMail As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents TxtAttributeValue As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents CbxLicenseType As ComboBox
    Friend WithEvents Label12 As Label
    Friend WithEvents DtpExpiration As DateTimePicker
    Friend WithEvents Label14 As Label
    Friend WithEvents GpbLicDetails As GroupBox
    Friend WithEvents BtnUpdateLic As Button
    Friend WithEvents LbxFeatures As ListBox
    Friend WithEvents CbxAttributes As ComboBox
    Friend WithEvents BtnApplyAtt As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents TxtAddress As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents TxtPhone As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents CbxCity As ComboBox
    Friend WithEvents CbxState As ComboBox
    Friend WithEvents CbxCountry As ComboBox
    Friend WithEvents Label11 As Label
    Friend WithEvents CbxPostal As ComboBox
    Friend WithEvents Label13 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents CbxCompany As ComboBox
    Friend WithEvents TxtComment As TextBox
    Friend WithEvents Label16 As Label
    Friend WithEvents BtnNewLic As Button
    Friend WithEvents Label17 As Label
    Friend WithEvents TxtSearch As TextBox
    Friend WithEvents GpbUserDetails As GroupBox
    Friend WithEvents TxtValidation As TextBox
    Friend WithEvents BtnExportKeys As Button
    Friend WithEvents BtnExportLic As Button
    Friend WithEvents GpbClients As GroupBox
    Friend WithEvents CbxCustomer As ComboBox
    Friend WithEvents VscrlUsers As VScrollBar
    Friend WithEvents CbxUsers As ComboBox
End Class
