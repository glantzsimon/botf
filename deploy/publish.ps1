param([String]$publishPassword='', [String]$env='')

$publishDir = "publish"
$appDir = "webapp"
$projectPath = "WebApplication\WebApplication.csproj"
$applicationExecutablePath = "WebApplication\bin\$env\K9.DataAccessLayer.dll"
	
function ProcessErrors(){
  if($? -eq $false)
  {
    throw "The previous command failed (see above)";
  }
}

function _CreateDirectory($dir) {
  If (-Not (Test-Path $dir)) {
    New-Item -ItemType Directory -Path $dir
  }
}

function _MigrateDatabase() {
  echo "Preparing to migrate database"
  
  pushd $appDir  
  ProcessErrors
    
  echo "Migrating database"
  .\packages\EntityFramework.6.1.3\tools\migrate.exe $applicationExecutablePath /startupConfigurationFile=”..\\web.config”
  ProcessErrors
  popd
}

function _Publish() {
  echo "Publishing App"
  
  pushd $appDir
  ProcessErrors
  
  _CreateDirectory $publishDir
  ProcessErrors
  
  Msbuild $projectPath /p:DeployOnBuild=true /p:PublishProfile=$env /p:AllowUntrustedCertificate=true /p:Password=$publishPassword
  ProcessErrors
  popd
}

function Main {
  Try {
	_MigrateDatabase
    _Publish
  }
  Catch {
    Write-Error $_.Exception
    exit 1
  }
}

Main