Imports System.Data.SQLite
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports Standard.Licensing

Public Class LicenseViewfrm
    Dim AttributesDictionary As Dictionary(Of String, String)
    Dim ProductFeatureDictionary As New Dictionary(Of String, String)
    Private AllProductClients As New List(Of Object)
    Private Sub Mainfrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadKeysFromDatabase()           ' Loads cryptographic keys for the selected product from the database.
        LoadClientNames()                ' Populates a list or control with names of clients from the database.
        LoadFeaturesIntoLbxFeatures(SelectedProductID) ' Loads features for the selected product into a ListBox.
        InitializeProductFeatures()      ' Sets up UI components related to product features.
        FillComboBoxWithAttributes()     ' Fills a ComboBox with attribute names from the database.
        LoadClientData()                 ' Loads detailed client data into a DataGridView or another data display control.
        LoadClientsIntoComboBox()        ' Populates a ComboBox with client information for selection or filtering.
        LoadUserPermission()             ' Loads permissions for the current user and adjusts the UI accordingly.
        CbxUsers.SelectedIndex = 0
        ' Configuración inicial del VScrollBar
        VscrlUsers.Minimum = 1
        VscrlUsers.Maximum = 10000  ' Ajusta esto según el rango que esperas
        VscrlUsers.Value = 1
    End Sub


    Sub LoadUserPermission()
        ' Set button enabled state based on user permissions and admin status
        BtnApplyAtt.Enabled = canEditLics OrElse isAdmin
        BtnNewLic.Enabled = canEditClients AndAlso canEditLics OrElse isAdmin
        BtnUpdateLic.Enabled = canEditClients AndAlso canEditLics OrElse isAdmin
        BtnCreateLic.Enabled = canEditClients AndAlso canEditLics OrElse isAdmin
        BtnExportKeys.Enabled = canExportKeys OrElse isAdmin
        BtnExportLic.Enabled = canExportLics OrElse isAdmin
    End Sub

    Private Sub LoadFeaturesIntoLbxFeatures(productID As Integer)
        ' Load features associated with the selected product ID
        Dim query As String = "SELECT FeatureName FROM Features WHERE ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    LbxFeatures.Items.Add(reader("FeatureName").ToString())
                End While
            End Using
        End Using
    End Sub

    Private Sub LoadClientNames()
        ' Clear and load client names associated with the selected product
        LbxClients.Items.Clear()
        AllProductClients.Clear()

        Dim query As String = "SELECT DISTINCT Clients.ClientID, Clients.Name FROM Clients JOIN Licenses ON Clients.ClientID = Licenses.ClientID WHERE Licenses.ProductID = @ProductID;"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", SelectedProductID)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim clientID As Integer = Convert.ToInt32(reader("ClientID"))
                    Dim clientName As String = reader("Name").ToString()
                    Dim client As New With {.Name = clientName, .ID = clientID}
                    LbxClients.Items.Add(client)
                    AllProductClients.Add(client)  ' Add to global list for other operations
                End While
            End Using
        End Using
    End Sub

    Private Sub BtnCreateLic_Click(sender As Object, e As EventArgs) Handles BtnCreateLic.Click
        If String.IsNullOrEmpty(CbxCustomer.Text) Then
            MessageBox.Show("Customer cannot be empty", "License Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Obtain ClientID and check if a license already exists
        Dim clientID As Integer = FindAndCreateClient(CbxCustomer.Text, TxtEMail.Text)

            If LicenseExists(clientID, SelectedProductID) Then
                MessageBox.Show("A license for this client and product already exists.", "License Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Retrieve the password for the selected product
            Dim password As String = GetProductPassword(SelectedProductID)

            ' Generate a new LicenseID
            Dim licenseId As Guid = Guid.NewGuid

            ' Create the license with specified parameters
            ULicense = License.[New].WithUniqueIdentifier(licenseId).As(GetLicenseType(CbxLicenseType.Text)).WithMaximumUtilization(CbxUsers.Text).WithAdditionalAttributes(AttributesDictionary).WithProductFeatures(ProductFeatureDictionary).LicensedTo(CbxCustomer.Text, TxtEMail.Text).ExpiresAt(CDate(DtpExpiration.Text)).CreateAndSignWithPrivateKey(PrivateKey, password)

            ' Convert Guid to bytes and then to Base64
            Dim licenseIdBytes As Byte() = licenseId.ToByteArray()
            Dim base64LicenseId As String = Convert.ToBase64String(licenseIdBytes)
            base64LicenseId = base64LicenseId.Replace("/", "_").Replace("+", "-")  ' Adjust for filename compatibility

            ' Prepare the license file name
            Dim licenseFileName As String = Path.Combine(LicenseDirectory, base64LicenseId + ".lic")

            ' Save the license to a file
            File.WriteAllText(licenseFileName, ULicense.ToString(), Encoding.UTF8)

            ' Save license info in the database or required location
            SaveLicenseInfo(licenseId.ToString(), clientID, base64LicenseId, CDate(DtpExpiration.Text))

            ' Update client information if needed
            UpdateClientsInfo(clientID)

            ' Refresh client list
            LoadClientNames()
    End Sub



    Private Sub FillComboBoxWithAttributes()
        ' Load attribute dictionary for the selected product
        AttributesDictionary = GetAttributesDictionary(SelectedProductID)

        ' Clear existing items and fill the ComboBox with attribute names
        CbxAttributes.Items.Clear()
        For Each attributeName As String In AttributesDictionary.Keys
            CbxAttributes.Items.Add(attributeName)
        Next
    End Sub

    Private Sub LoadClientData()
        ' Dictionary linking ComboBoxes with corresponding database columns for dynamic loading
        Dim comboBoxesAndColumns As New Dictionary(Of ComboBox, String) From {
        {CbxCountry, "Country"},
        {CbxCity, "City"},
        {CbxState, "State"},
        {CbxCompany, "Company"},
        {CbxPostal, "Postal"}
    }

        ' Iterate through each ComboBox, querying and loading data from the specified column
        For Each pair In comboBoxesAndColumns
            Dim comboBox As ComboBox = pair.Key
            Dim column As String = pair.Value

            ' Query to get distinct values from the specified column, sorted alphabetically
            Dim query As String = $"SELECT DISTINCT {column} FROM Clients ORDER BY {column};"

            Using cmd As New SQLiteCommand(query, conn)
                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    comboBox.Items.Clear() ' Clear ComboBox before filling

                    While reader.Read()
                        If Not reader.IsDBNull(0) Then ' Ensure the value is not DBNULL
                            comboBox.Items.Add(reader.GetString(0))
                        End If
                    End While
                End Using
            End Using
        Next
    End Sub

    Private Sub LbxClients_DrawItem(sender As Object, e As DrawItemEventArgs) Handles LbxClients.DrawItem
        ' Custom draw method for ListBox items
        e.DrawBackground()
        If e.Index >= 0 Then
            ' Assume items are dynamic objects with 'Name' and 'ID'
            Dim item = DirectCast(LbxClients.Items(e.Index), Object)
            e.Graphics.DrawString(item.Name, e.Font, Brushes.Black, e.Bounds)
        End If
        e.DrawFocusRectangle()  ' Highlight the selected item
    End Sub

    Private Sub LbxClients_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LbxClients.SelectedIndexChanged
        ' Return immediately if no client is selected
        If LbxClients.SelectedIndex = -1 Then Return

        ' Retrieve selected client details
        Dim selectedClient = DirectCast(LbxClients.SelectedItem, Object)
        Dim clientID As Integer = selectedClient.ID

        ' Display error if client ID is not found (sanity check)
        If clientID = -1 Then
            MessageBox.Show("Client ID not found.")
            Return
        End If

        ' Get the license ID using the client ID
        Dim licenseID As String = GetLicenseIDByClientID(clientID)
        If String.IsNullOrEmpty(licenseID) Then
            MessageBox.Show("License ID not found for the selected client.")
            Return
        End If

        ' Construct the full path to the license file
        Dim licenseFile As String = Path.Combine(LicenseDirectory, GetLicenseFileByLicenseID(licenseID) & ".lic")
        If String.IsNullOrEmpty(licenseFile) Then
            MessageBox.Show("License file not found for the given license ID.")
        End If

        ' Load detailed client information from the database
        LoadClientDetails(clientID)

        ' Load the license file and handle file operations
        Dim myStream As Stream = Nothing
        Try
            myStream = File.OpenRead(licenseFile)
            If myStream IsNot Nothing Then
                ULicense = Standard.Licensing.License.Load(myStream)

                ' Update UI with license details
                CbxCustomer.Text = ULicense.Customer.Name.ToString
                TxtEMail.Text = ULicense.Customer.Email.ToString
                CbxUsers.Text = ULicense.Quantity.ToString
                AttributesDictionary = ULicense.AdditionalAttributes.GetAll()
                CbxLicenseType.Text = ULicense.Type.ToString
                ProductFeatureDictionary = ULicense.ProductFeatures.GetAll()

                ' Populate features list and select corresponding items
                SelectFeatures()
                TxtValidation.Text = ValidateLicense(ULicense).ToString
            End If
        Catch Ex As Exception
            MessageBox.Show("Cannot read file from disk. Original error: " & Ex.Message)
        Finally
            ' Ensure the stream is closed even if an exception is thrown
            If myStream IsNot Nothing Then
                myStream.Close()
            End If
        End Try
    End Sub

    Private Sub LoadClientDetails(clientID As Integer)
        ' Query to retrieve specific client details
        Dim query As String = "SELECT Email, Country, City, State, Postal, Phone, Company, Address, Comment FROM Clients WHERE ClientID = @ClientID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ClientID", clientID)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Update UI with client details from the database
                    CbxCountry.Text = reader("Country").ToString()
                    TxtEMail.Text = reader("Email").ToString()
                    CbxCity.Text = reader("City").ToString()
                    CbxState.Text = reader("State").ToString()
                    CbxPostal.Text = reader("Postal").ToString()
                    TxtPhone.Text = reader("Phone").ToString()
                    CbxCompany.Text = reader("Company").ToString()
                    TxtAddress.Text = reader("Address").ToString()
                    TxtComment.Text = reader("Comment").ToString()
                Else
                    MessageBox.Show("No details found for the selected client.")
                End If
            End Using
        End Using
    End Sub

    Private Sub SelectFeatures()
        ' Prepare to select features from the loaded product features dictionary
        Dim itemsToSelect As New List(Of String)
        For Each item As String In LbxFeatures.Items
            If ProductFeatureDictionary.ContainsKey(item) AndAlso Boolean.Parse(ProductFeatureDictionary(item)) Then
                itemsToSelect.Add(item)
            End If
        Next

        ' Clear previous selections
        LbxFeatures.ClearSelected()

        ' Select items that are found in the product features dictionary
        For Each itemToSelect As String In itemsToSelect
            Dim index As Integer = LbxFeatures.Items.IndexOf(itemToSelect)
            If index >= 0 Then
                LbxFeatures.SetSelected(index, True)
            End If
        Next
    End Sub


    Private Sub CbxAttributes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbxAttributes.SelectedIndexChanged
        ' Check if an attribute is selected in the ComboBox
        If CbxAttributes.SelectedIndex <> -1 Then
            Dim selectedAttribute As String = CbxAttributes.SelectedItem.ToString()
            TxtAttributeValue.Text = AttributesDictionary(selectedAttribute) ' Display the value in the TextBox
        End If
    End Sub

    Private Sub BtnApplyAtt_Click(sender As Object, e As EventArgs) Handles BtnApplyAtt.Click
        ' Apply the new value to the selected attribute
        If CbxAttributes.SelectedIndex <> -1 Then
            Dim selectedAttribute As String = CbxAttributes.SelectedItem.ToString()
            Dim newValue As String = TxtAttributeValue.Text ' Get the new value from the TextBox
            ' Update the dictionary
            If AttributesDictionary.ContainsKey(selectedAttribute) Then
                AttributesDictionary(selectedAttribute) = newValue
            End If
            MessageBox.Show("Value updated successfully.")
        Else
            MessageBox.Show("Please select an attribute.")
        End If
    End Sub

    Private Sub LbxFeatures_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LbxFeatures.SelectedIndexChanged
        ' Temporarily create a list with copies of the current items
        Dim itemList As New List(Of String)(LbxFeatures.Items.Cast(Of String)())

        ' Iterate over the temporary list
        For Each item As String In itemList
            ' Check if the current item is in the selected items
            Dim isSelected As Boolean = LbxFeatures.SelectedItems.Contains(item)
            ' Update the dictionary with the current selection state
            If ProductFeatureDictionary.ContainsKey(item) Then
                ProductFeatureDictionary(item) = isSelected
            Else
                ProductFeatureDictionary.Add(item, isSelected)
            End If
        Next
    End Sub

    Private Sub InitializeProductFeatures()
        ' Assuming LbxFeatures is already filled with appropriate items
        For Each item As String In LbxFeatures.Items
            If Not ProductFeatureDictionary.ContainsKey(item) Then
                ProductFeatureDictionary.Add(item, False)  ' All items initially unselected
            End If
        Next
    End Sub


    Private Sub BtnUpdateLic_Click(sender As Object, e As EventArgs) Handles BtnUpdateLic.Click
        If String.IsNullOrEmpty(CbxCustomer.Text) Then
            MessageBox.Show("Customer cannot be empty", "License Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Get ClientID and check if a license already exists for this client and product
        Dim clientID As Integer = FindAndCreateClient(CbxCustomer.Text, TxtEMail.Text)

        If LicenseExists(clientID, SelectedProductID) Then
            ' Get the corresponding password for the selected ProductID
            Dim password As String = GetProductPassword(SelectedProductID)

            ' Generate a new LicenseID
            Dim licenseId As Guid = Guid.NewGuid

            ' Create the license
            ULicense = License.[New].WithUniqueIdentifier(licenseId).As(GetLicenseType(CbxLicenseType.Text)).WithMaximumUtilization(CbxUsers.Text).WithAdditionalAttributes(AttributesDictionary).WithProductFeatures(ProductFeatureDictionary).LicensedTo(CbxCustomer.Text, TxtEMail.Text).ExpiresAt(CDate(DtpExpiration.Text)).CreateAndSignWithPrivateKey(PrivateKey, password)

            ' Convert Guid to bytes and then to Base64
            Dim licenseIdBytes As Byte() = licenseId.ToByteArray()
            Dim base64LicenseId As String = Convert.ToBase64String(licenseIdBytes)

            ' Replace invalid characters for file names
            base64LicenseId = base64LicenseId.Replace("/", "_").Replace("+", "-")

            ' Prepare the license file name
            Dim licenseFileName As String = Path.Combine(LicenseDirectory, base64LicenseId + ".lic")

            ' Save the license in a file directly in the specified path
            File.WriteAllText(licenseFileName, ULicense.ToString(), Encoding.UTF8)

            ' Save license information in the database or wherever necessary
            SaveLicenseInfo(licenseId.ToString(), clientID, base64LicenseId, CDate(DtpExpiration.Text))

            UpdateClientsInfo(clientID)

            LoadClientNames()

            MessageBox.Show("License Updated Successfully.")
        End If
    End Sub

    Private Sub cmbUsers_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CbxUsers.KeyPress
        ' Allow only numbers, keyboard controls and backspace
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso e.KeyChar <> ChrW(Keys.Back) Then
            e.Handled = True  ' Ignora el caracter si no es número, control o backspace
        End If

        ' Prevenir first character from being 0
        If CbxUsers.Text.Length = 0 AndAlso e.KeyChar = "0"c Then
            e.Handled = True
        End If

        ' Prevenir leaving the combobox empty
        If e.KeyChar = ChrW(Keys.Back) AndAlso CbxUsers.Text.Length = 1 Then
            e.Handled = True
        End If
    End Sub
    Private Sub VscrlUsers_Scroll(sender As Object, e As ScrollEventArgs) Handles VscrlUsers.Scroll
        If CbxUsers.SelectedIndex <> -1 Or CbxUsers.Text.Length > 0 Then
            Dim currentValue As Integer
            If Integer.TryParse(CbxUsers.Text, currentValue) Then
                Select Case e.Type
                    Case ScrollEventType.SmallIncrement
                        currentValue -= 1  ' Decrement
                        If currentValue < 1 Then currentValue = 1 ' Asure it's not less than 1
                    Case ScrollEventType.SmallDecrement
                        currentValue += 1  ' Increment
                End Select
                CbxUsers.Text = currentValue.ToString()
                VscrlUsers.Value = currentValue ' Sincronizes scrollvar's value with combobox's value
            End If
        End If
    End Sub

    Private Sub CmbUsers_TextChanged(sender As Object, e As EventArgs) Handles CbxUsers.TextChanged
        Dim currentValue As Integer
        If Integer.TryParse(CbxUsers.Text, currentValue) Then
            ' Refresh scrolvar's value
            VscrlUsers.Value = Math.Max(VscrlUsers.Minimum, Math.Min(VscrlUsers.Maximum, currentValue))
        End If
    End Sub

    Private Sub UpdateClientsInfo(clientId As Integer)
        ' Update client details in the database
        Dim queryClients As String = "UPDATE Clients SET Name = @ClientName, Email = @ClientEmail, Country = @ClientCountry, State = @ClientState, City = @ClientCity, Address = @ClientAddress, Postal = @ClientPostal, Phone = @ClientPhone, Company = @ClientCompany, Comment = @ClientComment  Where ClientID = @ClientID;"
        Using cmd As New SQLiteCommand(queryClients, conn)
            cmd.Parameters.AddWithValue("@ClientID", clientId)
            cmd.Parameters.AddWithValue("@ClientName", CbxCustomer.Text)
            cmd.Parameters.AddWithValue("@ClientEmail", TxtEMail.Text)
            cmd.Parameters.AddWithValue("@ClientCountry", CbxCountry.Text)
            cmd.Parameters.AddWithValue("@ClientState", CbxState.Text)
            cmd.Parameters.AddWithValue("@ClientCity", CbxCity.Text)
            cmd.Parameters.AddWithValue("@ClientAddress", TxtAddress.Text)
            cmd.Parameters.AddWithValue("@ClientPostal", CbxPostal.Text)
            cmd.Parameters.AddWithValue("@ClientPhone", RemoveNonNumericCharacters(TxtPhone.Text))
            cmd.Parameters.AddWithValue("@ClientCompany", CbxCompany.Text)
            cmd.Parameters.AddWithValue("@ClientComment", TxtComment.Text)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub BtnNewLic_Click(sender As Object, e As EventArgs) Handles BtnNewLic.Click
        ClearFormControls(Me)
    End Sub

    Private Sub ClearFormControls(parentControl As Control)
        ' Recursively clear child controls if the current control contains other controls
        For Each ctrl As Control In parentControl.Controls
            If TypeOf ctrl Is TextBox Then
                DirectCast(ctrl, TextBox).Clear()
            ElseIf TypeOf ctrl Is ComboBox Then
                Dim comboBox As ComboBox = DirectCast(ctrl, ComboBox)
                comboBox.SelectedIndex = -1  ' This sets that there is no selection in the ComboBox
            End If

            ' Recursively clear child controls if the current control contains other controls
            If ctrl.HasChildren Then
                ClearFormControls(ctrl)
            End If
        Next
    End Sub


    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles TxtSearch.TextChanged
        Dim searchQuery As String = TxtSearch.Text.ToLower()
        LbxClients.Items.Clear()  ' Clear the ListBox before filtering

        ' Filter and add only the elements that match the search text
        For Each client In AllProductClients
            If client.Name.ToLower().Contains(searchQuery) Then
                LbxClients.Items.Add(client)
            End If
        Next
    End Sub

    Private Sub BtnExportKeys_Click(sender As Object, e As EventArgs) Handles BtnExportKeys.Click
        Using sfd As New SaveFileDialog()
            sfd.Title = "Save Private Key"
            sfd.Filter = "PEM files (*.pem)|*.pem|All files (*.*)|*.*"
            sfd.DefaultExt = "pem"
            sfd.AddExtension = True

            ' Prompt the user to select a location to save the private key
            If sfd.ShowDialog() = DialogResult.OK Then
                ' Save the private key at the selected location
                System.IO.File.WriteAllText(sfd.FileName, PrivateKey)

                ' Prompt location for the public key
                sfd.Title = "Save Public Key"
                If sfd.ShowDialog() = DialogResult.OK Then
                    ' Save the public key at the selected location
                    System.IO.File.WriteAllText(sfd.FileName, PublicKey)
                    MessageBox.Show("Keys exported successfully.")
                End If
            Else
                MessageBox.Show("Export cancelled.")
            End If
        End Using
    End Sub

    Private Sub BtnExportLic_Click(sender As Object, e As EventArgs) Handles BtnExportLic.Click
        If LbxClients.SelectedIndex = -1 Then Return

        Dim selectedClient = DirectCast(LbxClients.SelectedItem, Object)
        Dim clientID As Integer = selectedClient.ID
        Dim clientName As String = selectedClient.Name
        If clientID = -1 Then
            MessageBox.Show("Client ID not found.")
            Return
        End If

        Dim licenseID As String = GetLicenseIDByClientID(clientID)
        If String.IsNullOrEmpty(licenseID) Then
            MessageBox.Show("License ID not found for the selected client.")
            Return
        End If

        Dim licenseFile As String = Path.Combine(LicenseDirectory, GetLicenseFileByLicenseID(licenseID))
        If String.IsNullOrEmpty(licenseFile) Then
            MessageBox.Show("License file not found for the given license ID.")
            Return
        End If
        Using sfd As New SaveFileDialog()
            sfd.Title = "Save License"
            sfd.Filter = "LIC files (*.lic)|*.lic|All files (*.*)|*.*"

            ' Prompt the user to select a location to save the license
            If sfd.ShowDialog() = DialogResult.OK Then
                ' Save the license file at the selected location
                System.IO.File.Copy(licenseFile, sfd.FileName, True)
                MessageBox.Show("License exported successfully.")
            Else
                MessageBox.Show("Export cancelled.")
            End If
        End Using
    End Sub

    Private Sub LoadClientsIntoComboBox()
        Dim query As String = "SELECT ClientID, Name FROM Clients ORDER BY Name"
        Using cmd As New SQLiteCommand(query, conn)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                CbxCustomer.Items.Clear()  ' Clear the ComboBox before filling it
                While reader.Read()
                    Dim clientId As Integer = Convert.ToInt32(reader("ClientID"))
                    Dim clientName As String = reader("Name").ToString()
                    CbxCustomer.Items.Add(New ComboBoxItem(clientId, clientName))
                End While
            End Using
        End Using
    End Sub

    Private Sub CbxCustomer_DrawItem(sender As Object, e As DrawItemEventArgs) Handles CbxCustomer.DrawItem
        e.DrawBackground()
        If e.Index >= 0 Then
            Dim item = DirectCast(CbxCustomer.Items(e.Index), ComboBoxItem)  ' Assume that you have added the object as shown previously
            e.Graphics.DrawString(item.Name, e.Font, Brushes.Black, e.Bounds)
        End If
        e.DrawFocusRectangle()
    End Sub

    Private Sub CbxCustomer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbxCustomer.SelectedIndexChanged
        If CbxCustomer.SelectedIndex <> -1 Then
            Dim selectedClient As ComboBoxItem = CType(CbxCustomer.SelectedItem, ComboBoxItem)
            LoadClientDetails(selectedClient.ID)
        End If
    End Sub
End Class