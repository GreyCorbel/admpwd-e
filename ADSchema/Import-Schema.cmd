REM This imports AD schema changes
REM You must be Schema admin to run the line below
ldifde.exe -i -f .\AdmPwd_Full.ldf -c "cn=X" "#schemaNamingContext" -v

REM This imports extended permissions definition
REM You must be Enterprise admin to run the line below
ldifde.exe -i -f .\ExtendedRights.ldf -c "cn=X" "#configurationNamingContext" -v
