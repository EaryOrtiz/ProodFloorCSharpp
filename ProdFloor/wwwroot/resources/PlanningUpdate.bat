@ECHO OFF
powershell.exe -noprofile -command "Invoke-WebRequest -Uri http://10.113.0.117/PlanningReport/Update"
timeout 60 > NUL