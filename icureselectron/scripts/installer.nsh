; scripts/installer.nsh
; Custom NSIS hooks for iOSBridge.
; Copies bundled dependencies from the installer's resources to C:\iCures,
; and removes them on uninstall.

!macro customInstall
  DetailPrint "Installing iOSBridge dependencies to C:\iCures..."
  CreateDirectory "C:\iCures"
  ; robocopy exit codes 0-7 are success (bitmask). 8+ means an actual error.
  ; /E  = copy subdirectories including empty ones
  ; /IS = overwrite same-size files   /IT = overwrite tweaked files
  ; /NFL /NDL /NJH /NJS /NP = suppress all console output
  nsExec::ExecToLog '"robocopy" "$INSTDIR\resources\iCures" "C:\iCures" /E /IS /IT /NFL /NDL /NJH /NJS /NP'
  Pop $0
  DetailPrint "Dependencies ready."
!macroend

!macro customUninstall
  ; Only remove the Dependencies subfolder we placed — leave any user files alone.
  RMDir /r "C:\iCures\Dependencies"
!macroend
