<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Mainfrm
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
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.ChkPermissionExportKeys = New System.Windows.Forms.CheckBox()
        Me.ChkPermissionExportLics = New System.Windows.Forms.CheckBox()
        Me.ChkPermissionEditLics = New System.Windows.Forms.CheckBox()
        Me.BtnApplyPermission = New System.Windows.Forms.Button()
        Me.ChkPermissionDeleteProducts = New System.Windows.Forms.CheckBox()
        Me.ChkPermissionDeleteClients = New System.Windows.Forms.CheckBox()
        Me.ChkPermissionCreateEditProducts = New System.Windows.Forms.CheckBox()
        Me.ChkPermissionCreateEditClients = New System.Windows.Forms.CheckBox()
        Me.ChkPermissionAdmin = New System.Windows.Forms.CheckBox()
        Me.GpbUsers = New System.Windows.Forms.GroupBox()
        Me.BtnAddUser = New System.Windows.Forms.Button()
        Me.BtnDeleteUser = New System.Windows.Forms.Button()
        Me.BtnChangePassword = New System.Windows.Forms.Button()
        Me.LbxUsers = New System.Windows.Forms.ListBox()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.GpbOptionals = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.GpbFeatAndAtt = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.BtnAttributeDelete = New System.Windows.Forms.Button()
        Me.BtnAttributeAdd = New System.Windows.Forms.Button()
        Me.LbxAttributes = New System.Windows.Forms.ListBox()
        Me.BtnFeatureDelete = New System.Windows.Forms.Button()
        Me.BtnFeatureAdd = New System.Windows.Forms.Button()
        Me.LbxFeatures = New System.Windows.Forms.ListBox()
        Me.GpbProducts = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtSearch = New System.Windows.Forms.TextBox()
        Me.BtnViewLicenses = New System.Windows.Forms.Button()
        Me.LbxProducts = New System.Windows.Forms.ListBox()
        Me.BtnRemoveProduct = New System.Windows.Forms.Button()
        Me.BtnAddProduct = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.BtnDeleteClient = New System.Windows.Forms.Button()
        Me.BtnNewClient = New System.Windows.Forms.Button()
        Me.BtnSaveClientChanges = New System.Windows.Forms.Button()
        Me.LbxClients = New System.Windows.Forms.ListBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbxFilters = New System.Windows.Forms.ComboBox()
        Me.ClientsDataGrid = New System.Windows.Forms.DataGridView()
        Me.TxtClientSearch = New System.Windows.Forms.TextBox()
        Me.TabPage2.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GpbUsers.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GpbOptionals.SuspendLayout()
        Me.GpbFeatAndAtt.SuspendLayout()
        Me.GpbProducts.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.ClientsDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabPage2
        '
        Me.TabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TabPage2.Controls.Add(Me.GroupBox4)
        Me.TabPage2.Controls.Add(Me.GpbUsers)
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(598, 404)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Settings"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox4.Controls.Add(Me.ChkPermissionExportKeys)
        Me.GroupBox4.Controls.Add(Me.ChkPermissionExportLics)
        Me.GroupBox4.Controls.Add(Me.ChkPermissionEditLics)
        Me.GroupBox4.Controls.Add(Me.BtnApplyPermission)
        Me.GroupBox4.Controls.Add(Me.ChkPermissionDeleteProducts)
        Me.GroupBox4.Controls.Add(Me.ChkPermissionDeleteClients)
        Me.GroupBox4.Controls.Add(Me.ChkPermissionCreateEditProducts)
        Me.GroupBox4.Controls.Add(Me.ChkPermissionCreateEditClients)
        Me.GroupBox4.Controls.Add(Me.ChkPermissionAdmin)
        Me.GroupBox4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox4.Location = New System.Drawing.Point(295, 6)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox4.Size = New System.Drawing.Size(295, 176)
        Me.GroupBox4.TabIndex = 12
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "User Permissions"
        '
        'ChkPermissionExportKeys
        '
        Me.ChkPermissionExportKeys.AutoSize = True
        Me.ChkPermissionExportKeys.Location = New System.Drawing.Point(172, 47)
        Me.ChkPermissionExportKeys.Name = "ChkPermissionExportKeys"
        Me.ChkPermissionExportKeys.Size = New System.Drawing.Size(82, 17)
        Me.ChkPermissionExportKeys.TabIndex = 15
        Me.ChkPermissionExportKeys.Text = "Export Keys"
        Me.ChkPermissionExportKeys.UseVisualStyleBackColor = True
        '
        'ChkPermissionExportLics
        '
        Me.ChkPermissionExportLics.AutoSize = True
        Me.ChkPermissionExportLics.Location = New System.Drawing.Point(172, 24)
        Me.ChkPermissionExportLics.Name = "ChkPermissionExportLics"
        Me.ChkPermissionExportLics.Size = New System.Drawing.Size(101, 17)
        Me.ChkPermissionExportLics.TabIndex = 14
        Me.ChkPermissionExportLics.Text = "Export Licenses"
        Me.ChkPermissionExportLics.UseVisualStyleBackColor = True
        '
        'ChkPermissionEditLics
        '
        Me.ChkPermissionEditLics.AutoSize = True
        Me.ChkPermissionEditLics.Location = New System.Drawing.Point(26, 141)
        Me.ChkPermissionEditLics.Name = "ChkPermissionEditLics"
        Me.ChkPermissionEditLics.Size = New System.Drawing.Size(89, 17)
        Me.ChkPermissionEditLics.TabIndex = 13
        Me.ChkPermissionEditLics.Text = "Edit Licenses"
        Me.ChkPermissionEditLics.UseVisualStyleBackColor = True
        '
        'BtnApplyPermission
        '
        Me.BtnApplyPermission.BackColor = System.Drawing.SystemColors.Control
        Me.BtnApplyPermission.Cursor = System.Windows.Forms.Cursors.Default
        Me.BtnApplyPermission.ForeColor = System.Drawing.SystemColors.ControlText
        Me.BtnApplyPermission.Location = New System.Drawing.Point(192, 133)
        Me.BtnApplyPermission.Name = "BtnApplyPermission"
        Me.BtnApplyPermission.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.BtnApplyPermission.Size = New System.Drawing.Size(81, 25)
        Me.BtnApplyPermission.TabIndex = 12
        Me.BtnApplyPermission.Text = "Apply"
        Me.BtnApplyPermission.UseVisualStyleBackColor = True
        '
        'ChkPermissionDeleteProducts
        '
        Me.ChkPermissionDeleteProducts.AutoSize = True
        Me.ChkPermissionDeleteProducts.Location = New System.Drawing.Point(26, 119)
        Me.ChkPermissionDeleteProducts.Name = "ChkPermissionDeleteProducts"
        Me.ChkPermissionDeleteProducts.Size = New System.Drawing.Size(102, 17)
        Me.ChkPermissionDeleteProducts.TabIndex = 4
        Me.ChkPermissionDeleteProducts.Text = "Delete Products"
        Me.ChkPermissionDeleteProducts.UseVisualStyleBackColor = True
        '
        'ChkPermissionDeleteClients
        '
        Me.ChkPermissionDeleteClients.AutoSize = True
        Me.ChkPermissionDeleteClients.Location = New System.Drawing.Point(26, 96)
        Me.ChkPermissionDeleteClients.Name = "ChkPermissionDeleteClients"
        Me.ChkPermissionDeleteClients.Size = New System.Drawing.Size(91, 17)
        Me.ChkPermissionDeleteClients.TabIndex = 3
        Me.ChkPermissionDeleteClients.Text = "Delete Clients"
        Me.ChkPermissionDeleteClients.UseVisualStyleBackColor = True
        '
        'ChkPermissionCreateEditProducts
        '
        Me.ChkPermissionCreateEditProducts.AutoSize = True
        Me.ChkPermissionCreateEditProducts.Location = New System.Drawing.Point(26, 73)
        Me.ChkPermissionCreateEditProducts.Name = "ChkPermissionCreateEditProducts"
        Me.ChkPermissionCreateEditProducts.Size = New System.Drawing.Size(125, 17)
        Me.ChkPermissionCreateEditProducts.TabIndex = 2
        Me.ChkPermissionCreateEditProducts.Text = "Create/Edit Products"
        Me.ChkPermissionCreateEditProducts.UseVisualStyleBackColor = True
        '
        'ChkPermissionCreateEditClients
        '
        Me.ChkPermissionCreateEditClients.AutoSize = True
        Me.ChkPermissionCreateEditClients.Location = New System.Drawing.Point(26, 50)
        Me.ChkPermissionCreateEditClients.Name = "ChkPermissionCreateEditClients"
        Me.ChkPermissionCreateEditClients.Size = New System.Drawing.Size(135, 17)
        Me.ChkPermissionCreateEditClients.TabIndex = 1
        Me.ChkPermissionCreateEditClients.Text = "Create/Edit Client Data"
        Me.ChkPermissionCreateEditClients.UseVisualStyleBackColor = True
        '
        'ChkPermissionAdmin
        '
        Me.ChkPermissionAdmin.AutoSize = True
        Me.ChkPermissionAdmin.Location = New System.Drawing.Point(26, 27)
        Me.ChkPermissionAdmin.Name = "ChkPermissionAdmin"
        Me.ChkPermissionAdmin.Size = New System.Drawing.Size(86, 17)
        Me.ChkPermissionAdmin.TabIndex = 0
        Me.ChkPermissionAdmin.Text = "Administrator"
        Me.ChkPermissionAdmin.UseVisualStyleBackColor = True
        '
        'GpbUsers
        '
        Me.GpbUsers.BackColor = System.Drawing.SystemColors.Control
        Me.GpbUsers.Controls.Add(Me.BtnAddUser)
        Me.GpbUsers.Controls.Add(Me.BtnDeleteUser)
        Me.GpbUsers.Controls.Add(Me.BtnChangePassword)
        Me.GpbUsers.Controls.Add(Me.LbxUsers)
        Me.GpbUsers.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GpbUsers.Location = New System.Drawing.Point(8, 6)
        Me.GpbUsers.Name = "GpbUsers"
        Me.GpbUsers.Padding = New System.Windows.Forms.Padding(0)
        Me.GpbUsers.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GpbUsers.Size = New System.Drawing.Size(281, 176)
        Me.GpbUsers.TabIndex = 5
        Me.GpbUsers.TabStop = False
        Me.GpbUsers.Text = "Users"
        '
        'BtnAddUser
        '
        Me.BtnAddUser.BackColor = System.Drawing.SystemColors.Control
        Me.BtnAddUser.Cursor = System.Windows.Forms.Cursors.Default
        Me.BtnAddUser.ForeColor = System.Drawing.SystemColors.ControlText
        Me.BtnAddUser.Location = New System.Drawing.Point(184, 24)
        Me.BtnAddUser.Name = "BtnAddUser"
        Me.BtnAddUser.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.BtnAddUser.Size = New System.Drawing.Size(81, 25)
        Me.BtnAddUser.TabIndex = 11
        Me.BtnAddUser.Text = "Add user"
        Me.BtnAddUser.UseVisualStyleBackColor = True
        '
        'BtnDeleteUser
        '
        Me.BtnDeleteUser.BackColor = System.Drawing.SystemColors.Control
        Me.BtnDeleteUser.Cursor = System.Windows.Forms.Cursors.Default
        Me.BtnDeleteUser.ForeColor = System.Drawing.SystemColors.ControlText
        Me.BtnDeleteUser.Location = New System.Drawing.Point(184, 88)
        Me.BtnDeleteUser.Name = "BtnDeleteUser"
        Me.BtnDeleteUser.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.BtnDeleteUser.Size = New System.Drawing.Size(81, 25)
        Me.BtnDeleteUser.TabIndex = 3
        Me.BtnDeleteUser.Text = "Delete"
        Me.BtnDeleteUser.UseVisualStyleBackColor = True
        '
        'BtnChangePassword
        '
        Me.BtnChangePassword.BackColor = System.Drawing.SystemColors.Control
        Me.BtnChangePassword.Cursor = System.Windows.Forms.Cursors.Default
        Me.BtnChangePassword.ForeColor = System.Drawing.SystemColors.ControlText
        Me.BtnChangePassword.Location = New System.Drawing.Point(184, 56)
        Me.BtnChangePassword.Name = "BtnChangePassword"
        Me.BtnChangePassword.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.BtnChangePassword.Size = New System.Drawing.Size(81, 25)
        Me.BtnChangePassword.TabIndex = 2
        Me.BtnChangePassword.Text = "Change pwd"
        Me.BtnChangePassword.UseVisualStyleBackColor = True
        '
        'LbxUsers
        '
        Me.LbxUsers.BackColor = System.Drawing.SystemColors.Window
        Me.LbxUsers.Cursor = System.Windows.Forms.Cursors.Default
        Me.LbxUsers.ForeColor = System.Drawing.SystemColors.WindowText
        Me.LbxUsers.Location = New System.Drawing.Point(8, 24)
        Me.LbxUsers.Name = "LbxUsers"
        Me.LbxUsers.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.LbxUsers.Size = New System.Drawing.Size(169, 134)
        Me.LbxUsers.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TabPage1.Controls.Add(Me.GpbOptionals)
        Me.TabPage1.Controls.Add(Me.GpbFeatAndAtt)
        Me.TabPage1.Controls.Add(Me.GpbProducts)
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(598, 404)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Products"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'GpbOptionals
        '
        Me.GpbOptionals.Controls.Add(Me.Label6)
        Me.GpbOptionals.Location = New System.Drawing.Point(264, 6)
        Me.GpbOptionals.Name = "GpbOptionals"
        Me.GpbOptionals.Size = New System.Drawing.Size(326, 73)
        Me.GpbOptionals.TabIndex = 6
        Me.GpbOptionals.TabStop = False
        Me.GpbOptionals.Text = "Optionals"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(135, 32)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(36, 13)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Empty"
        '
        'GpbFeatAndAtt
        '
        Me.GpbFeatAndAtt.Controls.Add(Me.Label1)
        Me.GpbFeatAndAtt.Controls.Add(Me.Label4)
        Me.GpbFeatAndAtt.Controls.Add(Me.BtnAttributeDelete)
        Me.GpbFeatAndAtt.Controls.Add(Me.BtnAttributeAdd)
        Me.GpbFeatAndAtt.Controls.Add(Me.LbxAttributes)
        Me.GpbFeatAndAtt.Controls.Add(Me.BtnFeatureDelete)
        Me.GpbFeatAndAtt.Controls.Add(Me.BtnFeatureAdd)
        Me.GpbFeatAndAtt.Controls.Add(Me.LbxFeatures)
        Me.GpbFeatAndAtt.Location = New System.Drawing.Point(263, 85)
        Me.GpbFeatAndAtt.Name = "GpbFeatAndAtt"
        Me.GpbFeatAndAtt.Size = New System.Drawing.Size(326, 311)
        Me.GpbFeatAndAtt.TabIndex = 5
        Me.GpbFeatAndAtt.TabStop = False
        Me.GpbFeatAndAtt.Text = "Features / Attributes"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(164, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(91, 13)
        Me.Label1.TabIndex = 17
        Me.Label1.Text = "Aditional Atributes"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(3, 28)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 13)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "Features"
        '
        'BtnAttributeDelete
        '
        Me.BtnAttributeDelete.Location = New System.Drawing.Point(221, 247)
        Me.BtnAttributeDelete.Name = "BtnAttributeDelete"
        Me.BtnAttributeDelete.Size = New System.Drawing.Size(48, 41)
        Me.BtnAttributeDelete.TabIndex = 15
        Me.BtnAttributeDelete.Text = "Delete"
        Me.BtnAttributeDelete.UseVisualStyleBackColor = True
        '
        'BtnAttributeAdd
        '
        Me.BtnAttributeAdd.Location = New System.Drawing.Point(167, 247)
        Me.BtnAttributeAdd.Name = "BtnAttributeAdd"
        Me.BtnAttributeAdd.Size = New System.Drawing.Size(48, 42)
        Me.BtnAttributeAdd.TabIndex = 14
        Me.BtnAttributeAdd.Text = "Add"
        Me.BtnAttributeAdd.UseVisualStyleBackColor = True
        '
        'LbxAttributes
        '
        Me.LbxAttributes.FormattingEnabled = True
        Me.LbxAttributes.Location = New System.Drawing.Point(167, 44)
        Me.LbxAttributes.Name = "LbxAttributes"
        Me.LbxAttributes.Size = New System.Drawing.Size(153, 199)
        Me.LbxAttributes.TabIndex = 13
        '
        'BtnFeatureDelete
        '
        Me.BtnFeatureDelete.Location = New System.Drawing.Point(60, 247)
        Me.BtnFeatureDelete.Name = "BtnFeatureDelete"
        Me.BtnFeatureDelete.Size = New System.Drawing.Size(48, 42)
        Me.BtnFeatureDelete.TabIndex = 12
        Me.BtnFeatureDelete.Text = "Delete"
        Me.BtnFeatureDelete.UseVisualStyleBackColor = True
        '
        'BtnFeatureAdd
        '
        Me.BtnFeatureAdd.Location = New System.Drawing.Point(6, 247)
        Me.BtnFeatureAdd.Name = "BtnFeatureAdd"
        Me.BtnFeatureAdd.Size = New System.Drawing.Size(48, 42)
        Me.BtnFeatureAdd.TabIndex = 11
        Me.BtnFeatureAdd.Text = "Add"
        Me.BtnFeatureAdd.UseVisualStyleBackColor = True
        '
        'LbxFeatures
        '
        Me.LbxFeatures.FormattingEnabled = True
        Me.LbxFeatures.Location = New System.Drawing.Point(6, 44)
        Me.LbxFeatures.Name = "LbxFeatures"
        Me.LbxFeatures.Size = New System.Drawing.Size(155, 199)
        Me.LbxFeatures.TabIndex = 10
        '
        'GpbProducts
        '
        Me.GpbProducts.Controls.Add(Me.Label2)
        Me.GpbProducts.Controls.Add(Me.TxtSearch)
        Me.GpbProducts.Controls.Add(Me.BtnViewLicenses)
        Me.GpbProducts.Controls.Add(Me.LbxProducts)
        Me.GpbProducts.Controls.Add(Me.BtnRemoveProduct)
        Me.GpbProducts.Controls.Add(Me.BtnAddProduct)
        Me.GpbProducts.Location = New System.Drawing.Point(7, 6)
        Me.GpbProducts.Name = "GpbProducts"
        Me.GpbProducts.Size = New System.Drawing.Size(250, 389)
        Me.GpbProducts.TabIndex = 4
        Me.GpbProducts.TabStop = False
        Me.GpbProducts.Text = "Products"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(44, 13)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "Search:"
        '
        'TxtSearch
        '
        Me.TxtSearch.Location = New System.Drawing.Point(6, 32)
        Me.TxtSearch.Name = "TxtSearch"
        Me.TxtSearch.Size = New System.Drawing.Size(137, 20)
        Me.TxtSearch.TabIndex = 9
        '
        'BtnViewLicenses
        '
        Me.BtnViewLicenses.Location = New System.Drawing.Point(149, 58)
        Me.BtnViewLicenses.Name = "BtnViewLicenses"
        Me.BtnViewLicenses.Size = New System.Drawing.Size(87, 40)
        Me.BtnViewLicenses.TabIndex = 4
        Me.BtnViewLicenses.Text = "View Licenses"
        Me.BtnViewLicenses.UseVisualStyleBackColor = True
        '
        'LbxProducts
        '
        Me.LbxProducts.FormattingEnabled = True
        Me.LbxProducts.Location = New System.Drawing.Point(6, 58)
        Me.LbxProducts.Name = "LbxProducts"
        Me.LbxProducts.Size = New System.Drawing.Size(137, 316)
        Me.LbxProducts.TabIndex = 1
        '
        'BtnRemoveProduct
        '
        Me.BtnRemoveProduct.Location = New System.Drawing.Point(149, 150)
        Me.BtnRemoveProduct.Name = "BtnRemoveProduct"
        Me.BtnRemoveProduct.Size = New System.Drawing.Size(87, 40)
        Me.BtnRemoveProduct.TabIndex = 3
        Me.BtnRemoveProduct.Text = "Remove Product"
        Me.BtnRemoveProduct.UseVisualStyleBackColor = True
        '
        'BtnAddProduct
        '
        Me.BtnAddProduct.Location = New System.Drawing.Point(149, 104)
        Me.BtnAddProduct.Name = "BtnAddProduct"
        Me.BtnAddProduct.Size = New System.Drawing.Size(87, 40)
        Me.BtnAddProduct.TabIndex = 0
        Me.BtnAddProduct.Text = "Add Product"
        Me.BtnAddProduct.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Multiline = True
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(606, 433)
        Me.TabControl1.TabIndex = 4
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.GroupBox3)
        Me.TabPage3.Location = New System.Drawing.Point(4, 25)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(598, 404)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Clients"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.BtnDeleteClient)
        Me.GroupBox3.Controls.Add(Me.BtnNewClient)
        Me.GroupBox3.Controls.Add(Me.BtnSaveClientChanges)
        Me.GroupBox3.Controls.Add(Me.LbxClients)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.Label3)
        Me.GroupBox3.Controls.Add(Me.cbxFilters)
        Me.GroupBox3.Controls.Add(Me.ClientsDataGrid)
        Me.GroupBox3.Controls.Add(Me.TxtClientSearch)
        Me.GroupBox3.Location = New System.Drawing.Point(8, 6)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(582, 390)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Client Info"
        '
        'BtnDeleteClient
        '
        Me.BtnDeleteClient.Location = New System.Drawing.Point(164, 356)
        Me.BtnDeleteClient.Name = "BtnDeleteClient"
        Me.BtnDeleteClient.Size = New System.Drawing.Size(50, 28)
        Me.BtnDeleteClient.TabIndex = 23
        Me.BtnDeleteClient.Text = "Delete"
        Me.BtnDeleteClient.UseVisualStyleBackColor = True
        '
        'BtnNewClient
        '
        Me.BtnNewClient.Location = New System.Drawing.Point(526, 356)
        Me.BtnNewClient.Name = "BtnNewClient"
        Me.BtnNewClient.Size = New System.Drawing.Size(50, 28)
        Me.BtnNewClient.TabIndex = 22
        Me.BtnNewClient.Text = "New"
        Me.BtnNewClient.UseVisualStyleBackColor = True
        '
        'BtnSaveClientChanges
        '
        Me.BtnSaveClientChanges.Location = New System.Drawing.Point(470, 356)
        Me.BtnSaveClientChanges.Name = "BtnSaveClientChanges"
        Me.BtnSaveClientChanges.Size = New System.Drawing.Size(50, 28)
        Me.BtnSaveClientChanges.TabIndex = 21
        Me.BtnSaveClientChanges.Text = "Save"
        Me.BtnSaveClientChanges.UseVisualStyleBackColor = True
        '
        'LbxClients
        '
        Me.LbxClients.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.LbxClients.FormattingEnabled = True
        Me.LbxClients.Location = New System.Drawing.Point(0, 47)
        Me.LbxClients.Name = "LbxClients"
        Me.LbxClients.Size = New System.Drawing.Size(158, 303)
        Me.LbxClients.TabIndex = 20
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(139, 24)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(19, 13)
        Me.Label5.TabIndex = 19
        Me.Label5.Text = "By"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 24)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(41, 13)
        Me.Label3.TabIndex = 18
        Me.Label3.Text = "Search"
        '
        'cbxFilters
        '
        Me.cbxFilters.FormattingEnabled = True
        Me.cbxFilters.Location = New System.Drawing.Point(164, 20)
        Me.cbxFilters.Name = "cbxFilters"
        Me.cbxFilters.Size = New System.Drawing.Size(85, 21)
        Me.cbxFilters.TabIndex = 2
        '
        'ClientsDataGrid
        '
        Me.ClientsDataGrid.AllowUserToAddRows = False
        Me.ClientsDataGrid.AllowUserToDeleteRows = False
        Me.ClientsDataGrid.AllowUserToOrderColumns = True
        Me.ClientsDataGrid.AllowUserToResizeColumns = False
        Me.ClientsDataGrid.AllowUserToResizeRows = False
        Me.ClientsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.ClientsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.ClientsDataGrid.Location = New System.Drawing.Point(164, 47)
        Me.ClientsDataGrid.Name = "ClientsDataGrid"
        Me.ClientsDataGrid.RowHeadersVisible = False
        Me.ClientsDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.ClientsDataGrid.Size = New System.Drawing.Size(418, 303)
        Me.ClientsDataGrid.TabIndex = 0
        '
        'TxtClientSearch
        '
        Me.TxtClientSearch.Location = New System.Drawing.Point(53, 21)
        Me.TxtClientSearch.Name = "TxtClientSearch"
        Me.TxtClientSearch.Size = New System.Drawing.Size(80, 20)
        Me.TxtClientSearch.TabIndex = 1
        '
        'Mainfrm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(606, 433)
        Me.Controls.Add(Me.TabControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "Mainfrm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "License Manager"
        Me.TabPage2.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GpbUsers.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.GpbOptionals.ResumeLayout(False)
        Me.GpbOptionals.PerformLayout()
        Me.GpbFeatAndAtt.ResumeLayout(False)
        Me.GpbFeatAndAtt.PerformLayout()
        Me.GpbProducts.ResumeLayout(False)
        Me.GpbProducts.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.ClientsDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents GpbOptionals As GroupBox
    Friend WithEvents GpbFeatAndAtt As GroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents BtnAttributeDelete As Button
    Friend WithEvents BtnAttributeAdd As Button
    Friend WithEvents LbxAttributes As ListBox
    Friend WithEvents BtnFeatureDelete As Button
    Friend WithEvents BtnFeatureAdd As Button
    Friend WithEvents LbxFeatures As ListBox
    Friend WithEvents GpbProducts As GroupBox
    Friend WithEvents LbxProducts As ListBox
    Friend WithEvents BtnRemoveProduct As Button
    Friend WithEvents BtnAddProduct As Button
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents BtnViewLicenses As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtSearch As TextBox
    Public WithEvents GpbUsers As GroupBox
    Public WithEvents BtnAddUser As Button
    Public WithEvents BtnDeleteUser As Button
    Public WithEvents BtnChangePassword As Button
    Public WithEvents LbxUsers As ListBox
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents TxtClientSearch As TextBox
    Friend WithEvents ClientsDataGrid As DataGridView
    Friend WithEvents Label5 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents cbxFilters As ComboBox
    Friend WithEvents LbxClients As ListBox
    Friend WithEvents BtnSaveClientChanges As Button
    Friend WithEvents BtnNewClient As Button
    Public WithEvents GroupBox4 As GroupBox
    Friend WithEvents ChkPermissionDeleteProducts As CheckBox
    Friend WithEvents ChkPermissionDeleteClients As CheckBox
    Friend WithEvents ChkPermissionCreateEditProducts As CheckBox
    Friend WithEvents ChkPermissionCreateEditClients As CheckBox
    Friend WithEvents ChkPermissionAdmin As CheckBox
    Friend WithEvents BtnDeleteClient As Button
    Public WithEvents BtnApplyPermission As Button
    Friend WithEvents ChkPermissionExportLics As CheckBox
    Friend WithEvents ChkPermissionExportKeys As CheckBox
    Friend WithEvents ChkPermissionEditLics As CheckBox
    Friend WithEvents Label6 As Label
End Class
