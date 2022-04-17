using System.Text;

class UserInterface {
  private string currentPath = "/";

  public UserInterface() {
    new Thread(() => {
      while(true) {
        
      }
    }).Start();
  }

  // Select a block
  private Block selectBlock() {
    Console.WriteLine("\nChoose a block:");
    for (int i = 0; i < Config.blocks.Length; i++)
      Console.WriteLine("{0}. {1}", i, Config.blocks[i]);
    Console.Write("\n> ");
    var blockId = int.Parse(Console.ReadLine());
    return Config.blocks[blockId];
  }

  // Write data
  private void writeData() {
    var block = selectBlock();
    block.writeToBlock(0, Encoding.UTF8.GetBytes("Hello World"));
  }

  // Read a range of data
  private void readRange() {
    // Ask for read offset
    Console.WriteLine("\nRead offset:");
    Console.Write("\n> ");
    var offset = int.Parse(Console.ReadLine());

    // Ask for a read size
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