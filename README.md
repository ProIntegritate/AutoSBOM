# AutoSBOM
A .NET auto SBOM Generator from Reflection.Assembly() + Embedded resource file (project.assets.json)

Basically these extract the Reflection.Assemblys + References from the resource file and outputs them into a CSV format, the advantage here is that you do not need any external tools to generate this information. Please note that there is no XML/JSON formatting, if you want that - add it yourself.

The project is released "as-is", i currently do not need to generate any SBOM information, but if and when i do, i will update it with a proper output module. But i don't want to stop others from using this code to skip the boring AF part about generating this information because a bunch of paperpushers thinks that this actually is useful.

Your job is to import the "project.assets.json" file as an Embedded Resource into the project/solution (see below on how to do that). After that, you just call the function and it will spit out all the packages.

As an example, this is what you get:

![Output](SBOM.jpg)

VB .NET and C# versions available. Works with .NET5 and up, 4.8 and below are NOT supported.

CC0 so go nuts.

# Adding a reference to project.assets.json

Start by right-clicking the project, then add and existing item:<br/>

![Output](reference_001.png)

Then navigate to the obj folder and select project.assets.json. Select "Add as Link" from options.<br/>

![Output](reference_002.png)

Now, in the solution explorer, click on the referenced project.assets.json file, set to Embedded Resource and Always copy.<br/>

![Output](reference_003.png)

NOTE: This has to be done for all projects in the solution, and you have to call the function from each project in the solution to get the results.

For a project.assets.json file to exist in the Obj folder, you have to add some nuget packages. If the file is missing and you know you have added nuget packages, just do a build and the file will appear.
