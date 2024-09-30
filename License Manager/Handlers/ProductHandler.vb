Imports System.Data.SQLite
Imports Standard.Licensing

Public Module ProductHandler

    ' Returns a list of products with their names and IDs from the database.
    Function GetProducts() As List(Of Object)
        Dim products As New List(Of Object)
        Dim query As String = "SELECT ProductID, ProductName FROM Products;"

        Using cmd As New SQLiteCommand(query, conn)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim productID As Integer = Convert.ToInt32(reader("ProductID"))
                    Dim productName As String = reader("ProductName").ToString()

                    ' Store the product as an anonymous object with Name and ID properties
                    products.Add(New With {.Name = productName, .ID = productID})
                End While
            End Using
        End Using

        Return products
    End Function

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
            If ProductHandler.ProductExists(productName) Then
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

    Function AddFeature(productID As Integer, featureName As String) As Boolean
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

    Function DeleteFeature(productID As Integer, featureName As String) As Boolean
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

    Function AddAttribute(productID As Integer, attributeName As String) As Boolean
        ' Add an attribute from the database
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

    Function DeleteAttribute(productID As Integer, attributeName As String) As Boolean
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

    Function LoadPrivateKey(ByRef productid As Integer) As String
        Dim privatekey As String
        ' Load keys based on the SelectedProductID
        Dim query As String = "SELECT PrivateKey FROM PrivateKeys WHERE ProductID = @ProductID"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productid)

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
    Function LoadPublicKey(ByRef productid As Integer) As String
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

    ' Returns a list of features from the database for the given product.
    Function GetFeaturesList(productID As Integer) As List(Of String)
        Dim features As New List(Of String)
        Dim query As String = "SELECT FeatureName FROM Features WHERE ProductID = @ProductID"

        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    features.Add(reader("FeatureName").ToString())
                End While
            End Using
        End Using

        Return features
    End Function

    ' Returns a list of features from the database for the given product.
    Function GetAttributesList(productID As Integer) As List(Of String)
        Dim attributes As New List(Of String)
        Dim query As String = "SELECT AttributeName FROM Attributes WHERE ProductID = @ProductID"

        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    attributes.Add(reader("AttributeName").ToString())
                End While
            End Using
        End Using

        Return attributes
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

    Sub UpdateLicenseInfoOnDatabase(licenseId As String, clientId As Integer, licenseFileName As String, ExpiresAt As Date, productid As Integer)
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

    Sub DeleteProductFromDatabase(productId As Integer)
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
    Sub ExecuteDeleteCommand(conn As SQLiteConnection, query As String, productId As Integer)
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ProductID", productId)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

End Module
