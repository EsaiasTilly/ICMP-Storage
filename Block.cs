class Block {
  public int id;
  public int start, size;
  public List<string> ips = new List<string>();
  public bool writtenTo = false;
  public List<Write> writes = new List<Write>();

  public Block(int id, int start, int size) {
    this.id = id;
    this.start = start;
    this.size = size;
  }

  public void writeToBlock(int offset, byte[] data) {
    var write = new Write() { offset = offset, data = data, ips_left = new List<string>(ips) };
    writes.Add(write);
  }

  public override string ToString() {
    //return String.Format("Block {0}, Assigned IPs: {1}, Written to: {2}", this.id, String.Join(", ", this.ips), this.writtenTo ? "Yes" : "No");
    return String.Format("Block {0}, Assigned IPs: {1}", this.id, String.Join(", ", this.ips));
  }
}