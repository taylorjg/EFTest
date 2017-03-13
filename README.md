## Description

This is my first experiment with EF Core.

## Gotcha

I ran into a problem trying to issue `dotnet ef` commands:

```
$ dotnet ef migrations add migration1
No executable found matching command "dotnet-ef"
```

After some research, I found out that I needed to add `Microsoft.EntityFrameworkCore.Tools.DotNet`
to the `.csproj` file as a `DotNetCliToolReference` as opposed to a `PackageReference`:

```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="1.1.0" />
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.0" />
  </ItemGroup>
</Project>
```

## Links

* http://thedatafarm.com/data-access/no-executable-found-matching-command-dotnet-ef/
