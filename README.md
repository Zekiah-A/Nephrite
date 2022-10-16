# Nephrite - Easy and quick C# repls.
### A minimal interpreted C# lang, with built in reflection, ideal for easy .NET CLI server REPLs. 

## Getting started
- Setup is easy with nephrite, simply put the following in your project, and the repl will be ready to run.
   ```csharp
   var repl = new NephriteRepl();
   await NephriteRepl.Start();
   ```

- If you want more control over how it executes, use `NephriteRunner` directly to be able to control exactly how you want nephrite to execute. See the following for an example.
  ```csharp
  var runner = new NephriteRunner();
  await NephriteRunner.Execute("var x = 'Hello World!'; writeLine(x);");
  ```
  
## How it works
Nephrite is built upon a simple interpreter model, allowing it to be much quicker and lighter than other repls. With similar syntax to conventional C#, and a very simple language design, learning how to get around Nephrite is an easy feat.

Nephrite makes use of reflection, in order to be able to access and modify variables throught your project as it runs, regardless if they are instanced, private, or static. Allowing you to quickly configure, or modify your server without any downtime.

### WARNING: This is pre-pre alpha software, and there may be bugs!