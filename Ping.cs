using System.Net.NetworkInformation;

class Ping {
  // Event invoked when a ping response is received
  public delegate void EventHandler(string ip, Block block, byte[] data);
  public event EventHandler? OnReceive;

  // Send a payload to a specific ip via ping
  public void sendData(string ip, Block block, byte[] data)
  {
    new Thread(() => {
      try
      {
        // Send ping
        var options = new PingOptions(1, true);
        var ping = new System.Net.NetworkInformation.Ping();
        ping.SendAsync(ip, 10000, data, options);

        // Remove variables
        ip = default(string);
        data = default(byte[]);
        GC.Collect();

        // Wait for response
        ping.PingCompleted += (s, e) => {
          if (e?.Reply?.Status == IPStatus.Success)
          {
            OnReceive?.Invoke(e.Reply.Address.ToString(), block, e.Reply.Buffer);
          }
        };
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }).Start();
  }
}