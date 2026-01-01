@echo off
setlocal enabledelayedexpansion

::Batch para subir proyecto API REST a la nube de Azure
::Al definir las variables tener en cuenta que algunos nombres de recursos de Azure deben ser unicos a nivel mundial
::Se recomienda añadir el nombre del proyecto antes del nombre del recurso para identificarlo y evitar que ya exista
::Si ocurre algun error, detener el proceso en la consola con CTRL+C varias veces, ir al portal de Azure y eliminar todo el grupo de recursos, corregir y reintentar
::Aunque puedes añadir az-login al script, como no es necesario logarse cada vez que corras el script, ejecutalo antes
::El parametro de az login use-device-code se usa cuando usas MFA o authenticator.
::By Gerson Azabache - bravedeveloper.com

:: Obtener el tiempo de inicio
for /f "tokens=1-2 delims=: " %%a in ('time /t') do (
    set startHour=%%a
    set startMinute=%%b
)

:: DEFINICION DE variables // No usar espacios ni caracteres especiales
set dbAdminUser=userdb
set dbAdminPassword="Aa123456"
set resourceGroup=ticketstore2025RG
set appServicePlan=ticketstore2025SP
set appServiceWebApp=ticketstore2025WA
set dbServer=ticketstore2025dbsrv
set dbName=ticketstoredb
::el nombre del storage account debe estar entre 3-24 caracteres y solo en minusculas
set storageAccount=ticketsacc2025
::set IP_PUBLICA=x.x.x.x

call az version

:: call az login
:: call az login --use-device-code

:: Obteniendo automaticamente tu IP Publica y asignandola a la variable IP_PUBLICA
::for /f "delims=" %%i in ('powershell -Command "(Invoke-WebRequest -uri http://ifconfig.me).Content"') do set IP_PUBLICA=%%i
for /f "delims=" %%i in ('curl -s http://ifconfig.me') do set IP_PUBLICA=%%i

echo 1/16: ========== Creando grupo de recursos ==========
call az group create --name %resourceGroup% --location westus3

echo 2/16: ========== Creando plan de App Service ==========
call az appservice plan create --name %appServicePlan% --resource-group %resourceGroup% --sku FREE --location westus3

echo 3/16: ========== Creando aplicacion web ==========
call az webapp create --resource-group %resourceGroup% --plan %appServicePlan% --name %appServiceWebApp% --runtime "dotnet:9"

echo 4/16: ========== Creando servidor SQL ==========
call az sql server create --name %dbServer% --resource-group %resourceGroup% --location westus3 --admin-user %dbAdminUser% --admin-password %dbAdminPassword%

echo 5/16: ========== Creando bd SQL ==========
call az sql db create --resource-group %resourceGroup% --server %dbServer% --name %dbName% --service-objective Basic --backup-storage-redundancy Local

echo 6/16: ========== Creando regla firewall para IP especifica ==========
call az sql server firewall-rule create --resource-group %resourceGroup% --server %dbServer% --name AllowMyPublicIP --start-ip-address %IP_PUBLICA% --end-ip-address %IP_PUBLICA%

echo 7/16: ========== Creando regla firewall para accesos Azure ==========
call az sql server firewall-rule create --resource-group %resourceGroup% --server %dbServer% --name AllowAzureIps --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

echo 8/16: ========== Creando Storage account ==========
call az storage account create --name %storageAccount% --resource-group %resourceGroup% --location westus3 --sku Standard_LRS --kind StorageV2 --allow-blob-public-access true --access-tier Cool --min-tls-version TLS1_2

echo 9/16: ========== Obteniendo cadena de conexion para bd ==========
for /f "tokens=*" %%i in ('az sql db show-connection-string --server %dbServer% --name %dbName% --client ado.net --output tsv') do set defaultConnection=%%i
echo !defaultConnection!

:: Reemplazando <user> y <password> en la cadena de conexion
set "defaultConnection=!defaultConnection:<username>=%dbAdminUser%!"
set "defaultConnection=!defaultConnection:<password>=%dbAdminPassword%!"

echo 10/16: ========== Configurando cadena de conexion de la bd en la web app ==========
call az webapp config connection-string set --resource-group %resourceGroup% --name %appServiceWebApp% --settings defaultConnection="!defaultConnection!" --connection-string-type SQLAzure

echo 11/16: ========== Obteniendo cadena de conexion Azure Storage ==========
for /f "tokens=*" %%i in ('az storage account show-connection-string --resource-group %resourceGroup% --name %storageAccount% --output tsv') do set AzureStorage=%%i
echo !AzureStorage!

echo 12/16: ========== Configurando cadena de conexion Azure Storage ==========
call az webapp config connection-string set --resource-group %resourceGroup% --name %appServiceWebApp% --settings AzureStorage="!AzureStorage!" --connection-string-type Custom

echo 13/16: ========== Configurando appsettings JWTKEY y ASPNETCORE_ENVIRONMENT ==========
call az webapp config appsettings set --resource-group %resourceGroup% --name %appServiceWebApp% --settings "JWT:JWTKey"=1VGSDF8VHDFBN88P7INGILDFY8U7KNMFHBVRWCGEW78WE0GC820CWG2FVC8VG80WE48G4W8EC4W1FV
call az webapp config appsettings set --resource-group %resourceGroup% --name %appServiceWebApp% --settings ASPNETCORE_ENVIRONMENT=Production

echo 14/16: ========== Publicando aplicacion ==========
call dotnet publish -c Release -o ./publish

echo 15/16: ========== Comprimiendo archivos de publicacion ==========
call powershell Compress-Archive -Path .\publish\* -DestinationPath .\publish.zip -Update

echo 16/16: ========== Desplegando aplicacion a Azure App Service ==========
::call az webapp deployment source config-zip --resource-group %resourceGroup% --name %appServiceWebApp% --src ./publish.zip
call az webapp deploy --resource-group %resourceGroup% --name %appServiceWebApp% --src-path .\publish.zip --type zip

echo Despliegue completado.

:: Obtener el tiempo de finalizacion
for /f "tokens=1-2 delims=: " %%a in ('time /t') do (
    set endHour=%%a
    set endMinute=%%b
)

:: Calculando la diferencia en minutos
set /a startTotalMinutes=startHour*60 + startMinute
set /a endTotalMinutes=endHour*60 + endMinute
set /a duration=endTotalMinutes - startTotalMinutes

REM Mostrar el tiempo total de ejecución
echo El script tuvo una duracion de %duration% minutos.

pause
endlocal