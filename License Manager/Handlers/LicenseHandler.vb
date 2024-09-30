Imports System.Data.SQLite
Imports Standard.Licensing
Imports Standard.Licensing.Validation

Public Module LicenseHandler

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


    Sub SaveLicenseInfoOnDatabase(licenseId As String, clientId As Integer, licenseFileName As String, ExpiresAt As Date, productid As Integer)
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

End Module
