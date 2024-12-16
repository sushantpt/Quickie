using Figgle;
using static System.Console;

Clear();
const string headerText = "Quickie";
var asciiArt = FiggleFonts.Slant.Render(headerText);
WriteLine(asciiArt, System.Drawing.Color.Crimson);
            
WriteLine("\n---------------------------------------\n", System.Drawing.Color.DarkGreen);

WriteLine("Welcome to the Quickie Console App!");
WriteLine("Build apps QUICKLY.");

WriteLine("Documentation: ");
WriteLine("https://github.com/sushantpt/Quickie", System.Drawing.Color.Cyan);

WriteLine("GitHub Repo: ");
WriteLine("https://github.com/sushantpt/Quickie", System.Drawing.Color.Cyan);

WriteLine("\n---------------------------------------\n", System.Drawing.Color.Gray);

WriteLine("Enjoy using Quickie!");
WriteLine("Press any key to exit...", System.Drawing.Color.Green);
            
ReadKey(); 