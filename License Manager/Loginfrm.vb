Imports System.Data.SQLite
Imports System.IO

Public Class Loginfrm

    ' This method is called when the login form loads.
    Private Sub Loginfrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Check if the database exists and create tables and a default user if necessary.
        CheckDatabase()

        ' Ensure the SQLite connection is open.
        If conn.State <> ConnectionState.Open Then
            OpenSqliteConnection()
        End If

        ' Check if the directory for storing licenses exists; if not, create it.
        If Not Directory.Exists(LicenseDirectory) Then
            Directory.CreateDirectory(LicenseDirectory)
        End If
    End Sub

    ' This method handles the click event of the OK button.
    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        ' Attempt to authenticate the user.
        If AuthenticateUser(UsernameTextBox.Text, PasswordTextBox.Text) Then
            ' If authentication is successful, show the main form and close the login form.
            Mainfrm.Show()
            Me.Close()
        Else
            ' If authentication fails, show an error message.
            MessageBox.Show("Invalid username or password")
        End If
    End Sub

    ' This method handles the click event of the Cancel button.
    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        ' Close the login form.
        Me.Close()
    End Sub


End Class
