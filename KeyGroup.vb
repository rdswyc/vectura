Imports System.Collections.Generic
Imports System.Globalization

Public Class AlphaKeyGroup(Of T)
    Inherits List(Of T)

    Shared SortedLocalGrouping As String = "#abcdefghijklmnopqrstuvwxyz*"
    Public Delegate Function GetKeyDelegate(Item As T) As String

    Public Property Key() As String
        Get
            Return _key
        End Get
        Set(value As String)
            _key = value
        End Set
    End Property
    Private _key As String

    Public Sub New(key As String)
        Me.Key = key
    End Sub

    Public Overrides Function ToString() As String
        Return Key
    End Function

    Private Shared Function CreateGroups(NameList As String) As List(Of AlphaKeyGroup(Of T))
        Dim list As New List(Of AlphaKeyGroup(Of T))()

        For Each key As Char In NameList.ToCharArray()
            list.Add(New AlphaKeyGroup(Of T)((key.ToString())))
        Next

        Return list
    End Function

    Public Shared Function CreateGroups(Items As IEnumerable(Of T), CI As CultureInfo, GetKey As GetKeyDelegate, Sort As Boolean) As List(Of AlphaKeyGroup(Of T))

        Dim list As List(Of AlphaKeyGroup(Of T)) = CreateGroups(SortedLocalGrouping)

        For Each item As T In Items

            Dim index As Integer = 0
            Dim label As String = GetKey(item)
            index = SortedLocalGrouping.IndexOf(label(0).ToString().ToLower())

            If index >= 0 AndAlso index < list.Count Then
                list(index).Add(item)
            End If
        Next

        If Sort Then
            For Each group As AlphaKeyGroup(Of T) In list
                group.Sort(Function(c0, c1) CI.CompareInfo.Compare(GetKey(c0), GetKey(c1)))
            Next
        End If

        Return list
    End Function

End Class

Public Class KeyedList(Of TKey, TItem)
    Inherits List(Of TItem)
    Public Property Key() As TKey

    Public Sub New(Key As TKey, Items As IEnumerable(Of TItem))
        MyBase.New(Items)
        Me.Key = Key
    End Sub

    Public Sub New(Grouping As IGrouping(Of TKey, TItem))
        MyBase.New(Grouping)
        Key = Grouping.Key
    End Sub
End Class