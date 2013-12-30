Imports System.Globalization

Public Class Linha

    Private _cod As String
    Public Property COD() As String
        Get
            Return _cod
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_cod) Then
                _cod = value
            End If
        End Set
    End Property

    Private _nome As String
    Public Property NOME() As String
        Get
            Return _nome
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_nome) Then
                _nome = value
            End If
        End Set
    End Property

    Private _tipo As String
    Public Property TIPO() As String
        Get
            Return _tipo
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_tipo) Then
                _tipo = value
            End If
        End Set
    End Property

End Class

Public Class Horario

    Private _hora As String
    Public Property HORA() As String
        Get
            Return _hora
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_hora) Then
                _hora = value
            End If
        End Set
    End Property

    Private _ponto As String
    Public Property PONTO() As String
        Get
            Return _ponto
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_ponto) Then
                _ponto = value
            End If
        End Set
    End Property

    Private _dia As Byte
    Public Property DIA() As Byte
        Get
            Return _dia
        End Get
        Set(ByVal value As Byte)
            If Not value.Equals(_dia) Then
                _dia = value
            End If
        End Set
    End Property

    Private _num As String
    Public Property NUM() As String
        Get
            Return _num
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_num) Then
                _num = value
            End If
        End Set
    End Property

End Class

Public Class Ponto

    Private _nome As String
    Public Property NOME() As String
        Get
            Return _nome
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_nome) Then
                _nome = value
            End If
        End Set
    End Property

    Private _num As String
    Public Property NUM() As String
        Get
            Return _num
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_num) Then
                _num = value
            End If
        End Set
    End Property

    Private _lat As Double
    Public Property LAT() As Double
        Get
            Return _lat
        End Get
        Set(ByVal value As Double)
            If Not value.Equals(_lat) Then
                _lat = value
            End If
        End Set
    End Property

    Private _lon As Double
    Public Property LON() As Double
        Get
            Return _lon
        End Get
        Set(ByVal value As Double)
            If Not value.Equals(_lon) Then
                _lon = value
            End If
        End Set
    End Property

    Private _seq As Integer
    Public Property SEQ() As Integer
        Get
            Return _seq
        End Get
        Set(ByVal value As Integer)
            If Not value.Equals(_seq) Then
                _seq = value
            End If
        End Set
    End Property

    Private _tipo As String
    Public Property TIPO() As String
        Get
            Return _tipo
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_tipo) Then
                _tipo = value
            End If
        End Set
    End Property

    Private _grupo As String
    Public Property GRUPO() As String
        Get
            Return _grupo
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_grupo) Then
                _grupo = value
            End If
        End Set
    End Property

    Private _sentido As String
    Public Property SENTIDO() As String
        Get
            Return _sentido
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_sentido) Then
                _sentido = value
            End If
        End Set
    End Property

End Class

Public Class Coordenada

    Private _lat As Double
    Public Property LAT() As Double
        Get
            Return _lat
        End Get
        Set(ByVal value As Double)
            If Not value.Equals(_lat) Then
                _lat = value
            End If
        End Set
    End Property

    Private _lon As Double
    Public Property LON() As Double
        Get
            Return _lon
        End Get
        Set(ByVal value As Double)
            If Not value.Equals(_lon) Then
                _lon = value
            End If
        End Set
    End Property

End Class

Public Class Veiculo

    Private _prefixo As String
    Public Property PREFIXO() As String
        Get
            Return _prefixo
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_prefixo) Then
                _prefixo = value
            End If
        End Set
    End Property

    Private _lat As String
    Public Property LAT() As String
        Get
            Return _lat
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_lat) Then
                _lat = value
            End If
        End Set
    End Property

    Private _lon As String
    Public Property LON() As String
        Get
            Return _lon
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_lon) Then
                _lon = value
            End If
        End Set
    End Property

    Private _hora As String
    Public Property HORA() As String
        Get
            Return _hora
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_hora) Then
                _hora = value
            End If
        End Set
    End Property

    Private _adapt As Boolean
    Public Property ADAPT() As Boolean
        Get
            Return _adapt
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_adapt) Then
                _adapt = value
            End If
        End Set
    End Property

    Private _linha As String
    Public Property LINHA() As String
        Get
            Return _linha
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_linha) Then
                _linha = value
            End If
        End Set
    End Property

End Class