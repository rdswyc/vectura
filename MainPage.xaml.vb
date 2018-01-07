Imports System.Collections.ObjectModel
Imports System.IO.IsolatedStorage.IsolatedStorageSettings
Imports Microsoft.Phone.Tasks


Partial Public Class MainPage
    Inherits PhoneApplicationPage

    Private Shared _favoritos As MainViewModel = Nothing
    Public Shared ReadOnly Property Favoritos() As MainViewModel
        Get
            If _favoritos Is Nothing Then _favoritos = New MainViewModel()
            Return _favoritos
        End Get
    End Property

    Dim IsHorario As Boolean = True

    Public Sub New()
        InitializeComponent()
        DataContext = Me
        App.ProgressMsg(Progress, "Olá. Seja bem vindo!", 3)
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
        UpdateUI()
    End Sub

#Region "Immediate Events"

    Private Sub Favorito_Tap(sender As Object, e As Input.GestureEventArgs)
        If Not MultiList.IsSelectionEnabled Then
            App.ActiveLinha = TryCast(TryCast(sender, TiltableControl).DataContext, Linha)
            NavigationService.Navigate(New Uri("/" & If(IsHorario, "Horarios", "Itinerarios") & ".xaml", UriKind.Relative))
        End If
    End Sub

    Private Sub Linhas_Tap(sender As Object, e As Input.GestureEventArgs)
        NavigationService.Navigate(New Uri("/LinhasPage.xaml?IsHorario=" & IsHorario, UriKind.Relative))
    End Sub

    Private Sub AppBar_Del(sender As Object, e As EventArgs)
        If MultiList.IsSelectionEnabled Then
            Dim Temp As List(Of Linha) = (From l In MultiList.SelectedItems Select CType(l, Linha)).ToList
            For i = Temp.Count - 1 To 0 Step -1
                App.ProgressMsg(Progress, Favoritos.DelFav(Temp(i)), 2)
            Next
            UpdateUI()
            Temp = Nothing
        Else
            MultiList.IsSelectionEnabled = True
        End If
    End Sub

    Private Sub AppBar_Change(sender As Object, e As EventArgs)
        IsHorario = Not IsHorario
        CType(MainPano.Items(0), PanoramaItem).Header = If(IsHorario, "horários", "itinerários")
    End Sub

    Private Sub AppBar_Settings(sender As Object, e As EventArgs)
        MessageBox.Show("Rodrigo Chin" & vbLf & "rdswyc@hotmail.com", "Contato", MessageBoxButton.OK)
    End Sub

    Private Sub MenuItem_Tap(sender As Object, e As Input.GestureEventArgs)
        App.ActiveLinha = TryCast(TryCast(sender, MenuItem).DataContext, Linha)
        App.ProgressMsg(Progress, Favoritos.DelFav(App.ActiveLinha), 2)
    End Sub

    Private Sub Marketplace_Tap(sender As Object, e As Input.GestureEventArgs)
        Dim MarketTask As New MarketplaceReviewTask()
        MarketTask.Show()
    End Sub

#End Region

#Region "Other Events"

    Private Sub MainPano_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles MainPano.SelectionChanged

        If Not MultiList.IsSelectionEnabled Then

            Dim Btn As ApplicationBarIconButton

            If MainPano.SelectedIndex = 0 Then
                ApplicationBar.Buttons.RemoveAt(0)

                Btn = New ApplicationBarIconButton(New Uri("Assets/AppBar/change.png", UriKind.Relative)) With {.Text = "alternar"}
                AddHandler Btn.Click, AddressOf AppBar_Change
                ApplicationBar.Buttons.Add(Btn)

                Btn = New ApplicationBarIconButton(New Uri("Assets/AppBar/delete.png", UriKind.Relative)) With {.Text = "remover"}
                AddHandler Btn.Click, AddressOf AppBar_Del
                ApplicationBar.Buttons.Add(Btn)
            Else
                ApplicationBar.Buttons.RemoveAt(0)
                ApplicationBar.Buttons.RemoveAt(0)

                Btn = New ApplicationBarIconButton(New Uri("Assets/AppBar/email.png", UriKind.Relative)) With {.Text = "contato"}
                AddHandler Btn.Click, AddressOf AppBar_Settings
                ApplicationBar.Buttons.Add(Btn)
            End If

            Btn = Nothing

        End If

    End Sub

    Private Sub MultiList_IsSelectionEnabledChanged(sender As Object, e As DependencyPropertyChangedEventArgs) Handles MultiList.IsSelectionEnabledChanged

        If MainPano.SelectedIndex = 0 Then

            If e.NewValue Then
                App.ProgressMsg(Progress, "Selecione os itens...", -0.001)
                TodasLinhas.Visibility = Windows.Visibility.Collapsed
                ApplicationBar.Buttons.RemoveAt(0)
            Else
                Progress.IsVisible = False
                TodasLinhas.Visibility = Windows.Visibility.Visible

                Dim Btn As New ApplicationBarIconButton(New Uri("Assets/AppBar/change.png", UriKind.Relative)) With {.Text = "alternar"}
                AddHandler Btn.Click, AddressOf AppBar_Change
                ApplicationBar.Buttons.Insert(0, Btn)
                Btn = Nothing
            End If

        End If

    End Sub

    Private Sub MainPage_BackKeyPress(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.BackKeyPress

        If MultiList.IsSelectionEnabled Then
            e.Cancel = True
            MultiList.IsSelectionEnabled = False
            Progress.IsVisible = False
        End If

    End Sub

#End Region

#Region "Functions and Callbacks"

    Public Sub UpdateUI()
        If Favoritos.Count Then
            EmptyMsg.Visibility = Windows.Visibility.Collapsed
            MultiList.Visibility = Windows.Visibility.Visible
        Else
            EmptyMsg.Visibility = Windows.Visibility.Visible
            MultiList.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

#End Region

End Class

Public Class MainViewModel
    Inherits ObservableCollection(Of Linha)

    Public Sub New()
        If Count = 0 Then
            For Each F In ApplicationSettings
                If F.Key.StartsWith("f") Then Add(New Linha With {.COD = F.Key.Substring(1), .NOME = F.Value.ToString.Substring(1), .TIPO = F.Value.ToString.Chars(0)})
            Next
        End If
    End Sub

    Public Function AddFav(linha As Linha) As String

        If ApplicationSettings.Contains("f" & linha.COD) Then
            Return "Já está nos favoritos."
        Else
            ApplicationSettings.Add("f" & linha.COD, linha.TIPO & linha.NOME)
            ApplicationSettings.Save()
            Add(linha)
            Return "Adicionado!"
        End If

    End Function

    Public Function DelFav(linha As Linha) As String

        If ApplicationSettings.Remove("f" & linha.COD) Then
            ApplicationSettings.Save()
            Remove(linha)
            Return "Removido!"
        Else
            Return "Não removido."
        End If

    End Function

End Class