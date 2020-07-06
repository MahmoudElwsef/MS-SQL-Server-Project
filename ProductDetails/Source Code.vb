Imports System.Data.SqlClient

Public Class Form1

    Dim conn As SqlConnection
    Dim da As SqlDataAdapter
    Dim ds As DataSet

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Make connection to The SQL Server
        Dim strConnection As String = "Data Source=DESKTOP-MSMKMEG\ELWASEFSERVER ; initial Catalog=master ; integrated Security=true"
        conn = New SqlConnection(strConnection)
        conn.Open()

        'This outer Try used to Check Database 'ProductDB' exist or Not
        Try
            conn.ChangeDatabase("ProductDB") 'Change connection From Master to ProductDB

            'if Database 'ProductDB' Not exist
        Catch ex1 As Exception
            MsgBox(ex1.Message)

            Try
                'Create Database 'ProductDB'
                Dim command1 As New SqlCommand("Create database ProductDB", conn)
                command1.ExecuteNonQuery() 'To execute Query

                'Change connection From Master to ProductDB
                command1.Connection.ChangeDatabase("ProductDB")

                'Create Table 'Products'
                Dim command2 As New SqlCommand("Create table Products(ID int , name char(30) , Quantity int , unit_Price int , delar_Price int)", conn)
                command2.ExecuteNonQuery() 'To execute Query

            Catch ex2 As Exception
                MsgBox(ex2.Message)

            End Try

        End Try

        'To get Data From Database to DataAdapter
        da = New SqlDataAdapter("select * from Products", conn)

        'To Reconcile DataAdapter with DataSet
        Dim cb As New SqlCommandBuilder(da)

        'To Fill Data in Dataset From DataAdapter and give it a Name (This Name to Table in DataSet)
        ds = New DataSet()
        da.Fill(ds, "Products")

        'view Data in DataGridView From DataSet
        DataGridView1.DataBindings.Add("DataSource", ds, "Products")

    End Sub

    'This Button Used To Save data From TextBoxes to Original Database
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtnNewProduct.Click

        'make NewRow in Table 'Products' in DataSet
        Dim r As DataRow = ds.Tables("Products").NewRow()

        r("ID") = Val(TextBox1.Text)                    ' r(0) = Val(TextBox1.Text)
        r("name") = TextBox2.Text                       ' r(1) = TextBox2.Text
        r("Quantity") = TextBox3.Text                   ' r(2) = Val(TextBox3.Text)
        r("unit_Price") = Val(TextBox4.Text)            ' r(3) = Val(TextBox4.Text)
        r("delar_Price") = Val(TextBox5.Text)           ' r(4) = Val(TextBox5.Text)

        'Add Row in Table 'Products' in DataSet
        ds.Tables("Products").Rows.Add(r)

        'Add Row in Table 'Products' in DataAdapter(To affect the original Database)
        da.Update(ds, "Products")

    End Sub

    Private Sub BtnViewProduct_Click(sender As Object, e As EventArgs) Handles BtnViewProduct.Click

        'get Spacific Row From Database
        Dim command1 As New SqlCommand("Select * from Products where ID=" & Val(TextBox1.Text), conn)

        Try
            'execute and Reade Query
            Dim Reader As SqlDataReader = command1.ExecuteReader()
            Reader.Read()

            'show Row in TextBoxes
            TextBox2.Text = Reader(1)
            TextBox3.Text = Reader(2)
            TextBox4.Text = Reader(3)
            TextBox5.Text = Reader(4)

            'close connection because ..
            '    conn.Close()

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try

    End Sub

    Private Sub BtnDeleteProduct_Click(sender As Object, e As EventArgs) Handles BtnDeleteProduct.Click

        Dim answer As Integer
        Dim r As DataRow = Nothing

        For Each r In ds.Tables("Products").Rows

            If r("ID") = Val(TextBox1.Text) Then

                'show Row that user want to Delete in TextBoxes
                TextBox2.Text = r("name")
                TextBox3.Text = r("Quantity")
                TextBox4.Text = r("unit_Price")
                TextBox5.Text = r("delar_Price")

                'if we Found ID that user want to Delete
                Exit For

            End If

        Next

        answer = MsgBox("Are You Sure you Want To delete This Product?", MsgBoxStyle.OkCancel)
        '1 --> Yes
        '2 --> No

        If answer = 2 Then Exit Sub

        r.Delete()
        da.Update(ds, "Products")

    End Sub

    Private Sub BtnUpdateProduct_Click(sender As Object, e As EventArgs) Handles BtnUpdateProduct.Click

        Dim r As DataRow = Nothing

        For Each r In ds.Tables("Products").Rows

            If r("ID") = Val(TextBox1.Text) Then

                'get Updates From TextBoxes to Row 'r'
                r("name") = TextBox2.Text
                r("Quantity") = TextBox3.Text
                r("unit_Price") = TextBox4.Text
                r("delar_Price") = TextBox5.Text

                'if we Found ID that user want to Update
                Exit For

            End If

        Next

        da.Update(ds, "Products")

    End Sub

End Class

