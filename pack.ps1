param (
    [string]$version = (Get-Date -Format "999.yyMM.ddHH.mmss")
) 

dotnet clean
dotnet pack -c Release ./src/SourceGeneratorTemplate.Package/SourceGeneratorTemplate.Package.csproj --verbosity normal /p:Version=$version -o ./nugets
