Keywords:   web map, .net core map, asp net core, linux map

# Getting Started Guide for building a Web Map with ASP.NET Core

This is the beginners guide to walk you through the steps to build a web gis application with Visual Studio Code on Windows. With the power of ASP.NET Core WebAPI, it allows to host the map services on linux and macOS.

Before starting, we need to prepare the environment. Please refer the following list.
> - [.NET Core SDK](https://www.microsoft.com/net/download/core)
> - [Visual Studio Code](http://code.visualstudio.com/Download)
> - Recommend Visual Studio Code Plugins: 
>   - C# 

All the tools in the list are cross-platform, which means you could play with it on Windows, Linux and macOS. We will show you how to run on Linux and macOS later. Here is [a great video from Microsoft](https://channel9.msdn.com/Blogs/dotnet/Get-started-with-VS-Code-using-CSharp-and-NET-Core) to guide you prepare the environment.

Get back to the topic. What we are going to talk about in this thread?

1. Create a webapi project
2. Add MapKit component and the 3rd part package to the reference list
3. Add our controller
4. Start server and test
5. Run on Linux
6. Run on macOS

## Create a webapi project
To create a WebAPI project, it is very easy with Visual Studio Code. Please follow me.

1. Open your command window (`Win` key and type `cmd`, it should be in the search result list). 
2. Navigate to a folder where you want to contain the sample we are going to create (e.g. `cd c:\C:\Users\[USER NAME]]\Documents\Projects`). 
3. Create the sample folder and enter to the folder by: 
    ```
    mkdir HelloMap
    cd HelloMap
    ```
4. Create DotNetCore project and open VSCode by CLI.
    ```
    dotnet new webapi
    code .
    ```
Now that we have created a WebAPI project template with VS Code. It contains a default `ValuesController`. Here can do a simple test. In the VS Code window, press ctrl + \` to open the terminal window. Type the following command and you will restore the required packages and start the server.
```
dotnet restore
dotnet run
```
If everything goes fine, you will get this output.

![quickstart-webapi-dnc-server-run](http://p1.bpimg.com/567571/97442afea50e01b2.png)

The line with red underscroll is the address and port we will test in web browser. Type the address in browser: `http://localhost:5000/api/values`. You will see the first cross-plaform web API app running with VS Code.

## Add MapKit component and the 3rd part package to the reference list
In this section, we will add several dependencies into the project. Including:

1. CoreCompat.System.Drawing. This is a System.Drawing implementation for .NET Core. Refer [this project](https://github.com/CoreCompat/CoreCompat) for detail.
2. SGMapKit.WebApi (.NET Core version)
3. SGMapKit.Core (.NET Core version)

`CoreCompat.System.Drawing` is a nuget package. So goto VS Code and open up the *HelloMap.csproj* file. Find the `ItemGroup` node that contains the package references. Add the `CoreCompat.System.Drawing` package into this node. It will be like this:
```
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore" Version="1.0.3" />
  <PackageReference Include="Microsoft.AspNetCore.Mvc" Version="1.0.2" />
  <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="1.0.1" />
  <PackageReference Include="CoreCompat.System.Drawing" Version="1.0.0-beta006" />
</ItemGroup>
```

Then create a new folder *Dependencies* in the project. Copy the assemblies *SGMapKit.Core.dll*, *SGMapKit.WebApi.dll* from the installed folder e.g *C:\Program Files (x86)\SlimGIS\SDK\3.0.0\WebApi.Dnc* to this new folder.

Then let's add the following node into the *HelloMap.csproj* file.
```
<ItemGroup>
  <Reference Include="SGMapKit.Core">
    <HintPath>Dependencies\SGMapKit.Core.dll</HintPath>
  </Reference>   
  <Reference Include="SGMapKit.WebApi">
    <HintPath>Dependencies\SGMapKit.WebApi.dll</HintPath>
  </Reference>
</ItemGroup>
```

Turn on terminal and type `dotnet restore`. When it restored complete, we are almost there to see the effect.

## Add our controller
To keep it simple, we will use the existing `ValuesController`. We add an action at the bottom of this class.
```
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;
using SlimGis.MapKit.WebAPI;
using SlimGis.MapKit.Geometries;
```

```
[HttpGet]
[Route("{z}/{x}/{y}")]
public IActionResult GetImage(int z, int x, int y)
{
    ShapefileLayer countriesLayer = new ShapefileLayer(@"AppData\countries-900913.shp");
    countriesLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#AAFFDF3E"), GeoColors.White));

    MapModel mapModel = new MapModel(GeoUnit.Meter);
    mapModel.Layers.Add(countriesLayer);


    return new XyzTileResult(mapModel, x, y, z);
}
```

In this method, we add a method and set its route to `{z}/{x}/{y}`. We expect our final address is like `values/0/0/0` which means fetch me back a tile image that is at zoom level 1, column 1 and row 1 in the tile matrix.

You might see there is a data in AppData folder, please copy the attached data to the specified *AppData* folder in the project.

## Start server and test
This step is the easiest one. Let's goto the terminal window and type 'dotnet run'. It will take few seconds to compile and start the server. Like the last time we make it run, goto the web browser and type the address we expected. [http://localhost:5000/api/values/0/0/0](http://localhost:5000/api/values/0/0/0). We will see the page returns me an image like this:

![quickstart-webapi-dnc-one-tile](http://p1.bqimg.com/567571/8ddb24ac730c71ab.png)

Then we create a HTML page names *index.html* and fill the html as following:
```
<!DOCTYPE html>
<html>
<head>
    <title>Quickstart Guide</title>
    <meta charset="utf-8" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.5.0/ol.css" rel="stylesheet" />
    <style>
        #map {
            width: 400px;
            height: 400px;
        }
    </style>
</head>
<body>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.5.0/ol.js" type="text/javascript"></script>
    <div id="map" />

    <script type="text/javascript">
        let map = new ol.Map({
            layers: [
                new ol.layer.Tile({ source: new ol.source.OSM() }),
                new ol.layer.Tile({
                    source: new ol.source.XYZ({ url: 'http://localhost:5000/api/values/{z}/{x}/{y}' })
                })
            ],
            target: 'map',
            view: new ol.View({ center: [0, 0], zoom: 2 })
        });
    </script>
</body>
</html>
```

Congratulations! Our first ASP.NET Core project is created. Double click the html file and see the effect. Pretty good. I finally can work without the heavy-weight IDE Visual Studio.

![quickstart-webapi-dnc](http://p1.bpimg.com/567571/20210147ce952e71.png)

## Related Resources
- [Source code](https://github.com/SlimGIS/Quickstart-WebAPI-DotNetCore)
- [Getting started on macOS and Linux](https://slimgis.com/documents/getting-started-webapi-dnc-multi)
- [Features overview](https://slimgis.com/documents/features-overview-webapi-dnc)
- [API Reference](https://slimgis.com/documents/api-ref-webapi-dnc)

