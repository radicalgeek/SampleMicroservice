#!/bin/bash
export EnableNuGetPackageRestore=true
cd .nuget
test -e nuget.config || ln -s NuGet.Config nuget.config
test -e nuget.exe || ln -s NuGet.exe nuget.exe
test -e nuget.targets || ln -s NuGet.targets nuget.targets
cd  ..
nuget restore Prototype.sln
xbuild
