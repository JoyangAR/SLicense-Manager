Public Class NewProductfrm
    Public Property NewProductName As String
    Public Property NewProductPassword As String

    ' Handles the event when the Save button is clicked.
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        ' Retrieve the product name and password from the respective text boxes.
        NewProductName = textBoxProductName.Text
        NewProductPassword = textBoxProductPassword.Text

        ' Check if either the product name or password is empty or whitespace.
        If String.IsNullOrWhiteSpace(NewProductName) Or String.IsNullOrWhiteSpace(NewProductPassword) Then
            ' If either field is empty, show an error message.
            MessageBox.Show("Both fields are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            ' If both fields are filled, set the dialog result to OK and close the form.
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub

    ' Handles the KeyDown event for the textBoxProductPassword to allow submission with the Enter key.
    Private Sub textBoxProductPassword_KeyDown(sender As Object, e As KeyEventArgs) Handles textBoxProductPassword.KeyDown
        ' Check if the Enter key was pressed.
        If e.KeyCode = Keys.Enter Then
            ' Suppress the beep sound when pressing Enter.
            e.SuppressKeyPress = True

            ' Retrieve the product name and password from the text boxes.
            NewProductName = textBoxProductName.Text
            NewProductPassword = textBoxProductPassword.Text

            ' Check if either the product name or password is empty or whitespace.
            If String.IsNullOrWhiteSpace(NewProductName) Or String.IsNullOrWhiteSpace(NewProductPassword) Then
                ' Show an error message if either field is empty.
                MessageBox.Show("Both fields are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                ' If both fields are filled, set the dialog result to OK and close the form.
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End If
        End If
    End Sub
End Class