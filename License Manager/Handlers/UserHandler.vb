Imports System.Data.SQLite

Public Module UserHandler

    Sub InsertDefaultUser()
        ' Insert a default user into the database
        Dim insertQuery As String = "INSERT INTO Users (User, Password, IsAdmin, EditClients, EditProducts, DeleteClients, DeleteProducts, ExportLics, EditLics, ExportKeys, OnlyTrials, MaxTrialDays) VALUES ('Administrator', @Supervisor, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0)"
        Using cmd As New SQLiteCommand(insertQuery, conn)
            cmd.Parameters.AddWithValue("@Supervisor", EncodeToBase64("Supervisor"))
            cmd.ExecuteNonQuery()  ' Execute the insertion
        End Using
        MessageBox.Show("No database found. New one was created")
        MessageBox.Show($"DEFAULTS:{vbCrLf} *Username: Administrator{vbCrLf} *Password: Supervisor")
    End Sub

    Function AuthenticateUser(username As String, password As String) As Boolean
        ' Authenticate user based on username and password
        Dim query As String = "SELECT * FROM Users WHERE User = @User AND Password = @Password"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@User", username)
            cmd.Parameters.AddWithValue("@Password", EncodeToBase64(password))
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Return True  ' User found
                End If
            End Using
        End Using
        Return False  ' User not found
    End Function

    Sub UpdateUserPermissionBool(user As String, permission As String, value As Boolean)

        Dim query As String = $"UPDATE Users SET {permission} = @Value WHERE User = @UserName"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@Value", value)
            cmd.Parameters.AddWithValue("@UserName", user)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Sub UpdateUserPermisionInt(user As String, permission As String, value As Integer)

        Dim query As String = $"UPDATE Users SET {permission} = @Value WHERE User = @UserName"
        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@Value", value)
            cmd.Parameters.AddWithValue("@UserName", user)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Function DeleteUser(user As String) As Boolean
        ' Delete user
        Try
            Dim deleteQuery As String = "DELETE FROM Users WHERE User = @User;"
            Using cmd As New SQLiteCommand(deleteQuery, conn)
                cmd.Parameters.AddWithValue("@User", user)
                cmd.ExecuteNonQuery()
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

    Function UpdatePassword(user As String, password As String) As Boolean
        ' Update user's password
        Try
            Dim updateQuery As String = "UPDATE Users SET Password = @Password WHERE User = @User;"
            Using cmd As New SQLiteCommand(updateQuery, conn)
                cmd.Parameters.AddWithValue("@Password", EncodeToBase64(password))
                cmd.Parameters.AddWithValue("@User", user)
                cmd.ExecuteNonQuery()
            End Using
            Return True
        Catch
            Return False
        End Try
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

    Function InsertUser(user As String, password As String) As Boolean
        ' Insert new user
        Try
            Dim insertQuery As String = "INSERT INTO Users (User, Password, IsAdmin, EditClients, EditProducts, DeleteClients, DeleteProducts, ExportLics, EditLics, ExportKeys, OnlyTrials, MaxTrialDays) VALUES (@User, @Password, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)"
            Using cmd As New SQLiteCommand(insertQuery, conn)
                cmd.Parameters.AddWithValue("@User", user)
                cmd.Parameters.AddWithValue("@Password", EncodeToBase64(password))
                cmd.ExecuteNonQuery()
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

    ' Returns a list of user names from the database.
    Function GetUsers() As List(Of String)
        Dim users As New List(Of String)
        Dim query As String = "SELECT User FROM Users"

        Using cmd As New SQLiteCommand(query, conn)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                ' Read each record and add the user names to the list.
                While reader.Read()
                    users.Add(reader("User").ToString())
                End While
            End Using
        End Using

        Return users
    End Function

    ' Retrieves user permissions from the database based on the provided userName.
    Function GetUserPermissionsDictionary(userName As String) As Dictionary(Of String, Object)
        Dim permissions As New Dictionary(Of String, Object)
        Dim query As String = "SELECT IsAdmin, EditClients, EditProducts, DeleteClients, DeleteProducts, ExportLics, ExportKeys, EditLics, OnlyTrials, MaxTrialDays FROM Users WHERE User = @User"

        Using cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@User", userName)

            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Store the permission values in the dictionary
                    permissions("IsAdmin") = Convert.ToBoolean(reader("IsAdmin"))
                    permissions("EditClients") = Convert.ToBoolean(reader("EditClients"))
                    permissions("EditProducts") = Convert.ToBoolean(reader("EditProducts"))
                    permissions("DeleteClients") = Convert.ToBoolean(reader("DeleteClients"))
                    permissions("DeleteProducts") = Convert.ToBoolean(reader("DeleteProducts"))
                    permissions("EditLics") = Convert.ToBoolean(reader("EditLics"))
                    permissions("ExportLics") = Convert.ToBoolean(reader("ExportLics"))
                    permissions("ExportKeys") = Convert.ToBoolean(reader("ExportKeys"))
                    permissions("OnlyTrials") = Convert.ToBoolean(reader("OnlyTrials"))
                    permissions("MaxTrialDays") = reader("MaxTrialDays").ToString()
                End If
            End Using
        End Using

        Return permissions
    End Function

End Module
