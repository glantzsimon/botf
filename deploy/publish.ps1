param([String]$publishPassword='', [String]$env='')

$publishDir = "publish"
$appDir = "webapp"
$projectPath = "WebApplication\WebApplication.csproj"
$migrationsExecutable = "K9.DataAccessLayer.dll"
$configPath = Resolve-Path -Path ".\webapp\webapplication\web.config"
$startupPath = ".\WebApplication\bin\$env"
	
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

function _Build() {
  echo "Building App"
  
  pushd $appDir
  ProcessErrors
  
  Msbuild $projectPath /p:Configuration=$env /P:OutputPath=bin\$env
  ProcessErrors
  popd
}

function _MigrateDatabase() {
  echo "Preparing to migrate database"
  
  pushd $appDir  
  ProcessErrors
    
  echo "Migrating database"  
  ..\tools\migrate.exe $migrationsExecutable /startUpDirectory=$startupPath  /startUpConfigurationFile=$configPath
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
	_Build
	_MigrateDatabase
    _Publish
  }
  Catch {
    Write-Error $_.Exception
    exit 1
  }
}

Main