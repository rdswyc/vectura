Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.IO.IsolatedStorage.IsolatedStorageSettings
Imports System.Windows.Resources
Imports System.Threading

Partial Public Class App
    Inherits Application

    Public Property RootFrame As PhoneApplicationFrame
    Public Shared Property Local As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
    Public Shared Property ActiveLinha As Linha = Nothing

    Public Shared Sub ProgressMsg(Progress As ProgressIndicator, Message As String, Seconds As Single)
        Progress.Text = Message
        Progress.IsVisible = True
        Dim TimerOut As New Timer(New TimerCallback(Sub()
                                                        Deployment.Current.Dispatcher.BeginInvoke(Sub() Progress.IsVisible = False)
                                                        TimerOut = Nothing
                                                    End Sub), Nothing, CLng(Seconds * 1000), Timeout.Infinite)
    End Sub

    Public Sub New()
        InitializeComponent()
        InitializePhoneApplication()

        ' Show graphics profiling information while debugging.
        If System.Diagnostics.Debugger.IsAttached Then
            ' Display the current frame rate counters.
            Application.Current.Host.Settings.EnableFrameRateCounter = True

            ' Show the areas of the app that are being redrawn in each frame.
            'Application.Current.Host.Settings.EnableRedrawRegions = True

            ' Enable non-production analysis visualization mode, 
            ' which shows areas of a page that are handed off to GPU with a colored overlay.
            'Application.Current.Host.Settings.EnableCacheVisualization = True


            ' Disable the application idle detection by setting the UserIdleDetectionMode property of the
            ' application's PhoneApplicationService object to Disabled.
            ' Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
            ' and consume battery power when the user is not using the phone.
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled
        End If

        TiltEffect.TiltableItems.Add(GetType(TiltableControl))

    End Sub

    Private Sub Application_Activated(ByVal sender As Object, ByVal e As ActivatedEventArgs)
        If ApplicationSettings.Contains("l") Then
            ActiveLinha = New Linha With {.COD = ApplicationSettings("l").ToString().Substring(0, 3), .TIPO = ApplicationSettings("l").ToString().Chars(3), .NOME = ApplicationSettings("l").ToString().Substring(4)}
        End If
    End Sub

    Private Sub Application_Deactivated(ByVal sender As Object, ByVal e As DeactivatedEventArgs)
        If ActiveLinha IsNot Nothing Then
            If ApplicationSettings.Contains("l") Then
                ApplicationSettings("l") = ActiveLinha.COD & ActiveLinha.TIPO & ActiveLinha.NOME
            Else
                ApplicationSettings.Add("l", ActiveLinha.COD & ActiveLinha.TIPO & ActiveLinha.NOME)
            End If
            ApplicationSettings.Save()
        End If
    End Sub

    Private Sub Application_Launching(ByVal sender As Object, ByVal e As LaunchingEventArgs)
    End Sub

    Private Sub Application_Closing(ByVal sender As Object, ByVal e As ClosingEventArgs)
    End Sub

    Private Sub RootFrame_NavigationFailed(ByVal sender As Object, ByVal e As NavigationFailedEventArgs)
        If Diagnostics.Debugger.IsAttached Then Diagnostics.Debugger.Break()
    End Sub

    Public Sub Application_UnhandledException(ByVal sender As Object, ByVal e As ApplicationUnhandledExceptionEventArgs) Handles Me.UnhandledException
        If Diagnostics.Debugger.IsAttached Then
            Diagnostics.Debugger.Break()
        Else
            e.Handled = True
            MessageBox.Show("Ocorreu um erro!") 'MessageBox.Show(e.ExceptionObject.Message & Environment.NewLine & e.ExceptionObject.StackTrace, "Error", MessageBoxButton.OK)
            RootFrame.Navigate(New Uri("/MainPage.xaml", UriKind.Relative))
        End If
    End Sub

#Region "Phone application initialization"
    ' Avoid double-initialization
    Private phoneApplicationInitialized As Boolean = False

    ' Do not add any additional code to this method
    Private Sub InitializePhoneApplication()
        If phoneApplicationInitialized Then
            Return
        End If

        ' Create the frame but don't set it as RootVisual yet; this allows the splash
        ' screen to remain active until the application is ready to render.
        RootFrame = New TransitionFrame() 'PhoneApplicationFrame()
        AddHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication

        ' Handle navigation failures
        AddHandler RootFrame.NavigationFailed, AddressOf RootFrame_NavigationFailed

        ' Ensure we don't initialize again
        phoneApplicationInitialized = True
    End Sub

    ' Do not add any additional code to this method
    Private Sub CompleteInitializePhoneApplication(ByVal sender As Object, ByVal e As NavigationEventArgs)
        ' Set the root visual to allow the application to render
        If RootVisual IsNot RootFrame Then
            RootVisual = RootFrame
        End If

        ' Remove this handler since it is no longer needed
        RemoveHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication
    End Sub
#End Region

End Class