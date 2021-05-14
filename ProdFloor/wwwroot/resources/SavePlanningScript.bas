Attribute VB_Name = "Module1"
Public Sub SaveAttachmentsToDisk(MItem As Outlook.MailItem)
  Dim oAttachment As Outlook.Attachment
  Dim sSaveFolder As String
  Dim LDate As String
  LDate = Format(Date, "MM-dd-yyyy")
  sSaveFolder = "C:\ProdFloorOld80\wwwroot\resources\Planning\"
  For Each oAttachment In MItem.Attachments
    oAttachment.SaveAsFile sSaveFolder & "PlanningScheduleReport" + LDate + ".xlsx"
  Next
    Shell "C:\ProdFloorOld80\wwwroot\resources\PlanningUpdate.bat"
End Sub
