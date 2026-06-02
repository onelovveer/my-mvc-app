param(
  [string]$ConnectionString = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FitnessClubDB;Integrated Security=True;"
)

Add-Type -AssemblyName System.Data

$cn = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
$cn.Open()

function ExecScalar([string]$sql) {
  $cmd = $cn.CreateCommand()
  $cmd.CommandText = $sql
  return $cmd.ExecuteScalar()
}

$services = ExecScalar "SELECT COUNT(1) FROM dbo.Services"
$users = ExecScalar "SELECT COUNT(1) FROM dbo.Users"
$subs = ExecScalar "SELECT COUNT(1) FROM dbo.Subscriptions"

$cn.Close()

Write-Output ("Services={0}" -f $services)
Write-Output ("Users={0}" -f $users)
Write-Output ("Subscriptions={0}" -f $subs)

