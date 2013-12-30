Imports System.Xml.Linq


Partial Public Class LinhasPage
    Inherits PhoneApplicationPage

    Dim TempItem As PivotItem = Nothing, IsAdding As Boolean, IsSearch As Boolean
    Private Property Linhas As LinhasViewModel

    Public Sub New()
        InitializeComponent()
        Linhas = New LinhasViewModel()
        DataContext = Linhas
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
        MainPivot.Title = "TODOS " & If(Boolean.Parse(NavigationContext.QueryString("IsHorario")), "HORARIOS", "ITINERARIOS").ToString
    End Sub

#Region "Immediate Events"

    Private Sub Linha_Tap(sender As Object, e As Input.GestureEventArgs)
        App.ActiveLinha = TryCast(TryCast(sender, TiltableControl).DataContext, Linha)

        If IsAdding Then
            App.ProgressMsg(Progress, MainPage.Favoritos.AddFav(App.ActiveLinha), 2)
        Else
            NavigationService.Navigate(New Uri(If(Boolean.Parse(NavigationContext.QueryString("IsHorario")), "/Horarios", "/Itinerarios") & ".xaml", UriKind.Relative))
        End If
    End Sub

    Private Sub MenuItem_Tap(sender As Object, e As Input.GestureEventArgs)
        App.ActiveLinha = TryCast(TryCast(sender, MenuItem).DataContext, Linha)
        App.ProgressMsg(Progress, MainPage.Favoritos.AddFav(App.ActiveLinha), 2)
    End Sub

    Private Sub AppBar_Search(sender As Object, e As EventArgs)
        IsSearch = True

        TempItem = MainPivot.Items(1)
        MainPivot.Items.RemoveAt(1)

        CType(MainPivot.Items(0), PivotItem).Header = vbNullString
        SearchBox.Visibility = Windows.Visibility.Visible
        ApplicationBar.IsVisible = False

        SearchBox.Focus()

        NameList.IsFlatList = True
        NameList.ItemsSource = Linhas.List
    End Sub

    Private Sub AppBar_Add(sender As Object, e As EventArgs)
        IsAdding = True
        App.ProgressMsg(Progress, "Selecione os itens...", -0.001)

        ApplicationBar.Buttons.RemoveAt(0)
        ApplicationBar.Buttons.RemoveAt(0)

        Dim Btn As New ApplicationBarIconButton(New Uri("Assets/AppBar/check.png", UriKind.Relative)) With {.Text = "concluído"}
        AddHandler Btn.Click, AddressOf AppBar_Done
        ApplicationBar.Buttons.Add(Btn)
        Btn = Nothing
    End Sub

    Private Sub AppBar_Done(sender As Object, e As EventArgs)
        IsAdding = False
        Progress.IsVisible = False
        ApplicationBar.Buttons.RemoveAt(0)

        Dim Btn As New ApplicationBarIconButton(New Uri("Assets/AppBar/add.png", UriKind.Relative)) With {.Text = "favorito"}
        AddHandler Btn.Click, AddressOf AppBar_Add
        ApplicationBar.Buttons.Insert(0, Btn)

        Btn = New ApplicationBarIconButton(New Uri("Assets/AppBar/search.png", UriKind.Relative)) With {.Text = "pesquisar"}
        AddHandler Btn.Click, AddressOf AppBar_Search
        ApplicationBar.Buttons.Add(Btn)

        Btn = Nothing
    End Sub

#End Region

#Region "Other Events"

    Private Sub SearchBox_Changed(sender As Object, e As TextChangedEventArgs)

        Dispatcher.BeginInvoke( _
            Sub()

                NameList.ItemsSource = Linhas.List.Where( _
                    Function(l)

                        Dim Text As String = If(SearchBox.Text.Length > 1, l.COD, String.Empty) & l.NOME
                        Dim Diacritics() As Char = ("âêôûÂÊÔÛáéíóúÁÉÍÓÚãõÃÕçÇ").ToCharArray
                        Dim Clean() As Char = ("AEOUAEOUAEIOUAEIOUAOAOCC").ToCharArray

                        For i As Integer = 0 To Diacritics.GetUpperBound(0)
                            Text = Text.Replace(Diacritics(i), Clean(i))
                        Next

                        Return Text.Contains(SearchBox.Text.ToUpper)

                    End Function).ToList

            End Sub)

    End Sub

    Private Sub LinhasPage_BackKeyPress(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.BackKeyPress
        If IsAdding Then
            e.Cancel = True
            AppBar_Done(Me, Nothing)
        ElseIf IsSearch Then
            e.Cancel = True
            UnSearch()
        End If
    End Sub

#End Region

#Region "Functions and Callbacks"

    Public Sub UnSearch()
        IsSearch = False
        MainPivot.Items.Add(TempItem)
        SearchBox.Visibility = Windows.Visibility.Collapsed
        ApplicationBar.IsVisible = True

        CType(MainPivot.Items(0), PivotItem).Header = "nome"

        NameList.IsFlatList = False
        NameList.ItemsSource = Linhas.LinhasKey
        SearchBox.Text = String.Empty
    End Sub

#End Region

End Class

Public Class LinhasViewModel

    Public Property List As IEnumerable(Of Linha)
    Public Property LinhasKey As List(Of AlphaKeyGroup(Of Linha))
    Public Property LinhasType As List(Of KeyedList(Of String, Linha))

    Public Sub New()

        If List Is Nothing Then

            List = From x In XElement.Load("Data/Linhas.xml").<l>
                   Select New Linha() With {.COD = x.<c>.Value, .NOME = x.<n>.Value, .TIPO = x.<t>.Value}

            LinhasKey = AlphaKeyGroup(Of Linha).CreateGroups(List, Nothing, Function(b As Linha) b.NOME, False)

            LinhasType = (From l In List Order By LinhasTipo(l.TIPO)
                          Group By Tipo = LinhasTipo(l.TIPO) Into Tipos = Group
                          Select New KeyedList(Of String, Linha)(Tipo, Tipos)).ToList
        End If

    End Sub

    Public Function LinhasTipo(TIPO As Char) As String
        Select Case TIPO
            Case "A" : Return "ALIMENTADOR"
            Case "B" : Return "INTERBAIRROS"
            Case "C" : Return "CONVENCIONAL/TRONCAL"
            Case "D" : Return "EXPRESSO"
            Case "E" : Return "EXPRESSO LIGEIRÃO"
            Case "H" : Return "INTERHOSPITAIS"
            Case "I" : Return "CIRCULAR CENTRO"
            Case "L" : Return "LIGEIRINHO"
            Case "M" : Return "MADRUGUEIRO"
            Case "S" : Return "SITES"
            Case "T" : Return "LINHA TURISMO"
            Case "X" : Return "EXECUTIVO AEROPORTO"
            Case Else : Return "OUTROS"
        End Select
    End Function

End Class