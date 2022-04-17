class Config {
  // Static config
  public static int BLOCK_COUNT = 1;
  public static int BLOCK_SIZE = 32;
  public static int READ_TIMEOUT = 5000;
  public static int IPS_PER_BLOCK = 2;
  public static List<string> IPS = new List<string>() {
    // Google IPs
    "172.253.62.100",
    "172.253.62.101",
    "172.253.62.102",
    "172.253.62.113",
    "172.253.62.138",
    "172.253.62.139",
    "172.253.122.94",

    // Amazon IPs
    "54.239.28.85",
    "205.251.242.103",
    "176.32.103.205",
    "52.119.174.16",
    "52.119.171.206",
    "52.119.167.231",
    "54.239.18.172",
    "54.239.19.238",
    "52.94.225.242"
  };

  // Dynamic config
  public static bool running = true;
  public static Block[] blocks = new Block[BLOCK_COUNT];
  public static List<Read> reads = new List<Read>();
}