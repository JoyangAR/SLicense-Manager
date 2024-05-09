Imports System.Data.SQLite
Imports System.IO
Imports System.Text.RegularExpressions
Imports Standard.Licensing
Imports Standard.Licensing.Validation

Module FunctionsModule

    Public AppDirectory As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
    Public LicenseDirectory As String = Path.Combine(AppDirectory, "LicenseFiles")
    Public dbFileName As String = Path.Combine(AppDirectory, "StandardLM.db")
    Public connectionString As String = $"Data Source={dbFileName};Version=3;"


    Public SignedUser As String

    Public isAdmin As Boolean
    Public canEditClients As Boolean
    Public canEditProducts As Boolean
    Public canDeleteClients As Boolean
    Public canDeleteProducts As Boolean
    Public canExportLics As Boolean
    Public canExportKeys As Boolean
    Public canEditLics As Boolean
    Public conn As New SQLiteConnection(connectionString)

    Sub OpenSqliteConnection()
        ' Open SQLite Connection
        conn.Open()
    End Sub

    Function LicenseException(ByVal license As License) As Boolean
        '  Check licensetype
        If (license.Type = LicenseType.Trial) Then
            Return True
        End If
        Return False
    End Function

    Function GetLicenseType(ByRef LicenseText) As LicenseType
        '  Get licensetype
        If (LicenseText = "Trial") Then
            Return LicenseType.Trial
        Else
            Return LicenseType.Standard
        End If

    End Function

    Function ValidateLicense(ByVal license As License, PublicKey As String) As String
        ' Validate license and define return value

        Dim ReturnValue As String = "License is Valid"

        Dim validationFailures = license.Validate().ExpirationDate().When(AddressOf LicenseException).And().Signature(PublicKey).AssertValidLicense()

        If (Not IsNothing(validationFailures)) Then
            ReturnValue = ""
            For Each validationFailure As IValidationFailure In validationFailures
                ReturnValue = ReturnValue & validationFailure.HowToResolve & ": " & vbNewLine & validationFailure.Message & vbNewLine
            Next
        End If

        Return ReturnValue

    End Function

    Sub CreateTables()
        OpenSqliteConnection()

        ' Create Users table
        Dim queryUsers As String = "CREATE TABLE IF NOT EXISTS Users (UserID INTEGER PRIMARY KEY AUTOINCREMENT, User TEXT, Password TEXT, IsAdmin INTEGER, EditClients INTEGER, EditProducts INTEGER, DeleteClients INTEGER, DeleteProducts INTEGER, ExportLics INTEGER, EditLics INTEGER, ExportKeys INTEGER);"
        Using cmd As New SQLiteCommand(queryUsers, conn)
            cmd.ExecuteNonQuery()
        End Using

        ' Create Products table
        Dim queryProducts As String = "CREATE TABLE IF NOT EXISTS Products (ProductID INTEGER PRIMARY KEY AUTOINCREMENT, ProductName TEXT, ProductPassword TEXT);"
        Using cmd As New SQLiteCommand(queryProducts, conn)
            cmd.ExecuteNonQuery()
        End Using

        ' Create Clients table
        Dim queryClients As String = "CREATE TABLE IF NOT EXISTS Clients (ClientID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT,Email TEXT, ProductCount INTEGER,Country TEXT, State TEXT, City TEXT, Company TEXT, Postal TEXT, Phone INTEGER, Address TEXT, CreatedBy TEXT, CreatedTime TEXT, Comment TEXT);"
        Using cmd As New SQLiteCommand(queryClients, conn)
            cmd.ExecuteNonQuery()
        End Using

        ' Create Licenses table
        Dim queryLicenses As String = "CREATE TABLE IF NOT EXISTS Licenses (LicenseID TEXT, ClientID INTEGER, ProductID INTEGER, CreatedBy TEXT, IssueDate TEXT, Expiration TEXT);"
        Using cmd As New SQLiteCommand(queryLicenses, conn)
            cmd.ExecuteNonQuery()
        End Using

        ' Create LicenseFiles table
        Dim queryLicenseFiles As String = "CREATE TABLE IF NOT EXISTS LicenseFiles (LicenseID TEXT, LicenseFile TEXT);"
        Using cmd As New SQLiteCommand(queryLicenseFiles, conn)
            cmd.ExecuteNonQuery()
        End Using

        ' Create PrivateKeys table
        Dim queryPrivateKeys As String = "CREATE TABLE IF NOT EXISTS PrivateKeys (ProductID INTEGER, PrivateKey TEXT);"
        Using cmd As New SQLiteCommand(queryPrivateKeys, conn)
            cmd.ExecuteNonQuery()
        End Using

        ' Create PublicKeys table
        Dim queryPublicKeys As String = "CREATE TABLE IF NOT EXISTS PublicKeys (ProductID INTEGER, PublicKey TEXT);"
        Using cmd As New SQLiteCommand(queryPublicKeys, conn)
            cmd.ExecuteNonQuery()
        End Using

        ' Create Features table
        Dim queryFeatures As String = "CREATE TABLE IF NOT EXISTS Features (FeatureID INTEGER PRIMARY KEY AUTOINCREMENT, FeatureName TEXT, ProductID INTEGER);"
        Using cmd As New SQLiteCommand(queryFeatures, conn)
            cmd.ExecuteNonQuery()
        End Using

        ' Create Attributes table
        Dim queryAtributes As String = "CREATE TABLE IF NOT EXISTS Attributes (AttributeID INTEGER PRIMARY KEY AUTOINCREMENT, AttributeName TEXT, ProductID INTEGER);"
        Using cmd As New SQLiteCommand(queryAtributes, conn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Sub InsertNewProduct()
        Dim publickey, privatekey As String
        ' Initialize the form for new product details
        Dim NewProduct As New NewProductfrm
        NewProduct.Text = "Enter Product Details"

        ' Show the form as a dialog and check if the result is OK
        If NewProduct.ShowDialog() = DialogResult.OK Then
            ' Retrieve the product name and password from the form
            Dim productName As String = NewProduct.NewProductName
            Dim productPassword As String = NewProduct.NewProductPassword

            ' Check if the product already exists in the database
            If ProductExists(productName) Then
                MessageBox.Show("A product with the same name already exists. Please choose a different name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Generate encryption keys for the product
            Dim KeyGenerator = Security.Cryptography.KeyGenerator.Create()
            Dim KeyPair = KeyGenerator.GenerateKeyPair()

            ' Encrypt and store the private key using the product password
            privatekey = KeyPair.ToEncryptedPrivateKeyString(productPassword.Trim())
            ' Store the public key in plain format
            publickey = KeyPair.ToPublicKeyString()

            ' Insert the new product into the database
            Dim insertProductQuery As String = "INSERT INTO Products (ProductName, ProductPassword) VALUES (@ProductName, @ProductPassword);"
            Using cmdProduct As New SQLiteCommand(insertProductQuery, conn)
                cmdProduct.Parameters.AddWithValue("@ProductName", productName)
                cmdProduct.Parameters.AddWithValue("@ProductPassword", EncodeToBase64(productPassword))
                cmdProduct.ExecuteNonQuery()
            End Using

            ' Retrieve the automatically generated ProductID
            Dim queryLastID As String = "SELECT last_insert_rowid();"
            Using cmdLastID As New SQLiteCommand(queryLastID, conn)
                Dim productID As Integer = Convert.ToInt32(cmdLastID.ExecuteScalar())

                ' Insert the private and public keys into the database using the retrieved ProductID
                Dim insertPrivateKeyQuery As String = "INSERT INTO PrivateKeys (ProductID, PrivateKey) VALUES (@ProductID, @PrivateKey)"
                Using cmdPrivate As New SQLiteCommand(insertPrivateKeyQuery, conn)
                    cmdPrivate.Parameters.AddWithValue("@ProductID", productID)
                    cmdPrivate.Parameters.AddWithValue("@PrivateKey", EncodeToBase64(privatekey))
                    cmdPrivate.ExecuteNonQuery()
                End Using

                Dim insertPublicKeyQuery As String = "INSERT INTO PublicKeys (ProductID, PublicKey) VALUES (@ProductID, @PublicKey)"
                Using cmdPublic As New SQLiteCommand(insertPublicKeyQuery, conn)
                    cmdPublic.Parameters.AddWithValue("@ProductID", productID)
                    cmdPublic.Parameters.AddWithValue("@PublicKey", EncodeToBase64(publickey))
                    cmdPublic.ExecuteNonQuery()
                End Using
            End Using
            MessageBox.Show("Product and keys successfully saved")
        Else
            MessageBox.Show("Operation cancelled or input was invalid.", "Operation Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Function ProductExists(productName As String) As Boolean
        ' Check if a product already exists in the database by name
        Dim query As String = "SELECT COUNT(*) FROM Products WHERE ProductName = @ProductName"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductName", productName)
            Dim result As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return result > 0
        End Using
    End Function

    Function UserExists(userName As String) As Boolean
        ' Check if a user already exists in the database by username
        Dim query As String = "SELECT COUNT(*) FROM Users WHERE User = @User"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@User", userName)
            Dim result As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return result > 0
        End Using
    End Function

    Function FeatureExists(productID As Integer, featureName As String) As Boolean
        ' Check if a feature already exists for a given product by feature name
        Dim query As String = "SELECT COUNT(*) FROM Features WHERE ProductID = @ProductID AND FeatureName = @FeatureName"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            cmd.Parameters.AddWithValue("@FeatureName", featureName)
            Dim result As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return result > 0
        End Using
    End Function

    Function AttributeExists(productID As Integer, attributeName As String) As Boolean
        ' Check if an attribute already exists for a given product by attribute name
        Dim query As String = "SELECT COUNT(*) FROM Attributes WHERE ProductID = @ProductID AND AttributeName = @AttributeName"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            cmd.Parameters.AddWithValue("@AttributeName", attributeName)
            Dim result As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return result > 0
        End Using
    End Function
    Function GetProductIDByProductName(productName As String) As Integer
        ' Retrieve the ProductID based on product name
        Dim query As String = "SELECT ProductID FROM Products WHERE ProductName = @ProductName"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductName", productName)
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then
                Return Convert.ToInt32(result)
            Else
                Return -1
            End If
        End Using
    End Function

    Function AddFeatureToDatabase(productID As Integer, featureName As String) As Boolean
        ' Insert a new feature into the database
        Dim query As String = "INSERT INTO Features (FeatureName, ProductID) VALUES (@FeatureName, @ProductID)"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@FeatureName", featureName)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Try
                cmd.ExecuteNonQuery()
                Return True
            Catch ex As Exception
                MessageBox.Show("Error inserting the feature: " & ex.Message)
                Return False
            End Try
        End Using
    End Function

    Function DeleteFeatureFromDatabase(productID As Integer, featureName As String) As Boolean
        ' Delete a feature from the database
        Dim query As String = "DELETE FROM Features WHERE FeatureName = @FeatureName AND ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@FeatureName", featureName)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Try
                Dim result As Integer = cmd.ExecuteNonQuery()
                If result > 0 Then
                    MessageBox.Show("Feature deleted successfully.")
                    Return True
                Else
                    MessageBox.Show("The specified feature was not found to delete.")
                    Return False
                End If
            Catch ex As Exception
                MessageBox.Show("Error deleting the feature: " & ex.Message)
                Return False
            End Try
        End Using
    End Function

    Function DeleteAttributeFromDatabase(productID As Integer, attributeName As String) As Boolean
        ' Delete an attribute from the database
        Dim query As String = "DELETE FROM Attributes WHERE AttributeName = @AttributeName AND ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@AttributeName", attributeName)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Try
                Dim result As Integer = cmd.ExecuteNonQuery()
                If result > 0 Then
                    MessageBox.Show("Attribute deleted successfully.")
                    Return True
                Else
                    MessageBox.Show("The specified attribute was not found to delete.")
                    Return False
                End If
            Catch ex As Exception
                MessageBox.Show("Error deleting the attribute: " & ex.Message)
                Return False
            End Try
        End Using
    End Function

    Sub CheckDatabase()
        ' Verify if the database file exists and create it if it does not
        If Not File.Exists(dbFileName) Then
            SQLiteConnection.CreateFile(dbFileName)  ' Create a new database file
            CreateTables()  ' Create all necessary tables
            InsertDefaultUser()  ' Insert a default admin user
        End If
    End Sub

    Sub InsertDefaultUser()
        ' Insert a default user into the database
        Dim insertQuery As String = "INSERT INTO Users (User, Password, IsAdmin, EditClients, EditProducts, DeleteClients, DeleteProducts, ExportLics, EditLics, ExportKeys) VALUES ('Administrator', @Supervisor, 1, 0, 0, 0, 0, 0, 0, 0)"
        Using cmd As New SQLiteCommand(insertQuery, conn)
            cmd.Parameters.AddWithValue("@Supervisor", EncodeToBase64("Supervisor"))
            cmd.ExecuteNonQuery()  ' Execute the insertion
        End Using
        MessageBox.Show("No database found. New one was created")
        MessageBox.Show($"DEFAULTS: {vbCrLf} *Username: Administrator {vbCrLf} *Password: Supervisor")
    End Sub

    Function AuthenticateUser(username As String, password As String) As Boolean
        ' Authenticate user based on username and password
        Dim query As String = "SELECT * FROM Users WHERE User = @User AND Password = @Password"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@User", username)
            cmd.Parameters.AddWithValue("@Password", EncodeToBase64(password))
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    SignedUser = username  ' Set the globally signed-in user
                    Return True  ' User found
                End If
            End Using
        End Using
        Return False  ' User not found
    End Function

    Function LoadPrivateKeyFromDatabase(ByRef productid As Integer) As String
        Dim privatekey As String
        ' Load keys based on the SelectedProductID
        Dim query As String = "SELECT PrivateKey FROM PrivateKeys WHERE ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productid)  ' Ensure the correct ProductID is used

            Dim reader As SQLiteDataReader = cmd.ExecuteReader()
            If reader.Read() Then
                privatekey = DecodeFromBase64(Convert.ToString(reader("PrivateKey")))  ' Load the private key
                Return privatekey
            Else
                MessageBox.Show("Private key not found for the selected product.")
                Return ""
            End If
        End Using
    End Function
    Function LoadPublicKeyFromDatabase(ByRef productid As Integer) As String
        Dim publickey As String
        ' Load the public key in a similar manner
        Dim query As String = "SELECT PublicKey FROM PublicKeys WHERE ProductID = @ProductID"

        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productid)

            Dim reader As SQLiteDataReader = cmd.ExecuteReader()
            If reader.Read() Then
                publickey = DecodeFromBase64(Convert.ToString(reader("PublicKey")))  ' Load the public key
                Return publickey
            Else
                MessageBox.Show("Public key not found for the selected product.")
                Return ""
            End If
        End Using
    End Function

    Function LicenseExists(clientID As Integer, productID As Integer) As Boolean
        ' Check if a license already exists for a given client and product
        Dim query As String = "SELECT COUNT(*) FROM Licenses WHERE ClientID = @ClientID AND ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ClientID", clientID)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Dim result As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return result > 0
        End Using
    End Function

    Function GetAttributesDictionary(productID As Integer) As Dictionary(Of String, String)
        ' Retrieve all attribute names for a specific product and initialize their values to "0"
        Dim attributes As New Dictionary(Of String, String)
        Dim query As String = "SELECT AttributeName FROM Attributes WHERE ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim attributeName As String = reader("AttributeName").ToString()
                    attributes.Add(attributeName, "0")
                End While
            End Using
        End Using
        Return attributes
    End Function

    Function FindAndCreateClient(clientName As String, clientemail As String) As Integer
        ' Try to find an existing client or create a new one if not found
        Dim transaction = conn.BeginTransaction()
        Try
            ' Attempt to find client by name and email
            Dim findClientQuery As String = "SELECT ClientID FROM Clients WHERE Name = @ClientName AND Email = @ClientEmail;"
            Using cmd As New SQLiteCommand(findClientQuery, conn, transaction)
                cmd.Parameters.AddWithValue("@ClientName", clientName)
                cmd.Parameters.AddWithValue("@ClientEmail", clientemail)
                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        ' Client found
                        Dim clientId As Integer = Convert.ToInt32(reader("ClientID"))
                        transaction.Commit()
                        Return clientId
                    End If
                End Using
            End Using

            ' Client not found, insert new client
            Dim insertClientQuery As String = "INSERT INTO Clients (Name, Email, ProductCount, CreatedBy, CreatedTime) VALUES (@ClientName, @ClientEmail, 0, @CreatedBy, @CreatedTime); SELECT last_insert_rowid();"
            Using insertCmd As New SQLiteCommand(insertClientQuery, conn, transaction)
                insertCmd.Parameters.AddWithValue("@ClientName", clientName)
                insertCmd.Parameters.AddWithValue("@ClientEmail", clientemail)
                insertCmd.Parameters.AddWithValue("@CreatedBy", SignedUser)
                insertCmd.Parameters.AddWithValue("@CreatedTime", DateTime.Now)
                Dim newClientId As Integer = Convert.ToInt32(insertCmd.ExecuteScalar())
                transaction.Commit()
                Return newClientId
            End Using
        Catch ex As Exception
            transaction.Rollback()
            Throw
        End Try
    End Function

    Function GetProductPasswordByID(productID As Integer) As String
        ' Retrieve the password for a specific product
        Dim query As String = "SELECT ProductPassword FROM Products WHERE ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Return DecodeFromBase64(reader("ProductPassword").ToString())
                End If
            End Using
        End Using
        Return String.Empty
    End Function

    Sub SaveLicenseInfo(licenseId As String, clientId As Integer, licenseFileName As String, ExpiresAt As Date, productid As Integer)
        ' Insert license details into the Licenses table and associated license file details into LicenseFiles table
        Dim queryLicenses As String = "INSERT INTO Licenses (LicenseID, ClientID, ProductID, CreatedBy, IssueDate, Expiration) VALUES (@LicenseID, @ClientID, @ProductID, @CreatedBy, @IssueDate, @Expiration);"
        Dim queryLicenseFiles As String = "INSERT INTO LicenseFiles (LicenseID, LicenseFile) VALUES (@LicenseID, @LicenseFile);"
        Using cmd As New SQLiteCommand(queryLicenses, conn)
            cmd.Parameters.AddWithValue("@LicenseID", licenseId)
            cmd.Parameters.AddWithValue("@ClientID", clientId)
            cmd.Parameters.AddWithValue("@ProductID", productid)
            cmd.Parameters.AddWithValue("@CreatedBy", SignedUser)
            cmd.Parameters.AddWithValue("@IssueDate", DateTime.Now)
            cmd.Parameters.AddWithValue("@Expiration", ExpiresAt)
            cmd.ExecuteNonQuery()
        End Using
        Using cmd As New SQLiteCommand(queryLicenseFiles, conn)
            cmd.Parameters.AddWithValue("@LicenseID", licenseId)
            cmd.Parameters.AddWithValue("@LicenseFile", licenseFileName)
            cmd.ExecuteNonQuery()
        End Using
        ' Update product count for the client
        Dim findClientQuery As String = "SELECT ProductCount FROM Clients WHERE ClientID = @ClientID;"
        Using cmd As New SQLiteCommand(findClientQuery, conn)
            cmd.Parameters.AddWithValue("@ClientID", clientId)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Dim productCount As Integer = Convert.ToInt32(reader("ProductCount")) + 1
                    Dim updateClientQuery As String = "UPDATE Clients SET ProductCount = @ProductCount WHERE ClientID = @ClientID;"
                    Using updateCmd As New SQLiteCommand(updateClientQuery, conn)
                        updateCmd.Parameters.AddWithValue("@ProductCount", productCount)
                        updateCmd.Parameters.AddWithValue("@ClientID", clientId)
                        updateCmd.ExecuteNonQuery()
                    End Using
                End If
            End Using
        End Using
    End Sub

    Function GetUniqueID() As Integer
        ' Retrieve the highest ClientID from the Clients table and increment it by 1 for the new client
        Dim query As String = "SELECT MAX(ClientID) FROM Clients;"
        Using cmd As New SQLiteCommand(query, conn)
            Dim result = cmd.ExecuteScalar()
            If IsDBNull(result) Then
                Return 1  ' Start from 1 if no rows are found
            Else
                Return Convert.ToInt32(result) + 1  ' Return the next available ID
            End If
        End Using
    End Function

    Function GetLicenseIDByClientID(clientID As Integer, productid As Integer) As String
        ' Fetch the LicenseID for a given ClientID and ProductID
        Dim query As String = "SELECT LicenseID FROM Licenses WHERE ClientID = @ClientID AND ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ClientID", clientID)
            cmd.Parameters.AddWithValue("@ProductID", productid)
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then
                Return result.ToString()
            Else
                Return String.Empty
            End If
        End Using
    End Function

    Function GetLicenseFileByLicenseID(licenseID As String) As String
        ' Retrieve the file path for a given LicenseID from the LicenseFiles table
        Dim query As String = "SELECT LicenseFile FROM LicenseFiles WHERE LicenseID = @LicenseID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@LicenseID", licenseID)
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then
                Return result.ToString()
            Else
                Return String.Empty
            End If
        End Using
    End Function

    Sub UpdateLicenseInfo(licenseId As String, clientId As Integer, licenseFileName As String, ExpiresAt As Date, productid As Integer)
        ' Update license information in the Licenses table and insert the new license file
        Dim queryLicenses As String = "UPDATE Licenses SET LicenseID = @LicenseID, ProductID = @ProductID, CreatedBy = @CreatedBy, IssueDate = @IssueDate, Expiration = @Expiration WHERE ClientID = @ClientID AND ProductID = @ProductID;"
        Dim queryLicenseFiles As String = "INSERT INTO LicenseFiles (LicenseID, LicenseFile) VALUES (@LicenseID, @LicenseFile);"
        Using cmd As New SQLiteCommand(queryLicenses, conn)
            cmd.Parameters.AddWithValue("@LicenseID", licenseId)
            cmd.Parameters.AddWithValue("@ClientID", clientId)
            cmd.Parameters.AddWithValue("@ProductID", productid)
            cmd.Parameters.AddWithValue("@CreatedBy", SignedUser)
            cmd.Parameters.AddWithValue("@IssueDate", DateTime.Now)
            cmd.Parameters.AddWithValue("@Expiration", ExpiresAt)
            cmd.ExecuteNonQuery()
        End Using
        Using cmd As New SQLiteCommand(queryLicenseFiles, conn)
            cmd.Parameters.AddWithValue("@LicenseID", licenseId)
            cmd.Parameters.AddWithValue("@LicenseFile", licenseFileName)
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Function GetClientFields() As List(Of EditableKeyValuePair)
        ' Retrieves field information from the Clients table and returns it as a list of EditableKeyValuePairs
        Dim fields As New List(Of EditableKeyValuePair)
        Dim query As String = "PRAGMA table_info(Clients);"
        Using cmd As New SQLiteCommand(query, conn)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim fieldName As String = reader("name").ToString()
                    Dim defaultValue As Object = GetDefaultValueForField(fieldName)
                    fields.Add(New EditableKeyValuePair(fieldName, defaultValue))
                End While
            End Using
        End Using
        Return fields
    End Function

    Function GetDefaultValueForField(fieldName As String) As Object
        ' Returns the default value for a given field based on its name
        Select Case fieldName
            Case "ClientID"
                Return GetNextClientId()  ' Assumes this function computes the next available ID
            Case "ProductCount"
                Return 0
            Case "CreatedBy", "CreatedTime", "Comment", "Name", "Email", "Country", "State", "City", "Company", "Postal", "Phone", "Address"
                Return ""  ' Empty string for text fields
            Case Else
                Return DBNull.Value  ' Default value for any other unspecified field
        End Select
    End Function

    Function GetNextClientId() As Integer
        ' Retrieves the highest ClientID from the Clients table and increments it by 1 for a new client
        Dim nextId As Integer = 1  ' Default value if there are no clients
        Dim query As String = "SELECT MAX(ClientID) FROM Clients"
        Using cmd As New SQLiteCommand(query, conn)
            Dim result As Object = cmd.ExecuteScalar()
            If Not IsDBNull(result) Then
                nextId = Convert.ToInt32(result) + 1
            End If
        End Using
        Return nextId
    End Function

    Function ClientExistsByEmail(clientId As Integer) As Boolean
        ' Checks if a client exists by their email
        Dim cmd As New SQLiteCommand("SELECT COUNT(*) FROM Clients WHERE ClientID = @ClientID", conn)
        cmd.Parameters.AddWithValue("@ClientID", clientId)
        Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
        Return count > 0
    End Function

    Sub UpdateClient(updatedData As List(Of EditableKeyValuePair), clientId As Integer)
        ' Updates client information in the Clients table based on provided data
        For Each item In updatedData
            Dim query As String = $"UPDATE Clients SET {item.Key} = @Value WHERE ClientID = @ClientID"
            Using cmd As New SQLiteCommand(query, conn)
                ' Check if the field is 'Phone' and apply the RemoveNonNumericCharacters function
                Dim value As Object = If(item.Key.Equals("Phone"), RemoveNonNumericCharacters(Convert.ToString(item.Value)), item.Value)
                cmd.Parameters.AddWithValue("@Value", value)
                cmd.Parameters.AddWithValue("@ClientID", clientId)
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Sub InsertNewClient(updatedData As List(Of EditableKeyValuePair))
        ' Constructs field and value strings for SQL query and executes an INSERT command
        Dim fields As String = String.Join(", ", updatedData.Select(Function(x) x.Key))
        Dim values As String = String.Join(", ", updatedData.Select(Function(x) "@" + x.Key))
        Dim query As String = $"INSERT INTO Clients ({fields}) VALUES ({values})"
        Using cmd As New SQLiteCommand(query, conn)
            ' Add parameters for each value in the list, applying special treatment for the 'Phone' field
            For Each item In updatedData
                Dim value As Object = If(item.Key.Equals("Phone"), RemoveNonNumericCharacters(Convert.ToString(item.Value)), item.Value)
                cmd.Parameters.AddWithValue("@" & item.Key, value)
            Next
            ' Execute the query
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Function RemoveNonNumericCharacters(input As String) As String
        Return Regex.Replace(input, "[^\d]", "")
    End Function

    Function EncodeToBase64(ByVal input As String) As String
        Dim bytesToEncode As Byte() = System.Text.Encoding.UTF8.GetBytes(input)
        Dim encodedText As String = System.Convert.ToBase64String(bytesToEncode)
        Return encodedText
    End Function

    Function DecodeFromBase64(ByVal input As String) As String
        Dim decodedBytes As Byte() = System.Convert.FromBase64String(input)
        Dim decodedText As String = System.Text.Encoding.UTF8.GetString(decodedBytes)
        Return decodedText
    End Function

End Module

' This class represents a key-value pair where the key is a string and the value is an object. It is used to easily manage data entries for operations like updating a database.
Public Class EditableKeyValuePair
    Public Property Key As String
    Public Property Value As Object

    ' Constructor to initialize a new instance of EditableKeyValuePair with a specified key and value.
    Public Sub New(key As String, value As Object)
        Me.Key = key
        Me.Value = value
    End Sub

End Class

' This class is used to represent items in a ComboBox, where each item has an ID and a Name.
Public Class ComboBoxItem
    Public Property ID As Integer
    Public Property Name As String

    ' Constructor to initialize a new instance of ComboBoxItem with a specified ID and name.
    Public Sub New(id As Integer, name As String)
        Me.ID = id
        Me.Name = name
    End Sub

    ' Overrides the ToString method to return the Name property when the ComboBox item is displayed.
    Public Overrides Function ToString() As String
        Return Name  ' This controls what is displayed in the ComboBox
    End Function

End Class
