class Config {
  /// 
  /// Static config
  /// 

  // The number of blocks to create
  public static int BLOCK_COUNT = 12;

  // The size of each block in bytes
  public static int BLOCK_SIZE = 512;

  // Timeout of reads in milliseconds
  public static int READ_TIMEOUT = 5000;

  // Nr of ips to assign to each block
  public static int IPS_PER_BLOCK = 5;

  // Path to IP list
  public static string IP_LIST_PATH = "./assets/ips.txt";

  /// 
  /// Dynamic config
  /// 

  // IP list
  public static List<string> IPS = new List<string>();

  // Indicates if the program is running
  public static bool running = true;

  // List of blocks
  public static Block[] blocks = new Block[BLOCK_COUNT];

  // List of read requests
  public static List<Read> reads = new List<Read>();
}