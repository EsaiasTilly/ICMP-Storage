using System.Text;

class UserInterface {
  public UserInterface() {
    new Thread(() => {
      while(true) {
        // Show input possibilities
        Console.WriteLine("\nChoose an option:");
        Console.WriteLine("1. Write to a block");
        Console.WriteLine("2. Read a block");
        Console.WriteLine("3. Exit");
        Console.Write("\n> ");

        // Get user input
        var input = Console.ReadLine();

        // Write to a block
        if (input == "1") writeToBlock();

        // Read a block
        else if (input == "2") readBlock();

        // Exit
        else if (input == "3") {
          Config.running = false;
          break;
        }
      }
    }).Start();
  }

  private Block selectBlock() {
    Console.WriteLine("\nChoose a block:");
    for (int i = 0; i < Config.blocks.Length; i++)
      Console.WriteLine("{0}. {1}", i, Config.blocks[i]);
    Console.Write("\n> ");
    var blockId = int.Parse(Console.ReadLine());
    return Config.blocks[blockId];
  }

  private void writeToBlock() {
    var block = selectBlock();
    block.writeToBlock(0, Encoding.UTF8.GetBytes("Hello World"));
  }

  private void readBlock() {
    Console.WriteLine("\nRead offset:");
    Console.Write("\n> ");
    var offset = int.Parse(Console.ReadLine());

    Console.WriteLine("\nRead size:");
    Console.Write("\n> ");
    var size = int.Parse(Console.ReadLine());

    // Create read request
    var read = new Read(offset, size);
    var readRecieved = false;
    read.OnReceive += (byte[] data) => {
      Console.WriteLine("\nRead data:");
      Console.WriteLine(Encoding.UTF8.GetString(data));
      readRecieved = true;
    };
    Config.reads.Add(read);

    // Wait for read to be recieved
    double start = DateTime.Now.TimeOfDay.TotalMilliseconds;
    while(!readRecieved && DateTime.Now.TimeOfDay.TotalMilliseconds - start < Config.READ_TIMEOUT) {
      Thread.Sleep(50);
    }
  }
}