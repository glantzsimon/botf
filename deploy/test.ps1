$appDir = "webapp"
$migrationsExecutable = "K9.DataAccessLayer.dll"
$configPath = Resolve-Path -Path ".\webapp\webapplication\web.config"
$startupPath = ".\WebApplication\bin\"
$migrationConfiguration = "LocalConfiguration"
	
function ProcessErrors(){
  if($? -eq $false)
  {
    throw "The previous command failed (see above)";
  }
}

function _Build() {
  echo "Building App"
  
  pushd $appDir
  ProcessErrors
  
  Msbuild /p:Configuration=Test
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

function Main {
  Try {
	_Build
	_MigrateDatabase
  }
  Catch {
    Write-Error $_.Exception
    exit 1
  }
}

Main