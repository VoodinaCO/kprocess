param (
    [string]$webConfig = "c:\inetpub\wwwroot\Web.config"
)

$doc = (Get-Content $webConfig) -as [Xml];

$connStr = $doc.configuration.connectionStrings.add | Where-Object {$_.name -eq "KsmedEntities"};
if ($connStr) {
	$connStr.connectionString = "metadata=          res://KProcess.Ksmed.Data/KL2.csdl|          res://KProcess.Ksmed.Data/KL2.ssdl|          res://KProcess.Ksmed.Data/KL2.msl;          provider=System.Data.SqlClient;          provider connection string=&quot;Data Source=" + (Get-ChildItem env:DATASOURCE).Value + ";Initial Catalog=" + (Get-ChildItem env:DATABASE).Value + ";User ID=KL2User;Password=B4A60FC44BCBEDC6267B80118219D235;MultipleActiveResultSets=True&quot;";
}

$appSetting = $doc.configuration.appSettings.add | Where-Object {$_.key -eq "ApiServerUri"};
if ($appSetting) {
	$appSetting.value = (Get-ChildItem env:APISCHEME).Value + "://" + (Get-ChildItem env:API).Value + ":" + (Get-ChildItem env:APIPORT).Value;
}

$appSetting = $doc.configuration.appSettings.add | Where-Object {$_.key -eq "FileServerUri"};
if ($appSetting) {
	$appSetting.value = (Get-ChildItem env:FILESERVERSCHEME).Value + "://" + (Get-ChildItem env:FILESERVER).Value + ":" + (Get-ChildItem env:FILESERVERPORT).Value;
}

$doc.Save($webConfig);

$docFile = (Get-Content $webConfig);
$docFile = $docFile.Replace("&amp;", "&");
Set-Content -Path $webConfig -Value $docFile;