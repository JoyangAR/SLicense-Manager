Imports System.Data.SQLite

Module ClientHandler

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

    ' Returns a list of clients with their names and IDs from the database.
    Function GetClients() As List(Of Object)
        Dim clients As New List(Of Object)
        Dim query As String = "SELECT DISTINCT Clients.ClientID, Clients.Name FROM Clients ORDER BY Name;"

        Using cmd As New SQLiteCommand(query, conn)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim clientID As Integer = Convert.ToInt32(reader("ClientID"))
                    Dim clientName As String = reader("Name").ToString()

                    ' Store the client as an anonymous object with Name and ID properties
                    clients.Add(New With {.Name = clientName, .ID = clientID})
                End While
            End Using
        End Using

        Return clients
    End Function

    ' Retrieves client data for the specified client ID and returns a list of EditableKeyValuePair.
    Function GetClientDataByClientID(clientId As Integer) As List(Of EditableKeyValuePair)
        Dim clientDetails As New List(Of EditableKeyValuePair)
        Dim query As String = "SELECT * FROM Clients WHERE ClientID = @ClientID"

        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@ClientID", clientId)

            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                ' Read client data if a record is found
                If reader.Read() Then
                    ' Loop through all fields and add them to the list as key-value pairs
                    For i As Integer = 0 To reader.FieldCount - 1
                        clientDetails.Add(New EditableKeyValuePair(reader.GetName(i), reader.GetValue(i)))
                    Next
                End If
            End Using
        End Using

        Return clientDetails
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

    Function ClientExistsByID(clientId As Integer) As Boolean
        Dim cmd As New SQLiteCommand("SELECT COUNT(*) FROM Clients WHERE ClientID = @ClientID", conn)
        cmd.Parameters.AddWithValue("@ClientID", clientId)
        Dim result As Object = cmd.ExecuteScalar()
        Return Convert.ToInt32(result) > 0
    End Function

    Function DeleteClientAndLicenses(clientId As Integer) As Boolean
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

    Function GetUniqueClientID() As Integer
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

End Module
