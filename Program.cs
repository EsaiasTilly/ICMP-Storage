using System.Text;

class Program {
  private static Ping ping = new Ping();

  public static void Main() {
    // Create random
    var random = new Random();

    // Set ping event
    ping.OnReceive += RecievePing;

    // Read IP list
    var ips = File.ReadAllLines(Config.IP_LIST_PATH);
    Config.IPS.AddRange(ips);
    Console.WriteLine("Read " + ips.Length + " IPs");

    // Create blocks
    Console.WriteLine("Creating blocks:");
    for (int i = 0; i < Config.BLOCK_COUNT; i++) {
      var block = new Block(i, i * Config.BLOCK_SIZE, Config.BLOCK_SIZE);
      for (int x = 0; x < Config.IPS_PER_BLOCK; x++)
        block.ips.Add(Config.IPS[random.Next(0, Config.IPS.Count)]);
      Config.blocks[i] = block;
      Console.WriteLine(block);
    }

    // Listen to user input
    var ui = new UserInterface();

    // Wait for first writes
    waitForFirstWrites();

    // Create root directory
    FileSystem.MakeDirectory("/");
  }

  // Wait for first block writes
  private static void waitForFirstWrites() {
    new Thread(() => {
      while(Config.running) {
        // Go through all blocks
        foreach(var block in Config.blocks) {
          // Skip if block has no writes
          if (block.writes.Count == 0) continue;

          // Skip if block has been written to
          if (block.writtenTo) continue;

          // Send first pings
          foreach(var ip in block.ips) {
            ping.sendData(ip, block, DataMerge.Merge(new byte[block.size], block.writes[0].data, block.writes[0].offset));
          }

          // Mark block as written
          block.writtenTo = true;

          // Remove first write
          block.writes.RemoveAt(0);
        }

        // Sleep
        Thread.Sleep(50);
      }
    }).Start();
  }

  // Handle a ping callback
  public static void RecievePing(string ip, Block block, byte[] data) {
    Console.WriteLine("{0}: {1}", ip, Encoding.UTF8.GetString(data));

    // Apply any writes
    int tries = 0;
    for (int i = 0; i < block.writes.Count; i++) {
      try
      {
        var write = block.writes[i];
        if (write.ips_left.Contains(ip)) {
          write.ips_left.Remove(ip);
          data = DataMerge.Merge(data, write.data, write.offset);
        }

        if (write.ips_left.Count == 0) {
          block.writes.Remove(write);
        }
      }
      catch {
        i = 0;
        tries++;
        if (tries > 10) {
          Console.WriteLine("Failed to apply write to block {0}", block.id);
          break;
        }
      }
    }

    // Read data
    for (int i = 0; i < Config.reads.Count; i++) {
      var read = Config.reads[i];
      if (read.addData(block.id, data, block.start)) i--;
    }

    // Send next ping
    ping.sendData(ip, block, data);
  }
}