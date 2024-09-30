Imports System.Data.SQLite
Imports System.IO
Imports System.Text.RegularExpressions
Imports Standard.Licensing
Imports Standard.Licensing.Validation

Module MainModule

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
    Public canOnlyTrials As Boolean
    Public canMaxDays As Integer
    Public conn As New SQLiteConnection(connectionString)

    Sub OpenSqliteConnection()
        ' Open SQLite Connection
        conn.Open()
    End Sub

    Sub CheckDatabase()
        ' Verify if the database file exists and create it if it does not
        If Not File.Exists(dbFileName) Then
            SQLiteConnection.CreateFile(dbFileName)  ' Create a new database file
            CreateTables()  ' Create all necessary tables
            InsertDefaultUser()  ' Insert a default admin user
        End If
    End Sub

    Sub CreateTables()
        OpenSqliteConnection()

        ' Create Users table
        Dim queryUsers As String = "CREATE TABLE IF NOT EXISTS Users (UserID INTEGER PRIMARY KEY AUTOINCREMENT, User TEXT, Password TEXT, IsAdmin INTEGER, EditClients INTEGER, EditProducts INTEGER, DeleteClients INTEGER, DeleteProducts INTEGER, ExportLics INTEGER, EditLics INTEGER, ExportKeys INTEGER, OnlyTrials INTEGER, MaxTrialDays INTEGER);"
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
        Dim queryAttributes As String = "CREATE TABLE IF NOT EXISTS Attributes (AttributeID INTEGER PRIMARY KEY AUTOINCREMENT, AttributeName TEXT, ProductID INTEGER);"
        Using cmd As New SQLiteCommand(queryAttributes, conn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Sub SetGlobalUserPermissions()
        ' Call the function to get the user permissions
        Dim permissions As Dictionary(Of String, Object) = GetUserPermissionsDictionary(SignedUser)

        If permissions.Count > 0 Then
            isAdmin = permissions("IsAdmin")
            canEditClients = permissions("EditClients")
            canEditProducts = permissions("EditProducts")
            canDeleteClients = permissions("DeleteClients")
            canDeleteProducts = permissions("DeleteProducts")
            canExportLics = permissions("ExportLics")
            canExportKeys = permissions("ExportKeys")
            canEditLics = permissions("EditLics")
            canOnlyTrials = permissions("OnlyTrials")
            canMaxDays = permissions("MaxTrialDays")
        Else
            MsgBox("Error loading user permissions")
        End If
    End Sub

    Public Function RemoveNonNumericCharacters(input As String) As String
        Return Regex.Replace(input, "[^\d]", "")
    End Function

    Function GetDefaultValueForField(fieldName As String) As Object
        ' Returns the default value for a given field based on its name
        Select Case fieldName
            Case "ClientID"
                Return GetNextClientId()
            Case "ProductCount"
                Return 0
            Case "CreatedBy", "CreatedTime", "Comment", "Name", "Email", "Country", "State", "City", "Company", "Postal", "Phone", "Address"
                Return ""  ' Empty string for text fields
            Case Else
                Return DBNull.Value  ' Default value for any other unspecified field
        End Select
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
