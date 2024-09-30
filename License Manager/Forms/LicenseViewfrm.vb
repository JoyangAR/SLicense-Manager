Imports System.Data.SQLite
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports Standard.Licensing

Public Class LicenseViewfrm
    Private ULicense As License
    Private PrivateKey As String
    Private PublicKey As String
    Dim LicenseAttributesDictionary As Dictionary(Of String, String)
    Dim ProductAttributesDictionary As Dictionary(Of String, String)
    Dim ProductFeatureDictionary As New Dictionary(Of String, String)
    Private AllProductClients As New List(Of Object)
    Public CurrentProductID As Integer
    Private Sub Mainfrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PublicKey = LoadPublicKey(CurrentProductID)           ' Loads cryptographic keys for the selected product from the database.
        PrivateKey = LoadPrivateKey(CurrentProductID)
        LoadClientNames()                ' Populates a list or control with names of clients from the database.
        LoadFeaturesIntoLbxFeatures(CurrentProductID) ' Loads features for the selected product into a ListBox.
        InitializeProductFeatures()      ' Sets up UI components related to product features.
        FillComboBoxWithAttributes()     ' Fills a ComboBox with attribute names from the database.
        LoadClientData()                 ' Loads detailed client data into a DataGridView or another data display control.
        LoadClientsIntoComboBox()        ' Populates a ComboBox with client information for selection or filtering.
        LoadUserPermission()             ' Loads permissions for the current user and adjusts the UI accordingly.
        CbxUsers.SelectedIndex = 0
        VscrlUsers.Minimum = 1
        VscrlUsers.Maximum = 10000
        VscrlUsers.Value = 1
        CbxLicenseType.SelectedIndex = 0
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
        ' Clear the ListBox before loading new features
        LbxFeatures.Items.Clear()

        ' Get the list of features for the given productID
        Dim features = GetFeaturesList(productID)

        ' Add each feature to the ListBox
        For Each feature In features
            LbxFeatures.Items.Add(feature)
        Next
    End Sub

    Private Sub LoadClientNames()
        ' Clear and load client names associated with the selected product
        LbxClients.Items.Clear()
        AllProductClients.Clear()

        Dim query As String = "SELECT DISTINCT Clients.ClientID, Clients.Name FROM Clients JOIN Licenses ON Clients.ClientID = Licenses.ClientID WHERE Licenses.ProductID = @ProductID;"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", CurrentProductID)
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
        ' Stop license creation if user does not have permission
        If Not isAdmin Then
            If canOnlyTrials Then
                If CbxLicenseType.Text <> "Trial" Then
                    MessageBox.Show("Permission is set for Only Trials")
                    Return
                Else
                    If (DtpExpiration.Value.Date - Date.Today).Days > canMaxDays Then
                        MessageBox.Show($"Permission is set for a max days of {canMaxDays}")
                        Return
                    End If
                End If
            End If
        End If
        ' Obtain ClientID and check if a license already exists
        Dim clientID As Integer = FindAndCreateClient(CbxCustomer.Text, TxtEMail.Text)

        If LicenseExists(clientID, CurrentProductID) Then
            MessageBox.Show("A license for this client and product already exists.", "License Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Retrieve the password for the selected product
        Dim productpassword As String = GetProductPasswordByID(CurrentProductID)
        Debug.WriteLine(productpassword)
        ' Generate a new LicenseID
        Dim licenseId As Guid = Guid.NewGuid

        ' Create the license with specified parameters
        Try
            ULicense = License.[New]() _
            .WithUniqueIdentifier(licenseId) _
            .As(GetLicenseType(CbxLicenseType.Text)) _
            .WithMaximumUtilization(CbxUsers.Text) _
            .WithAdditionalAttributes(LicenseAttributesDictionary) _
            .WithProductFeatures(ProductFeatureDictionary) _
            .LicensedTo(CbxCustomer.Text, TxtEMail.Text) _
            .ExpiresAt(CDate(DtpExpiration.Text)) _
            .CreateAndSignWithPrivateKey(Me.PrivateKey, productpassword)
        Catch ex As Exception
            MessageBox.Show("Error signing the license: " & ex.Message)
            Return
        End Try

        ' Convert Guid to bytes and then to Base64
        Dim licenseIdBytes As Byte() = licenseId.ToByteArray()
        Dim base64LicenseId As String = Convert.ToBase64String(licenseIdBytes)
        base64LicenseId = base64LicenseId.Replace("/", "_").Replace("+", "-")  ' Adjust for filename compatibility

        ' Prepare the license file name
        Dim licenseFileName As String = Path.Combine(LicenseDirectory, base64LicenseId + ".lic")

        ' Save the license to a file
        File.WriteAllText(licenseFileName, ULicense.ToString(), Encoding.UTF8)

        ' Save license info in the database or required location
        SaveLicenseInfoOnDatabase(licenseId.ToString(), clientID, base64LicenseId, CDate(DtpExpiration.Text), CurrentProductID)

        ' Update client information if needed
        UpdateClientsInfo(clientID)

        ' Refresh client list
        LoadClientNames()
    End Sub

    Private Sub FillComboBoxWithAttributes()
        ' Load attribute dictionary for the selected product
        ProductAttributesDictionary = GetAttributesDictionary(CurrentProductID)

        ' Clear existing items and fill the ComboBox with attribute names
        CbxAttributes.Items.Clear()
        For Each attributeName As String In ProductAttributesDictionary.Keys
            CbxAttributes.Items.Add(attributeName)
        Next

        ' Set the first item as the selected item
        If CbxAttributes.Items.Count > 0 Then
            CbxAttributes.SelectedIndex = 0  ' Esto seleccionará automáticamente el primer atributo
        End If
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
        Dim licenseID As String = GetLicenseIDByClientID(clientID, CurrentProductID)
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
                LicenseAttributesDictionary = ULicense.AdditionalAttributes.GetAll()
                CbxLicenseType.Text = ULicense.Type.ToString
                ProductFeatureDictionary = ULicense.ProductFeatures.GetAll()
                DtpExpiration.Value = ULicense.Expiration
                ' Populate features list and select corresponding items
                SelectFeatures()
                TxtValidation.Text = ValidateLicense(ULicense, PublicKey).ToString
            End If
        Catch Ex As Exception
            MessageBox.Show("Cannot read file from disk. Original error: " & Ex.Message)
        Finally
            ' Ensure the stream is closed even if an exception is thrown
            If myStream IsNot Nothing Then
                myStream.Close()
            End If
        End Try
        ' Check if an attribute is selected in the ComboBox
        If CbxAttributes.SelectedIndex <> -1 Then
            Dim selectedAttribute As String = CbxAttributes.SelectedItem.ToString()
            If LicenseAttributesDictionary.ContainsKey(selectedAttribute) Then
                TxtAttributeValue.Text = LicenseAttributesDictionary(selectedAttribute) ' Display the value in the TextBox
            Else
                TxtAttributeValue.Text = String.Empty 'Clear the TextBox or set a default value
                LicenseAttributesDictionary = GetAttributesDictionary(CurrentProductID)
            End If
        End If
    End Sub

    ' Loads client details into the form fields based on the clientID.
    Private Sub LoadClientDetails(clientID As Integer)
        ' Get the client data using the existing function.
        Dim clientDetails As List(Of EditableKeyValuePair) = GetClientDataByClientID(clientID)

        ' Check if client details were retrieved
        If clientDetails.Count > 0 Then
            ' Update the UI fields based on the retrieved key-value pairs
            For Each detail As EditableKeyValuePair In clientDetails
                Select Case detail.Key
                    Case "Email"
                        TxtEMail.Text = detail.Value.ToString()
                    Case "Country"
                        CbxCountry.Text = detail.Value.ToString()
                    Case "City"
                        CbxCity.Text = detail.Value.ToString()
                    Case "State"
                        CbxState.Text = detail.Value.ToString()
                    Case "Postal"
                        CbxPostal.Text = detail.Value.ToString()
                    Case "Phone"
                        TxtPhone.Text = detail.Value.ToString()
                    Case "Company"
                        CbxCompany.Text = detail.Value.ToString()
                    Case "Address"
                        TxtAddress.Text = detail.Value.ToString()
                    Case "Comment"
                        TxtComment.Text = detail.Value.ToString()
                End Select
            Next
        Else
            ' Show a message if no details are found.
            MessageBox.Show("No details found for the selected client.")
        End If
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
            If ProductAttributesDictionary.ContainsKey(selectedAttribute) Then
                TxtAttributeValue.Text = ProductAttributesDictionary(selectedAttribute) ' Display the value in the TextBox
            Else
                ' Handle the case where the attribute is not found in the dictionary
                TxtAttributeValue.Text = String.Empty ' Clear the TextBox or set a default value
            End If
        End If
    End Sub

    Private Sub BtnApplyAtt_Click(sender As Object, e As EventArgs) Handles BtnApplyAtt.Click
        ' Apply the new value to the selected attribute
        If CbxAttributes.SelectedIndex <> -1 Then
            Dim selectedAttribute As String = CbxAttributes.SelectedItem.ToString()
            Dim newValue As String = TxtAttributeValue.Text ' Get the new value from the TextBox
            ' Update the dictionary
            If LicenseAttributesDictionary.ContainsKey(selectedAttribute) Then
                LicenseAttributesDictionary(selectedAttribute) = newValue
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
        ' Stop license update if user does not have permission
        If Not isAdmin Then
            If canOnlyTrials Then
                If CbxLicenseType.Text <> "Trial" Then
                    MessageBox.Show("Permission is set for Only Trials")
                    Return
                Else
                    If (DtpExpiration.Value.Date - Date.Today).Days > canMaxDays Then
                        MessageBox.Show($"Permission is set for a max days of {canMaxDays}")
                        Return
                    End If
                End If
            End If
        End If
        ' Get ClientID and check if a license already exists for this client and product
        Dim clientID As Integer = FindAndCreateClient(CbxCustomer.Text, TxtEMail.Text)

        If LicenseExists(clientID, CurrentProductID) Then
            ' Get the corresponding password for the selected ProductID
            Dim productpassword As String = GetProductPasswordByID(CurrentProductID)

            ' Generate a new LicenseID
            Dim licenseId As Guid = Guid.NewGuid

            ' Create the license with specified parameters
            Try
                ULicense = License.[New]() _
                .WithUniqueIdentifier(licenseId) _
                .As(GetLicenseType(CbxLicenseType.Text)) _
                .WithMaximumUtilization(CbxUsers.Text) _
                .WithAdditionalAttributes(LicenseAttributesDictionary) _
                .WithProductFeatures(ProductFeatureDictionary) _
                .LicensedTo(CbxCustomer.Text, TxtEMail.Text) _
                .ExpiresAt(CDate(DtpExpiration.Text)) _
                .CreateAndSignWithPrivateKey(Me.PrivateKey, productpassword)
            Catch ex As Exception
                MessageBox.Show("Error signing the license: " & ex.Message)
                Return
            End Try

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
            UpdateLicenseInfoOnDatabase(licenseId.ToString(), clientID, base64LicenseId, CDate(DtpExpiration.Text), CurrentProductID)

            UpdateClientsInfo(clientID)

            LoadClientNames()

            MessageBox.Show("License Updated Successfully.")
        End If
    End Sub

    Private Sub CbxUsers_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CbxUsers.KeyPress
        ' Allow only numbers, keyboard controls and backspace
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso e.KeyChar <> ChrW(Keys.Back) Then
            e.Handled = True  ' Ignora el caracter si no es número, control o backspace
        End If

        ' Prevent first character from being 0
        If CbxUsers.Text.Length = 0 AndAlso e.KeyChar = "0"c Then
            e.Handled = True
        End If

        ' Prevent leaving the combobox empty
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

    Private Sub CbxUsers_TextChanged(sender As Object, e As EventArgs) Handles CbxUsers.TextChanged
        Dim currentValue As Integer
        If Integer.TryParse(CbxUsers.Text, currentValue) Then
            ' Refresh scrolvar's value
            VscrlUsers.Value = Math.Max(VscrlUsers.Minimum, Math.Min(VscrlUsers.Maximum, currentValue))
        End If
    End Sub

    ' Updates client information in the database based on the provided client ID.
    Private Sub UpdateClientsInfo(clientId As Integer)
        ' Create a list to hold updated client data as key-value pairs.
        Dim updatedData As New List(Of EditableKeyValuePair) From {
        New EditableKeyValuePair("Name", CbxCustomer.Text),         ' Client's name from combo box
        New EditableKeyValuePair("Email", TxtEMail.Text),         ' Client's email from text box
        New EditableKeyValuePair("Country", CbxCountry.Text),     ' Client's country from combo box
        New EditableKeyValuePair("State", CbxState.Text),         ' Client's state from combo box
        New EditableKeyValuePair("City", CbxCity.Text),           ' Client's city from combo box
        New EditableKeyValuePair("Address", TxtAddress.Text),     ' Client's address from text box
        New EditableKeyValuePair("Postal", CbxPostal.Text),       ' Client's postal code from combo box
        New EditableKeyValuePair("Phone", RemoveNonNumericCharacters(TxtPhone.Text)), ' Clean phone number
        New EditableKeyValuePair("Company", CbxCompany.Text),     ' Client's company from combo box
        New EditableKeyValuePair("Comment", TxtComment.Text)      ' Client's comment from text box
    }

        ' Call the UpdateClient function to perform the update
        UpdateClient(updatedData, clientId)
    End Sub

    Private Sub BtnNewLic_Click(sender As Object, e As EventArgs) Handles BtnNewLic.Click
        ClearFormControls(Me)
        FillComboBoxWithAttributes()
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
        CbxLicenseType.SelectedIndex = 0
        CbxUsers.Text = "1"
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

        If LbxClients.SelectedIndex = -1 Then
            MessageBox.Show("Please select a client from the list.")
            Return
        End If

        Dim selectedClient = DirectCast(LbxClients.SelectedItem, Object)
        Dim clientID As Integer = selectedClient.ID
        Dim clientName As String = selectedClient.Name
        If clientID = -1 Then
            MessageBox.Show("Client ID not found.")
            Return
        End If

        Dim licenseID As String = GetLicenseIDByClientID(clientID, CurrentProductID)
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
                System.IO.File.Copy(licenseFile & ".lic", sfd.FileName, True)
                MessageBox.Show("License exported successfully.")
            Else
                MessageBox.Show("Export cancelled.")
            End If
        End Using
    End Sub

    ' Loads clients into the ComboBox from the database.
    Private Sub LoadClientsIntoComboBox()
        ' Get the list of clients from the database
        Dim clients = GetClients()

        ' Clear the ComboBox before filling it
        CbxCustomer.Items.Clear()

        ' Populate the ComboBox with clients
        For Each client In clients
            CbxCustomer.Items.Add(New ComboBoxItem(client.ID, client.Name)) ' Add each client to the ComboBox
        Next
    End Sub


    Private Sub CbxCustomer_DrawItem(sender As Object, e As DrawItemEventArgs) Handles CbxCustomer.DrawItem
        e.DrawBackground()
        If e.Index >= 0 Then
            Dim item = DirectCast(CbxCustomer.Items(e.Index), ComboBoxItem)
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