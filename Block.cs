class Block {
  // Block id, the index of the block in the Config.blocks array
  public int id;

  // Block range
  public int start, size;

  // Assigned ips
  public List<string> ips = new List<string>();

  // Writes and if any writes have been applied
  public bool writtenTo = false;
  public List<Write> writes = new List<Write>();

  // Constructor
  public Block(int id, int start, int size) {
    this.id = id;
    this.start = start;
    this.size = size;
  }

  // Write data to block
  public void writeToBlock(int offset, byte[] data) {
    var write = new Write() { offset = offset, data = data, ips_left = new List<string>(ips) };
    writes.Add(write);
  }

  // Get block string representation
  public override string ToString() {
    return String.Format("Block {0}, Assigned IPs: {1}", this.id, String.Join(", ", this.ips));
  }
}