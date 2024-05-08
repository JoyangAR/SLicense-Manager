Imports System.Data.SQLite
Imports System.IO

Public Class Mainfrm

    Private dtClients As DataTable

    Private dbFileName As String = Path.Combine(AppDirectory, "StandardLM.db")
    Private connectionString As String = $"Data Source={dbFileName};Version=3;"
    Private productIDs As New Dictionary(Of String, Integer)
    Private AllClients As New List(Of Object)

    ' Handles the event when the MainForm is loaded.
    Private Sub Mainfrm_Load(sender As Object, e As EventArgs) Handles Me.Load
        LoadProducts()                                   ' Load all products into the ListBox.
        LoadUsersToLstUser()                             ' Load all users into the ListBox for user management.
        LoadClientNames()                                ' Load all client names into the ListBox for client management.
        InitializeFilterComboBox()                       ' Initialize the filter ComboBox with options based on client data fields.
        ConfigureUserPermissions()                       ' Check and set permissions for the currently signed-in user.
        Me.Text = $"License Manager User: {SignedUser}"  ' Set the form's title to include the signed-in user's name for a personalized touch.
    End Sub


    ' Load user names into the lstuser ListBox.
    Private Sub LoadUsersToLstUser()
        ' Define SQL command to select user names from the Users table.
        Dim query As String = "SELECT User FROM Users"
        Using cmd As New SQLiteCommand(query, conn)
            ' Execute the command and read the results.
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                ' Clear any existing items in the ListBox before loading new data.
                LbxUsers.Items.Clear()

                ' Read each record individually.
                While reader.Read()
                    ' Add each user name to the ListBox.
                    LbxUsers.Items.Add(reader("User").ToString())
                End While
            End Using
        End Using
    End Sub


    ' Load client names and IDs into the LbxClients ListBox and a global list.
    Private Sub LoadClientNames()
        LbxClients.Items.Clear()
        AllClients.Clear()  ' Ensure to clear the global list as well

        Dim query As String = "SELECT DISTINCT Clients.ClientID, Clients.Name FROM Clients;"

        Using cmd As New SQLiteCommand(query, conn)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim clientID As Integer = Convert.ToInt32(reader("ClientID"))
                    Dim clientName As String = reader("Name").ToString()
                    Dim client = New With {.Name = clientName, .ID = clientID}

                    LbxClients.Items.Add(client)
                    AllClients.Add(client)  ' Add the client to the global list
                End While
            End Using
        End Using
    End Sub

    ' Custom drawing of items in LbxClients ListBox.
    Private Sub LbxClients_DrawItem(sender As Object, e As DrawItemEventArgs) Handles LbxClients.DrawItem
        e.DrawBackground()
        If e.Index >= 0 Then
            Dim item = DirectCast(LbxClients.Items(e.Index), Object)  ' Assume you've added the object as shown previously
            e.Graphics.DrawString(item.Name, e.Font, Brushes.Black, e.Bounds)
        End If
        e.DrawFocusRectangle()
    End Sub

    ' Load product names and IDs into the LbxProducts ListBox and a dictionary.
    Private Sub LoadProducts()
        LbxProducts.Items.Clear()
        productIDs.Clear()

        Dim query As String = "SELECT ProductID, ProductName FROM Products;"
        Using cmd As New SQLiteCommand(query, conn)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim productName As String = reader("ProductName").ToString()
                    Dim productID As Integer = Convert.ToInt32(reader("ProductID"))
                    LbxProducts.Items.Add(productName)
                    productIDs.Add(productName, productID)  ' Map productName to productID
                End While
            End Using
        End Using
    End Sub


    ' Button click event to add a new product and refresh the product list.
    Private Sub ButtonCreate_Click(sender As Object, e As EventArgs) Handles BtnAddProduct.Click
        InsertNewProduct()
        LoadProducts()
    End Sub

    ' Handle double-click event on the products list box to open details for the selected product.
    Private Sub LbxProducts_DoubleClick(sender As Object, e As EventArgs) Handles LbxProducts.DoubleClick
        If LbxProducts.SelectedIndex <> -1 Then
            Dim selectedProductName As String = LbxProducts.SelectedItem.ToString()
            If productIDs.ContainsKey(selectedProductName) Then
                SelectedProductID = productIDs(selectedProductName)
                ' Open a new form to view and possibly edit the product details
                Dim LicenseView As New LicenseViewfrm()
                LicenseView.Text = selectedProductName
                LicenseView.Show()
            Else
                MessageBox.Show("Product ID not found.")
            End If
        Else
            MessageBox.Show("Please select a product from the list.")
        End If
    End Sub

    ' Load the features and attributes for the selected product when selection changes.
    Private Sub LbxProducts_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LbxProducts.SelectedIndexChanged
        If LbxProducts.SelectedIndex = -1 Then Return

        Dim productName As String = LbxProducts.SelectedItem.ToString()
        Dim productID As Integer = GetProductIDByProductName(productName)
        If productID = -1 Then
            MessageBox.Show("Product ID not found.")
            Return
        End If

        LoadFeaturesIntoLbxFeatures(productID)
        LoadAttributesIntoLbxAttributes(productID)
    End Sub

    ' Load features related to the selected product into LbxFeatures list box.
    Private Sub LoadFeaturesIntoLbxFeatures(productID As Integer)
        LbxFeatures.Items.Clear()

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

    ' Load attributes related to the selected product into LbxAttributes list box.
    Private Sub LoadAttributesIntoLbxAttributes(productID As Integer)
        LbxAttributes.Items.Clear()

        Dim query As String = "SELECT AttributeName FROM Attributes WHERE ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    LbxAttributes.Items.Add(reader("AttributeName").ToString())
                End While
            End Using
        End Using
    End Sub

    Private Sub BtnFeatureAdd_Click(sender As Object, e As EventArgs) Handles BtnFeatureAdd.Click
        ' Check if a product is selected from the list
        If LbxProducts.SelectedIndex = -1 Then
            MessageBox.Show("Please select a product.")
            Return
        End If

        Dim productName As String = LbxProducts.SelectedItem.ToString()
        Dim productID As Integer = GetProductIDByProductName(productName)
        If productID = -1 Then
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
        If FeatureExists(productID, featureName) Then
            MessageBox.Show("A feature with this name already exists for this product.")
            Return
        End If

        ' Insert the new feature into the database
        If AddFeatureToDatabase(productID, featureName) Then
            MessageBox.Show("Feature successfully added.")
            LoadFeaturesIntoLbxFeatures(productID)  ' Assuming this function refreshes the feature list
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

        Dim productName As String = LbxProducts.SelectedItem.ToString()
        Dim productID As Integer = GetProductIDByProductName(productName)
        If productID = -1 Then
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
        If AttributeExists(productID, attributeName) Then
            MessageBox.Show("An attribute with this name already exists for this product.")
            Return
        End If

        ' Insert the new attribute into the database
        If AddAttributeToDatabase(productID, attributeName) Then
            MessageBox.Show("Attribute successfully added.")
            LoadAttributesIntoLbxAttributes(productID)  ' Assuming this function refreshes the attribute list
        Else
            MessageBox.Show("Error adding the attribute.")
        End If
    End Sub


    Private Function AddAttributeToDatabase(productID As Integer, attributeName As String) As Boolean
        Dim query As String = "INSERT INTO Attributes (AttributeName, ProductID) VALUES (@AttributeName, @ProductID)"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@AttributeName", attributeName)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Try
                cmd.ExecuteNonQuery()
                Return True
            Catch ex As Exception
                MessageBox.Show("Error inserting attribute: " & ex.Message)
                Return False
            End Try
        End Using
    End Function

    Private Sub BtnFeatureDelete_Click(sender As Object, e As EventArgs) Handles BtnFeatureDelete.Click
        ' Check if a product is selected from the list
        If LbxProducts.SelectedIndex = -1 Then
            MessageBox.Show("Please select a product.")
            Return
        End If

        Dim featureName As String = LbxFeatures.SelectedItem.ToString()
        Dim productID As Integer = GetProductIDByProductName(LbxProducts.SelectedItem.ToString())
        If productID = -1 Then
            MessageBox.Show("The selected product could not be found.")
            Return
        End If

        ' Delete the feature from the database
        If DeleteFeatureFromDatabase(productID, featureName) Then
            MessageBox.Show("Feature successfully deleted.")
            LoadFeaturesIntoLbxFeatures(productID)  ' Assuming this function updates the feature list in the ListBox
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
        Dim productID As Integer = GetProductIDByProductName(LbxProducts.SelectedItem.ToString())
        If productID = -1 Then
            MessageBox.Show("The selected product could not be found.")
            Return
        End If

        ' Delete the attribute from the database
        If DeleteAttributeFromDatabase(productID, attributeName) Then
            MessageBox.Show("Attribute successfully deleted.")
            LoadAttributesIntoLbxAttributes(productID)
        Else
            MessageBox.Show("Error deleting the attribute.")
        End If
    End Sub

    Private Sub BtnViewLicenses_Click(sender As Object, e As EventArgs) Handles BtnViewLicenses.Click
        LbxProducts_DoubleClick(sender, e)  ' Trigger the double click event handler programmatically
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles TxtSearch.TextChanged
        Dim searchText As String = TxtSearch.Text.ToLower()

        ' Filter the dictionary for product names that contain the search text
        Dim filteredProducts = productIDs.Where(Function(p) p.Key.ToLower().Contains(searchText))

        ' Clear existing items in LbxProducts before adding new ones
        LbxProducts.Items.Clear()

        ' Add the filtered product names to the ListBox
        For Each product In filteredProducts
            LbxProducts.Items.Add(product.Key)
        Next
    End Sub


    Private Sub BtnAddUser_Click(sender As Object, e As EventArgs) Handles BtnAddUser.Click
        Dim NewUserName = InputBox("Enter new Username")
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
        If String.IsNullOrWhiteSpace(NewPassword) Then
            MessageBox.Show("Password cannot be empty.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Insert new user
        Dim insertQuery As String = "INSERT INTO Users (User, Password, IsAdmin, EditClients, EditProducts, DeleteClients, DeleteProducts, ExportLics, EditLics, ExportKeys) VALUES (@User, @Password, 0, 0, 0, 0, 0, 0, 0, 0)"
        Using cmd As New SQLiteCommand(insertQuery, conn)
            cmd.Parameters.AddWithValue("@User", NewUserName)
            cmd.Parameters.AddWithValue("@Password", NewPassword)
            cmd.ExecuteNonQuery()
        End Using

        LoadUsersToLstUser()
        MessageBox.Show("New user added successfully.", "User Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BtnChangePassword_Click(sender As Object, e As EventArgs) Handles BtnChangePassword.Click
        Dim NewPassword = InputBox("Enter new Password")

        ' Update user's password
        Dim updateQuery As String = "UPDATE Users SET Password = @Password WHERE User = @User;"
        Using cmd As New SQLiteCommand(updateQuery, conn)
            cmd.Parameters.AddWithValue("@Password", NewPassword)
            cmd.Parameters.AddWithValue("@User", LbxUsers.SelectedItem.ToString())
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub BtnDeleteUser_Click(sender As Object, e As EventArgs) Handles BtnDeleteUser.Click
        ' Delete user
        Dim deleteQuery As String = "DELETE FROM Users WHERE User = @User;"
        Using cmd As New SQLiteCommand(deleteQuery, conn)
            cmd.Parameters.AddWithValue("@User", LbxUsers.SelectedItem.ToString())
            cmd.ExecuteNonQuery()
        End Using
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
                        Dim clientID As Integer = reader.GetInt32(0) ' Assuming ClientID is the first column
                        Dim clientName As String = reader.GetString(1) ' Assuming Name is the second column
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


    Private Sub InitializeFilterComboBox()
        ' Query to fetch column names from the Clients table
        Dim query As String = "PRAGMA table_info(Clients);"
        Using cmd As New SQLiteCommand(query, conn)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                cbxFilters.Items.Clear()
                While reader.Read()
                    ' Column name is at index 1 in the result set
                    cbxFilters.Items.Add(reader.GetString(1))
                End While
            End Using
        End Using

        ' Set the selected index to the first item, if there are any items
        If cbxFilters.Items.Count > 0 Then cbxFilters.SelectedIndex = 0
    End Sub

    Private Sub cbxFilters_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxFilters.SelectedIndexChanged
        FilterData()
    End Sub

    Private Sub LoadClientData(clientId As Integer)
        Dim query As String = "SELECT * FROM Clients WHERE ClientID = @ClientID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ClientID", clientId)

            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                Dim clientDetails As New List(Of EditableKeyValuePair)

                If reader.Read() Then
                    For i As Integer = 0 To reader.FieldCount - 1
                        clientDetails.Add(New EditableKeyValuePair(reader.GetName(i), reader.GetValue(i)))
                    Next
                End If

                ClientsDataGrid.DataSource = clientDetails
                FormatDataGridView()
            End Using
        End Using
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
            Dim selectedClient = DirectCast(LbxClients.SelectedItem, Object)  ' Assuming items are dynamic objects
            LoadClientData(selectedClient.ID)
        End If
    End Sub

    Private Sub BtnSaveClientChanges_Click(sender As Object, e As EventArgs) Handles BtnSaveClientChanges.Click
        If ClientsDataGrid.DataSource IsNot Nothing Then
            Dim updatedData As List(Of EditableKeyValuePair) = CType(ClientsDataGrid.DataSource, List(Of EditableKeyValuePair))
            Dim clientId As Integer = CType(updatedData.FirstOrDefault(Function(x) x.Key = "ClientID").Value, Integer)

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
                MessageBox.Show("Changes saved successfully.")
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Error applying changes: " & ex.Message)
            Finally
                ' Cleanup or additional actions could be placed here if needed
            End Try

            LoadClientNames()  ' Refresh the client list
        End If
    End Sub

    Private Function ClientExistsByID(clientId As Integer) As Boolean
        Dim cmd As New SQLiteCommand("SELECT COUNT(*) FROM Clients WHERE ClientID = @ClientID", conn)
        cmd.Parameters.AddWithValue("@ClientID", clientId)
        Dim result As Object = cmd.ExecuteScalar()
        Return Convert.ToInt32(result) > 0
    End Function

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


    Private Sub ConfigureUserPermissions()
        Dim query As String = "SELECT IsAdmin, EditClients, EditProducts, DeleteClients, DeleteProducts, ExportLics, ExportKeys, EditLics FROM Users WHERE User = @SignedUser"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@SignedUser", SignedUser)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    isAdmin = Convert.ToBoolean(reader("IsAdmin"))
                    canEditClients = Convert.ToBoolean(reader("EditClients"))
                    canEditProducts = Convert.ToBoolean(reader("EditProducts"))
                    canDeleteClients = Convert.ToBoolean(reader("DeleteClients"))
                    canDeleteProducts = Convert.ToBoolean(reader("DeleteProducts"))
                    canExportLics = Convert.ToBoolean(reader("ExportLics"))
                    canExportKeys = Convert.ToBoolean(reader("ExportKeys"))
                    canEditLics = Convert.ToBoolean(reader("EditLics"))

                    ' Enable or disable controls based on permissions
                    If isAdmin Then
                        Exit Sub  ' Admin has all rights, no need to disable any controls
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
                    End If
                End If
            End Using
        End Using
    End Sub

    Private Sub BtnNewClient_Click(sender As Object, e As EventArgs) Handles BtnNewClient.Click
        Dim clientDetails As List(Of EditableKeyValuePair) = GetClientFields()
        ' Set the DataSource of the DataGridView
        ClientsDataGrid.DataSource = clientDetails
        FormatDataGridView()
    End Sub

    Private Sub UpdateUserPermission(permission As String, value As Boolean)
        If LbxUsers.SelectedIndex = -1 Then
            MessageBox.Show("Please select a user first.")
            Return
        End If

        Dim selectedUser As String = LbxUsers.SelectedItem.ToString()  ' Assuming ListBox displays usernames

        Dim query As String = $"UPDATE Users SET {permission} = @Value WHERE User = @UserName"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@Value", value)
            cmd.Parameters.AddWithValue("@UserName", selectedUser)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub LbxUsers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LbxUsers.SelectedIndexChanged
        If LbxUsers.SelectedIndex = -1 Then Return

        Dim selectedUser As String = LbxUsers.SelectedItem.ToString()
        LoadUserPermissions(selectedUser)
    End Sub


    Private Sub LoadUserPermissions(userName As String)
        Dim query As String = "SELECT IsAdmin, EditClients, EditProducts, DeleteClients, DeleteProducts, ExportLics, ExportKeys, EditLics FROM Users WHERE User = @User"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@User", userName)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Update each CheckBox based on user data
                    ChkPermissionAdmin.Checked = Convert.ToBoolean(reader("IsAdmin"))
                    ChkPermissionCreateEditClients.Checked = Convert.ToBoolean(reader("EditClients"))
                    ChkPermissionCreateEditProducts.Checked = Convert.ToBoolean(reader("EditProducts"))
                    ChkPermissionDeleteClients.Checked = Convert.ToBoolean(reader("DeleteClients"))
                    ChkPermissionDeleteProducts.Checked = Convert.ToBoolean(reader("DeleteProducts"))
                    ChkPermissionEditLics.Checked = Convert.ToBoolean(reader("EditLics"))
                    ChkPermissionExportLics.Checked = Convert.ToBoolean(reader("ExportLics"))
                    ChkPermissionExportKeys.Checked = Convert.ToBoolean(reader("ExportKeys"))
                Else
                    ' Uncheck all CheckBoxes if no data found
                    ClearAllCheckBoxes()
                End If
            End Using
        End Using
    End Sub

    Private Sub ClearAllCheckBoxes()
        ChkPermissionAdmin.Checked = False
        ChkPermissionCreateEditClients.Checked = False
        ChkPermissionCreateEditProducts.Checked = False
        ChkPermissionDeleteClients.Checked = False
        ChkPermissionDeleteProducts.Checked = False
        ChkPermissionEditLics.Checked = False
        ChkPermissionExportLics.Checked = False
        ChkPermissionExportKeys.Checked = False
    End Sub

    Private Sub BtnApplyPermission_Click(sender As Object, e As EventArgs) Handles BtnApplyPermission.Click
        ' Update each user permission based on CheckBox states
        UpdateUserPermission("IsAdmin", ChkPermissionAdmin.Checked)
        UpdateUserPermission("EditClients", ChkPermissionCreateEditClients.Checked)
        UpdateUserPermission("EditProducts", ChkPermissionCreateEditProducts.Checked)
        UpdateUserPermission("DeleteClients", ChkPermissionDeleteClients.Checked)
        UpdateUserPermission("DeleteProducts", ChkPermissionDeleteProducts.Checked)
        UpdateUserPermission("EditLics", ChkPermissionEditLics.Checked)
        UpdateUserPermission("ExportLics", ChkPermissionExportLics.Checked)
        UpdateUserPermission("ExportKeys", ChkPermissionExportKeys.Checked)
    End Sub

    Private Sub BtnRemoveProduct_Click(sender As Object, e As EventArgs) Handles BtnRemoveProduct.Click
        If LbxProducts.SelectedIndex = -1 Then
            MessageBox.Show("Please select a product first.")
            Return
        End If

        Dim productName As String = LbxProducts.SelectedItem.ToString()
        Dim productId As Integer
        If productIDs.TryGetValue(productName, productId) Then
            ' Confirm product deletion
            If MessageBox.Show("Are you sure you want to delete this product and all associated data?", "Confirm Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                DeleteProduct(productId)
            End If
        Else
            MessageBox.Show("Product ID not found.")
        End If
    End Sub

    Private Sub DeleteProduct(productId As Integer)
        Dim transaction = conn.BeginTransaction()

        Try
            ' Delete from each related table
            ExecuteDeleteCommand(conn, "DELETE FROM PrivateKeys WHERE ProductID = @ProductID", productId)
            ExecuteDeleteCommand(conn, "DELETE FROM PublicKeys WHERE ProductID = @ProductID", productId)
            ExecuteDeleteCommand(conn, "DELETE FROM Features WHERE ProductID = @ProductID", productId)
            ExecuteDeleteCommand(conn, "DELETE FROM Attributes WHERE ProductID = @ProductID", productId)
            ExecuteDeleteCommand(conn, "DELETE FROM Licenses WHERE ProductID = @ProductID", productId)
            ExecuteDeleteCommand(conn, "DELETE FROM Products WHERE ProductID = @ProductID", productId)

            transaction.Commit()
            MessageBox.Show("Product and all associated data have been deleted successfully.")
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("An error occurred: " & ex.Message)
        End Try
    End Sub

    ' Helper method to execute delete commands
    Private Sub ExecuteDeleteCommand(conn As SQLiteConnection, query As String, productId As Integer)
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productId)
            cmd.ExecuteNonQuery()
        End Using
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

    Private Function DeleteClientAndLicenses(clientId As Integer) As Boolean
        Dim transaction = conn.BeginTransaction()

        Try
            ' Delete all licenses associated with the client
            Dim deleteLicensesQuery As String = "DELETE FROM Licenses WHERE ClientID = @ClientID"
            Using cmdLicenses As New SQLiteCommand(deleteLicensesQuery, conn)
                cmdLicenses.Parameters.AddWithValue("@ClientID", clientId)
                cmdLicenses.Transaction = transaction
                cmdLicenses.ExecuteNonQuery()
            End Using

            ' Delete the client
            Dim deleteClientQuery As String = "DELETE FROM Clients WHERE ClientID = @ClientID"
            Using cmdClient As New SQLiteCommand(deleteClientQuery, conn)
                cmdClient.Parameters.AddWithValue("@ClientID", clientId)
                cmdClient.Transaction = transaction
                cmdClient.ExecuteNonQuery()
            End Using

            transaction.Commit()
            Return True
        Catch ex As Exception
            transaction.Rollback()  ' Undo changes on error
            MessageBox.Show("An error occurred: " & ex.Message, "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

End Class