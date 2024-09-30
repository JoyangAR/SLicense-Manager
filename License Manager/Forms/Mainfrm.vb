Imports System.Data.SQLite
Imports System.IO

Public Class Mainfrm

    Private dtClients As DataTable

    Private dbFileName As String = Path.Combine(AppDirectory, "StandardLM.db")
    Private connectionString As String = $"Data Source={dbFileName};Version=3;"
    Private AllProducts As New List(Of Object)
    Private AllClients As New List(Of Object)

    ' Handles the event when the MainForm is loaded.
    Private Sub Mainfrm_Load(sender As Object, e As EventArgs) Handles Me.Load
        LoadProducts()                                   ' Load all products into the ListBox.
        LoadUsersToLstUser()                             ' Load all users into the ListBox for user management.
        LoadClientNames()                                ' Load all client names into the ListBox for client management.
        InitializeFilterComboBox()                       ' Initialize the filter ComboBox with options based on client data fields.
        SetGlobalUserPermissions()                       ' Get and set user permissions for the currently signed-in user.
        UpdateControlsOnPermissions()                    ' Set Controls based on permissions for the currently signed-in user.
        Me.Text = $"License Manager User: {SignedUser}"  ' Set the form's title to include the signed-in user's name.
    End Sub

    ' Loads user names into the LbxUsers ListBox.
    Private Sub LoadUsersToLstUser()
        ' Clear any existing items in the ListBox before loading new data.
        LbxUsers.Items.Clear()

        ' Call the function to get the list of user names.
        Dim users As List(Of String) = GetUsers()

        ' Add each user name to the ListBox.
        For Each user As String In users
            LbxUsers.Items.Add(user)
        Next
    End Sub

    ' Loads client names and IDs into the LbxClients ListBox and the AllClients list.
    Private Sub LoadClientNames()
        LbxClients.Items.Clear()
        AllClients.Clear()  ' Clear the global list

        ' Call the function to get the list of clients
        Dim clients As List(Of Object) = GetClients()

        ' Populate the ListBox and the AllClients list
        For Each client In clients
            LbxClients.Items.Add(client)
            AllClients.Add(client)
        Next
    End Sub

    ' Loads the product names and IDs into the LbxProducts ListBox and the AllProducts list.
    Private Sub LoadProducts()
        LbxProducts.Items.Clear()
        AllProducts.Clear()

        ' Call the function to get the list of products
        Dim products As List(Of Object) = GetProducts()

        ' Populate the ListBox and the AllProducts list
        For Each product In products
            LbxProducts.Items.Add(product)
            AllProducts.Add(product)
        Next
    End Sub

    ' Custom drawing of items in LbxClients ListBox.
    Private Sub LbxClients_DrawItem(sender As Object, e As DrawItemEventArgs) Handles LbxClients.DrawItem
        e.DrawBackground()
        If e.Index >= 0 Then
            Dim item = DirectCast(LbxClients.Items(e.Index), Object)
            e.Graphics.DrawString(item.Name, e.Font, Brushes.Black, e.Bounds)
        End If
        e.DrawFocusRectangle()
    End Sub

    ' Custom drawing of items in LbxProducts ListBox.
    Private Sub LbxProducts_DrawItem(sender As Object, e As DrawItemEventArgs) Handles LbxProducts.DrawItem
        e.DrawBackground()
        If e.Index >= 0 Then
            Dim item = DirectCast(LbxProducts.Items(e.Index), Object)
            e.Graphics.DrawString(item.Name, e.Font, Brushes.Black, e.Bounds)
        End If
        e.DrawFocusRectangle()
    End Sub

    ' Button click event to add a new product and refresh the product list.
    Private Sub ButtonCreate_Click(sender As Object, e As EventArgs) Handles BtnAddProduct.Click
        InsertNewProduct()
        LoadProducts()
    End Sub

    ' Handle double-click event on the products list box to open details for the selected product.
    Private Sub LbxProducts_DoubleClick(sender As Object, e As EventArgs) Handles LbxProducts.DoubleClick
        If LbxProducts.SelectedIndex <> -1 Then
            Dim selectedProduct = DirectCast(LbxProducts.SelectedItem, Object)

            ' Open a new form to view and possibly edit the product details
            Dim LicenseView As New LicenseViewfrm()
            LicenseView.Text = selectedProduct.name
            LicenseView.CurrentProductID = selectedProduct.ID
            LicenseView.Show()
        Else
            MessageBox.Show("Please select a product from the list.")
        End If
    End Sub

    ' Load the features and attributes for the selected product when selection changes.
    Private Sub LbxProducts_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LbxProducts.SelectedIndexChanged
        If LbxProducts.SelectedIndex = -1 Then Return

        Dim selectedProduct = DirectCast(LbxProducts.SelectedItem, Object)
        Dim ProductID = selectedProduct.ID

        LoadFeaturesIntoLbxFeatures(ProductID)
        LoadAttributesIntoLbxAttributes(ProductID)
    End Sub

    ' Loads the features related to the selected product into the LbxFeatures ListBox.
    Private Sub LoadFeaturesIntoLbxFeatures(productID As Integer)
        LbxFeatures.Items.Clear()

        ' Call the function to get the features
        Dim features As List(Of String) = GetFeaturesList(productID)

        ' Add the obtained features to the ListBox
        For Each feature As String In features
            LbxFeatures.Items.Add(feature)
        Next
    End Sub

    ' Load attributes related to the selected product into LbxAttributes list box.
    Private Sub LoadAttributesIntoLbxAttributes(productID As Integer)
        LbxAttributes.Items.Clear()


        ' Call the function to get the features
        Dim attributes As List(Of String) = GetAttributesList(productID)

        ' Add the obtained features to the ListBox
        For Each attribute As String In attributes
            LbxAttributes.Items.Add(attribute)
        Next
    End Sub

    Private Sub BtnFeatureAdd_Click(sender As Object, e As EventArgs) Handles BtnFeatureAdd.Click
        ' Check if a product is selected from the list
        If LbxProducts.SelectedIndex = -1 Then
            MessageBox.Show("Please select a product.")
            Return
        End If

        Dim selectedProduct = DirectCast(LbxProducts.SelectedItem, Object)
        Dim ProductID = selectedProduct.ID
        If ProductID = -1 Then
            MessageBox.Show("The selected product could not be found.")
            Return
        End If

        ' Prompt the user to enter the name of the new feature
        Dim featureName As String = InputBox("Enter the name of the new feature:", "Add Feature")
        If String.IsNullOrEmpty(featureName) Then
            MessageBox.Show("Feature name cannot be empty.")
            Return
        End If

        ' Check if the feature already exists
        If FeatureExists(ProductID, featureName) Then
            MessageBox.Show("A feature with this name already exists for this product.")
            Return
        End If

        ' Insert the new feature into the database
        If AddFeature(ProductID, featureName) Then
            MessageBox.Show("Feature successfully added.")
            LoadFeaturesIntoLbxFeatures(ProductID)
        Else
            MessageBox.Show("Error adding the feature.")
        End If
    End Sub

    Private Sub BtnAttributeAdd_Click(sender As Object, e As EventArgs) Handles BtnAttributeAdd.Click
        ' Check if a product is selected from the list
        If LbxProducts.SelectedIndex = -1 Then
            MessageBox.Show("Please select a product.")
            Return
        End If

        Dim selectedProduct = DirectCast(LbxProducts.SelectedItem, Object)
        Dim ProductID = selectedProduct.ID
        If ProductID = -1 Then
            MessageBox.Show("The selected product could not be found.")
            Return
        End If

        ' Prompt the user to enter the name of the new attribute
        Dim attributeName As String = InputBox("Enter the name of the new attribute:", "Add Attribute")
        If String.IsNullOrEmpty(attributeName) Then
            MessageBox.Show("Attribute name cannot be empty.")
            Return
        End If

        ' Check if the attribute already exists
        If AttributeExists(ProductID, attributeName) Then
            MessageBox.Show("An attribute with this name already exists for this product.")
            Return
        End If

        ' Insert the new attribute into the database
        If AddAttribute(ProductID, attributeName) Then
            MessageBox.Show("Attribute successfully added.")
            LoadAttributesIntoLbxAttributes(ProductID)
        Else
            MessageBox.Show("Error adding the attribute.")
        End If
    End Sub

    Private Sub BtnFeatureDelete_Click(sender As Object, e As EventArgs) Handles BtnFeatureDelete.Click
        ' Check if a product is selected from the list
        If LbxProducts.SelectedIndex = -1 Then
            MessageBox.Show("Please select a product.")
            Return
        End If

        Dim featureName As String = LbxFeatures.SelectedItem.ToString()
        Dim selectedProduct = DirectCast(LbxProducts.SelectedItem, Object)
        Dim ProductID = selectedProduct.ID
        If ProductID = -1 Then
            MessageBox.Show("The selected product could not be found.")
            Return
        End If

        ' Delete the feature from the database
        If DeleteFeature(ProductID, featureName) Then
            MessageBox.Show("Feature successfully deleted.")
            LoadFeaturesIntoLbxFeatures(ProductID)
        Else
            MessageBox.Show("Error deleting the feature.")
        End If
    End Sub

    Private Sub BtnAttributeDelete_Click(sender As Object, e As EventArgs) Handles BtnAttributeDelete.Click
        ' Get the ProductID of the product selected in the list
        If LbxProducts.SelectedIndex = -1 Then
            MessageBox.Show("Please select a product.")
            Return
        End If

        Dim attributeName As String = LbxAttributes.SelectedItem.ToString()
        Dim selectedProduct = DirectCast(LbxProducts.SelectedItem, Object)
        Dim ProductID = selectedProduct.ID
        If ProductID = -1 Then
            MessageBox.Show("The selected product could not be found.")
            Return
        End If

        ' Delete the attribute from the database
        If DeleteAttribute(ProductID, attributeName) Then
            MessageBox.Show("Attribute successfully deleted.")
            LoadAttributesIntoLbxAttributes(ProductID)
        Else
            MessageBox.Show("Error deleting the attribute.")
        End If
    End Sub

    Private Sub BtnViewLicenses_Click(sender As Object, e As EventArgs) Handles BtnViewLicenses.Click
        LbxProducts_DoubleClick(sender, e)  ' Trigger the double click event handler programmatically
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles TxtSearch.TextChanged
        Dim searchQuery As String = TxtSearch.Text.ToLower()
        LbxProducts.Items.Clear()  ' Clear the ListBox before filtering

        ' Filter and add only the elements that match the search text
        For Each product In AllProducts
            If product.Name.ToLower().Contains(searchQuery) Then
                LbxProducts.Items.Add(product)
            End If
        Next
    End Sub

    Private Sub BtnAddUser_Click(sender As Object, e As EventArgs) Handles BtnAddUser.Click
        Dim NewUserName = InputBox("Enter new Username")
        ' Check if a user was writen
        If String.IsNullOrWhiteSpace(NewUserName) Then
            MessageBox.Show("Username cannot be empty.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Check if the user already exists
        If UserExists(NewUserName) Then
            MessageBox.Show("This username already exists. Please choose a different username.", "User Exists", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim NewPassword = InputBox("Enter new Password")
        ' Check if a password was writen
        If String.IsNullOrWhiteSpace(NewPassword) Then
            MessageBox.Show("Password cannot be empty.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not InsertUser(NewUserName, NewPassword) Then
            MessageBox.Show("Error inserting user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            MessageBox.Show("New user added successfully.", "User Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        LoadUsersToLstUser()
    End Sub

    Private Sub BtnChangePassword_Click(sender As Object, e As EventArgs) Handles BtnChangePassword.Click
        If LbxUsers.SelectedIndex = -1 Then
            MessageBox.Show("Please select a user first.")
            Exit Sub
        End If
        Dim NewPassword = InputBox("Enter new Password")
        If Not UpdatePassword(LbxUsers.SelectedItem.ToString(), NewPassword) Then
            MessageBox.Show("Error updating password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub BtnDeleteUser_Click(sender As Object, e As EventArgs) Handles BtnDeleteUser.Click
        If LbxUsers.SelectedIndex = -1 Then
            MessageBox.Show("Please select a user first.")
            Exit Sub
        End If
        If Not DeleteUser(LbxUsers.SelectedItem.ToString()) Then
            MessageBox.Show("Error deleting user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            MessageBox.Show("User deleted successfully.", "User Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        LoadUsersToLstUser()
    End Sub

    Private Sub TxtClientSearch_TextChanged(sender As Object, e As EventArgs) Handles TxtClientSearch.TextChanged
        FilterData()
    End Sub

    Private Sub FilterData()
        If cbxFilters.SelectedItem IsNot Nothing AndAlso Not String.IsNullOrEmpty(TxtClientSearch.Text) Then
            Dim filterField As String = cbxFilters.SelectedItem.ToString()
            Dim searchText As String = TxtClientSearch.Text

            ' Prepare SQL query with the applied filter
            Dim query As String = $"SELECT ClientID, Name FROM Clients WHERE {filterField} LIKE @SearchText"

            Using cmd As New SQLiteCommand(query, conn)
                ' Use parameters to prevent SQL injection
                cmd.Parameters.AddWithValue("@SearchText", "%" & searchText & "%")

                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    LbxClients.Items.Clear() ' Clear existing items in the ListBox

                    While reader.Read()
                        Dim clientID As Integer = reader.GetInt32(0)
                        Dim clientName As String = reader.GetString(1)
                        Dim client = New With {.ID = clientID, .Name = clientName}

                        LbxClients.Items.Add(client)
                    End While
                End Using
            End Using
        Else
            ' If no selection criteria or the search text is empty
            LoadClientNames()
        End If
    End Sub

    ' Initializes the Filter ComboBox with the column names from the Clients table.
    Private Sub InitializeFilterComboBox()
        ' Clear existing items in the ComboBox.
        cbxFilters.Items.Clear()

        ' Get the list of client fields using the existing function.
        Dim clientFields As List(Of EditableKeyValuePair) = GetClientFields()

        ' Add each field name to the ComboBox.
        For Each field As EditableKeyValuePair In clientFields
            cbxFilters.Items.Add(field.Key)
        Next

        ' Set the selected index to the first item, if there are any items.
        If cbxFilters.Items.Count > 0 Then cbxFilters.SelectedIndex = 0
    End Sub

    Private Sub cbxFilters_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxFilters.SelectedIndexChanged
        FilterData()
    End Sub

    ' Loads client data into the ClientsDataGrid based on the given client ID.
    Private Sub LoadClientData(clientId As Integer)
        ' Call the function to get the client data
        Dim clientDetails As List(Of EditableKeyValuePair) = GetClientDataByClientID(clientId)

        ' Set the data source of the DataGridView and format the grid
        ClientsDataGrid.DataSource = clientDetails
        FormatDataGridView()
    End Sub

    Private Sub FormatDataGridView()
        With ClientsDataGrid
            .AutoGenerateColumns = True
            .Columns(0).HeaderText = "Field"
            .Columns(0).DataPropertyName = "Key"
            .Columns(1).HeaderText = "Value"
            .Columns(1).DataPropertyName = "Value"
            .Columns("Key").ReadOnly = True

            ' Ensure that the DataGridView has been populated before attempting to configure columns
            If .Rows.Count > 0 Then
                For Each row As DataGridViewRow In .Rows
                    Dim key As String = row.Cells("Key").Value.ToString()
                    If key = "ClientID" OrElse key = "ProductCount" OrElse key = "CreatedBy" OrElse key = "CreatedTime" Then
                        row.Cells("Value").ReadOnly = True  ' Set only the value cell to read-only for specified keys
                    End If
                Next
            End If
        End With
    End Sub

    Private Sub LbxClients_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LbxClients.SelectedIndexChanged
        If LbxClients.SelectedIndex <> -1 Then
            Dim selectedClient = DirectCast(LbxClients.SelectedItem, Object)
            LoadClientData(selectedClient.ID)
        End If
    End Sub

    Private Sub BtnSaveClientChanges_Click(sender As Object, e As EventArgs) Handles BtnSaveClientChanges.Click
        If ClientsDataGrid.DataSource IsNot Nothing Then
            Dim updatedData As List(Of EditableKeyValuePair) = CType(ClientsDataGrid.DataSource, List(Of EditableKeyValuePair))
            Dim clientId As Integer = CType(updatedData.FirstOrDefault(Function(x) x.Key = "ClientID").Value, Integer)

            ' Check if the "Name" field is empty
            Dim nameKvp = updatedData.FirstOrDefault(Function(x) x.Key = "Name")
            If nameKvp IsNot Nothing AndAlso String.IsNullOrWhiteSpace(nameKvp.Value.ToString()) Then
                MessageBox.Show("The Name field cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim transaction = conn.BeginTransaction()

            Try
                If ClientExistsByID(clientId) Then
                    ' Update existing client
                    UpdateClient(updatedData, clientId)
                Else
                    ' Set or update CreatedBy and CreatedTime fields before inserting or updating
                    SetAuditFields(updatedData, clientId)

                    ' Insert new client
                    InsertNewClient(updatedData)
                    LoadClientData(clientId)  ' Refresh client data
                End If

                transaction.Commit()
                MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Error applying changes: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                ' Cleanup or additional actions could be placed here if needed
            End Try

            LoadClientNames()  ' Refresh the client list
        End If
    End Sub

    Private Sub SetAuditFields(ByRef updatedData As List(Of EditableKeyValuePair), clientId As Integer)
        ' Set CreatedBy and CreatedTime only if it is a new client
        If Not ClientExistsByID(clientId) Then
            ' If it is a new client, add CreatedBy and CreatedTime
            updatedData.RemoveAll(Function(x) x.Key = "CreatedBy" Or x.Key = "CreatedTime")  ' Remove if already present to avoid duplicates
            updatedData.Add(New EditableKeyValuePair("CreatedBy", SignedUser))
            updatedData.Add(New EditableKeyValuePair("CreatedTime", DateTime.Now))
        Else
            ' If it is an existing client, ensure these fields are not updated
            updatedData.RemoveAll(Function(x) x.Key = "CreatedBy" Or x.Key = "CreatedTime")
        End If
    End Sub

    Private Sub UpdateControlsOnPermissions()
        ' Enable or disable controls based on permissions
        If isAdmin Then
            Exit Sub  ' Admin has all permissions, no need to disable any controls
        Else
            ' Set controls based on specific permissions
            BtnNewClient.Enabled = canEditClients
            BtnSaveClientChanges.Enabled = canEditClients
            BtnAddProduct.Enabled = canEditProducts
            BtnAttributeAdd.Enabled = canEditProducts
            BtnAttributeDelete.Enabled = canEditProducts
            BtnFeatureAdd.Enabled = canEditProducts
            BtnFeatureDelete.Enabled = canEditProducts
            BtnDeleteClient.Enabled = canDeleteClients
            BtnRemoveProduct.Enabled = canDeleteProducts
            BtnDeleteUser.Enabled = isAdmin
            BtnAddUser.Enabled = isAdmin
            BtnChangePassword.Enabled = isAdmin
            ChkPermissionAdmin.Enabled = isAdmin
            ChkPermissionCreateEditClients.Enabled = isAdmin
            ChkPermissionCreateEditProducts.Enabled = isAdmin
            ChkPermissionDeleteProducts.Enabled = isAdmin
            ChkPermissionDeleteClients.Enabled = isAdmin
            ChkPermissionEditLics.Enabled = isAdmin
            ChkPermissionExportLics.Enabled = isAdmin
            ChkPermissionExportKeys.Enabled = isAdmin
            ChkPermissionOnlyTrials.Enabled = isAdmin
            TxtMaxTrial.Enabled = isAdmin
            BtnApplyPermission.Enabled = isAdmin
            Label7.Enabled = isAdmin
        End If

    End Sub

    Private Sub BtnNewClient_Click(sender As Object, e As EventArgs) Handles BtnNewClient.Click
        Dim clientDetails As List(Of EditableKeyValuePair) = GetClientFields()
        ' Set the DataSource of the DataGridView
        ClientsDataGrid.DataSource = clientDetails
        FormatDataGridView()
    End Sub

    Private Sub LbxUsers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LbxUsers.SelectedIndexChanged
        If LbxUsers.SelectedIndex = -1 Then Return

        Dim selectedUser As String = LbxUsers.SelectedItem.ToString()
        ShowUserPermissions(selectedUser)
    End Sub

    ' Loads the user permissions from the database and updates the CheckBoxes and TextBox.
    Private Sub ShowUserPermissions(userName As String)
        ' Call the function to get the user permissions
        Dim permissions As Dictionary(Of String, Object) = GetUserPermissionsDictionary(userName)

        If permissions.Count > 0 Then
            ' Update each CheckBox and TextBox based on the retrieved permissions
            ChkPermissionAdmin.Checked = permissions("IsAdmin")
            ChkPermissionCreateEditClients.Checked = permissions("EditClients")
            ChkPermissionCreateEditProducts.Checked = permissions("EditProducts")
            ChkPermissionDeleteClients.Checked = permissions("DeleteClients")
            ChkPermissionDeleteProducts.Checked = permissions("DeleteProducts")
            ChkPermissionEditLics.Checked = permissions("EditLics")
            ChkPermissionExportLics.Checked = permissions("ExportLics")
            ChkPermissionExportKeys.Checked = permissions("ExportKeys")
            ChkPermissionOnlyTrials.Checked = permissions("OnlyTrials")
            TxtMaxTrial.Text = permissions("MaxTrialDays").ToString()
        Else
            ' Clear the CheckBoxes if no data is found
            ClearAllUserCheckBoxes()
        End If
    End Sub

    Private Sub ClearAllUserCheckBoxes()
        ChkPermissionAdmin.Checked = False
        ChkPermissionCreateEditClients.Checked = False
        ChkPermissionCreateEditProducts.Checked = False
        ChkPermissionDeleteClients.Checked = False
        ChkPermissionDeleteProducts.Checked = False
        ChkPermissionEditLics.Checked = False
        ChkPermissionExportLics.Checked = False
        ChkPermissionExportKeys.Checked = False
        ChkPermissionOnlyTrials.Checked = False
        TxtMaxTrial.Text = String.Empty
    End Sub

    Private Sub BtnApplyPermission_Click(sender As Object, e As EventArgs) Handles BtnApplyPermission.Click
        If LbxUsers.SelectedIndex = -1 Then
            MessageBox.Show("Please select a user first.")
            Exit Sub
        End If
        Dim SelectedUser As String = LbxUsers.SelectedItem.ToString()
        ' Update each user permission based on CheckBox states
        UpdateUserPermissionBool(SelectedUser, "IsAdmin", ChkPermissionAdmin.Checked)
        UpdateUserPermissionBool(SelectedUser, "EditClients", ChkPermissionCreateEditClients.Checked)
        UpdateUserPermissionBool(SelectedUser, "EditProducts", ChkPermissionCreateEditProducts.Checked)
        UpdateUserPermissionBool(SelectedUser, "DeleteClients", ChkPermissionDeleteClients.Checked)
        UpdateUserPermissionBool(SelectedUser, "DeleteProducts", ChkPermissionDeleteProducts.Checked)
        UpdateUserPermissionBool(SelectedUser, "EditLics", ChkPermissionEditLics.Checked)
        UpdateUserPermissionBool(SelectedUser, "ExportLics", ChkPermissionExportLics.Checked)
        UpdateUserPermissionBool(SelectedUser, "ExportKeys", ChkPermissionExportKeys.Checked)
        UpdateUserPermissionBool(SelectedUser, "OnlyTrials", ChkPermissionOnlyTrials.Checked)
        UpdateUserPermisionInt(SelectedUser, "MaxTrialDays", TxtMaxTrial.Text)
    End Sub

    Private Sub BtnRemoveProduct_Click(sender As Object, e As EventArgs) Handles BtnRemoveProduct.Click
        If LbxProducts.SelectedIndex = -1 Then
            MessageBox.Show("Please select a product first.")
            Return
        End If

        Dim productName As String = LbxProducts.SelectedItem.ToString()
        Dim selectedProduct = DirectCast(LbxProducts.SelectedItem, Object)
        Dim ProductID = selectedProduct.ID

        ' Search for the product by name
        For Each product In AllProducts
            If product.Name.Equals(productName) Then
                ProductID = product.ID
                Exit For
            End If
        Next

        ' Check if a valid product ID was found
        If ProductID <> -1 Then
            ' Confirm product deletion
            If MessageBox.Show("Are you sure you want to delete this product and all associated data?", "Confirm Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                DeleteProductFromDatabase(ProductID)
            End If
        Else
            MessageBox.Show(ProductID & "Product ID not found.")
        End If
        LoadProducts()
    End Sub

    Private Sub BtnDeleteClient_Click(sender As Object, e As EventArgs) Handles BtnDeleteClient.Click
        ' Check if a client is selected in the ListBox
        If LbxClients.SelectedIndex <> -1 Then
            Dim selectedClient = DirectCast(LbxClients.SelectedItem, Object)
            Dim clientId = selectedClient.ID

            ' Confirm deletion with the user
            If MessageBox.Show("Are you sure you want to delete this client and all associated licenses?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                If DeleteClientAndLicenses(clientId) Then
                    MessageBox.Show("Client and all associated licenses successfully deleted.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ' Refresh the list of clients
                    LoadClientNames()  ' Ensure this function reloads the LbxClients properly
                Else
                    MessageBox.Show("Error deleting the client.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Else
            MessageBox.Show("Please select a client to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub TxtMaxTrial_TextChanged(sender As Object, e As EventArgs) Handles TxtMaxTrial.TextChanged
        ' Save the current cursor position
        Dim cursorPosition As Integer = TxtMaxTrial.SelectionStart

        ' Keep only numeric characters in the TextBox
        TxtMaxTrial.Text = System.Text.RegularExpressions.Regex.Replace(TxtMaxTrial.Text, "[^\d]", "")

        ' Ensure the TextBox is not empty
        If TxtMaxTrial.Text.Length = 0 Then
            TxtMaxTrial.Text = "0"
            cursorPosition = 1
        End If

        ' Restore the cursor position
        TxtMaxTrial.SelectionStart = Math.Min(cursorPosition, TxtMaxTrial.Text.Length)
    End Sub

End Class