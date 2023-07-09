
# Baked Resources

Baked Resources is a Nuget package that allows you to "bake files" into assembly and provides an easy-to-access API to retrieve its contents.

# How to use

Let's say you have a file called `` test.txt `` in your project and we want to baked it. To do that, intall the NuGet package `` BakedResoureces ``:
``` bash
dotnet add package BakedResources
```

and rename the file to `` test.baked.txt `` or `` test.b.txt ``.

You are done! 🚀

Now you can access the file contents like that:
``` csharp
var contents = BakedResources.TestApp.test;
```