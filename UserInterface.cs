using System.Text;

class UserInterface {
  private string currentPath = "/";

  public UserInterface() {
    new Thread(() => {
      // Display motd
      Console.WriteLine("Welcome to ICMP-Storage!");
      Console.WriteLine("\nThe file system is stored on the internet using");
      Console.WriteLine("the ICMP protocol.\n");
      Console.WriteLine("Type 'help' for a list of commands.");

      // Start listening for user input
      while(true) {
        Console.Write(currentPath + "> ");
        string input = Console.ReadLine();

        string command = input.Split(' ')[0];
        string[] arguments = input.Split(' ').Skip(1).ToArray();

        switch(command) {
          case "help":
            Console.WriteLine("help - Display this help message");
            Console.WriteLine("ls - List files and directories");
            Console.WriteLine("cd - Change directory");
            Console.WriteLine("cat - Display file contents");
            Console.WriteLine("write - Write to a file");
            Console.WriteLine("exit - Exit the program");
            break;
          case "mkdir":
            FileSystem.MakeDirectory(Path.Combine(currentPath, arguments[0]));
            break;
          case "ls":
            this.ls();
            break;
          case "cd":
            this.cd(arguments[0]);
            break;
          case "nano":
            break;
          case "write":
            break;
          case "exit":
            Environment.Exit(0);
            break;
          default:
            Console.WriteLine("Unknown command");
            break;
        }
      }
    }).Start();
  }

  private void ls() {
    var fs = FileSystem.ReadFilePointers();
    
    // Print all directories
    foreach(var dir in fs.directories) {
      Console.WriteLine(dir);
    }

    // Print all files
    foreach(var file in fs.files) {
      Console.WriteLine(file.path);
    }
  }

  private void cd(string relPath) {
    var fs = FileSystem.ReadFilePointers();

    // Check if directory exists
    if(fs.directories.Contains(Path.Combine(currentPath, relPath))) {
      currentPath = Path.Combine(currentPath, relPath);
    } else {
      Console.WriteLine("Directory does not exist");
    }
  }
}