Public Sub SaveAttachmentsToDisk(MItem As Outlook.MailItem)
  Dim oAttachment As Outlook.Attachment
  Dim sSaveFolder As String
  Dim LDate As String
  LDate = Format(Date, "MM-dd-yyyy")
  sSaveFolder = "C:\ProdFloor\wwwroot\resources\Planning\"
  For Each oAttachment In MItem.Attachments
    oAttachment.SaveAsFile sSaveFolder & "PlanningScheduleReport" + LDate + ".xlsx"
  Next
End Sub